using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace VoxelSeeds
{
    class Automaton
    {
        class LivingVoxel
        {
            public LivingVoxel(int x, int y, int z, int generation, int resources, int ticks)
            {
                X = x;
                Y = y;
                Z = z;
                Generation = generation;
                Resources = resources;
                Ticks = ticks;
            }

            public int X;
            public int Y;
            public int Z;

            public int Generation;
            public int Resources;

            public int Ticks;
        };

        public Automaton(int sizeX, int sizeY, int sizeZ, LevelType lvlType, int seed)
        {
            _map = new Map(sizeX, sizeY, sizeZ, lvlType, seed);
            _livingVoxels = new Dictionary<Int32,LivingVoxel>();
        }

        private void InsertVoxel(Int32 positionCode, VoxelType type, int generation, bool living, int resources, int ticks)
        {
            var pos = _map.DecodePosition(positionCode);
            if (_map.IsInside(pos.X, pos.Y, pos.Z))
            {
                _map.Set(positionCode, type, living);
                if (living)
                {
                    if (_livingVoxels.ContainsKey(positionCode))
                    {
                        _livingVoxels[positionCode] = new LivingVoxel(pos.X, pos.Y, pos.Z, generation, resources, ticks);
                    }
                    else
                        _livingVoxels.Add(positionCode, new LivingVoxel(pos.X, pos.Y, pos.Z, generation, resources, ticks));
                }
            }
        }

        public void InsertSeed(int x, int y, int z, VoxelType type)
        {
            Int32 pos = _map.EncodePosition(x, y, z);

            InsertVoxel(pos, type, 0, true, 0, 0);
        }


        // All existing voxels in the current time step.
        Map _map;
        public Map Map { get { return _map; } }

        // Extra data for all living voxels.
        // TODO: hashmap... ersparrt auch das living flag in der map
        Dictionary<Int32, LivingVoxel> _livingVoxels;

        private void IterateNeighbours(Action<int, int, int> func )
        {
            for (int z = 0; z < 3; ++z)
                for (int y = 0; y < 3; ++y)
                    for (int x = 0; x < 3; ++x)
                        func(x, y, z);
        }

        private void update(ref ConcurrentDictionary<Int32, VoxelInfo> results, ref Action<IEnumerable<Voxel>, IEnumerable<Voxel>> updateInstanceData, out int newBiomass, out int newParasites)
        {
            newBiomass = 0;
            newParasites = 0;

            List<Voxel> deleteList = new List<Voxel>();
            List<Voxel> insertionList = new List<Voxel>();
            foreach( KeyValuePair<Int32, VoxelInfo> vox in results )
            {
                var po = _map.DecodePosition(vox.Key);
                Debug.Assert( _map.IsInside(po.X, po.Y, po.Z) );
                VoxelType old = _map.Get(vox.Key);
                if (old != (int)VoxelType.EMPTY)
                {
                    if (TypeInformation.IsParasite(old)) --newParasites;
                    // Delete the old one and create a new one (outside branch).
                    deleteList.Add(new Voxel(vox.Key, old));
                }
                if (TypeInformation.IsParasite(vox.Value.Type)) ++newParasites;
                else if (vox.Value.Type != VoxelType.EMPTY) ++newBiomass;

                InsertVoxel(vox.Key, vox.Value.Type, vox.Value.Generation, vox.Value.Living, vox.Value.Resources, vox.Value.Ticks);

                // Insert to instance data only if visible
                if( !_map.IsOccluded(vox.Key) )
                    insertionList.Add(new Voxel( vox.Key, vox.Value.Type ));
                // The map set can cause neighboured voxels to be occluded
                // if so delete that from GPU.
                Action<Int32> checkAndRemoveNeighbour = (Int32 pos) => { if (!_map.IsEmpty(pos) && _map.IsOccluded(pos)) deleteList.Add(new Voxel(pos, _map.Get(pos))); };
                checkAndRemoveNeighbour(vox.Key - 1);
                checkAndRemoveNeighbour(vox.Key + 1);
                checkAndRemoveNeighbour(vox.Key - _map.SizeX);
                checkAndRemoveNeighbour(vox.Key + _map.SizeX);
                checkAndRemoveNeighbour(vox.Key - _map.SizeX * _map.SizeY);
                checkAndRemoveNeighbour(vox.Key + _map.SizeX * _map.SizeY);
            }
            updateInstanceData(deleteList, insertionList);
        }


        /// <summary>
        /// Simulates one step of the automaton.
        /// </summary>
        /// <param name="updateInstanceData">A function which takes an incremental
        /// update for all changed voxels. The first param </param>
        public void Tick(ref Action<IEnumerable<Voxel>, IEnumerable<Voxel>> updateInstanceData, out int newBiomass, out int newParasites)
        {
            // There is just one map but nobody should write before all have
            // seen the current state -> collect all results first.
            ConcurrentDictionary<Int32, VoxelInfo> results = new ConcurrentDictionary<Int32, VoxelInfo>();
            //Parallel.ForEach(_livingVoxels, currentVoxel =>
            foreach( KeyValuePair<Int32, LivingVoxel> currentVoxel in _livingVoxels )
                {
                    ++currentVoxel.Value.Ticks;
                    // Create a local window for the rule algorithms
                    VoxelInfo[,,] localFrame = new VoxelInfo[3, 3, 3];
                    IterateNeighbours( (x, y, z) =>
                    {
                        Int32 pos = _map.EncodePosition(currentVoxel.Value.X + x - 1, currentVoxel.Value.Y + y - 1, currentVoxel.Value.Z + z - 1);
                        VoxelType voxel = _map.Get(pos);
                        // Living?
                        if (_map.IsLiving(pos))
                        {
                            // Yes: query more information from the dictinary
                            LivingVoxel voxelInfo = _livingVoxels[pos];
                            localFrame[z, y, x] = new VoxelInfo(voxel, true, voxelInfo.Generation, voxelInfo.Resources, voxelInfo.Ticks);
                        } else
                        {
                            // No uses directly
                            localFrame[z, y, x] = new VoxelInfo((VoxelType)voxel);
                        }
                    });
                    // Apply the rule for currentVoxel
                    VoxelInfo[, ,] ruleResult = TypeInformation.GetRule(localFrame[1,1,1].Type).ApplyRule(localFrame);
                    if (ruleResult != null)
                    {
                        currentVoxel.Value.Ticks = 0;
                        // Add changes to the change collection
                        IterateNeighbours((x, y, z) =>
                        {
                            if (ruleResult[z, y, x] != null && _map.IsInside(currentVoxel.Value.X + x - 1, currentVoxel.Value.Y + y - 1, currentVoxel.Value.Z + z - 1))
                            {
                                Int32 positionCode = _map.EncodePosition(currentVoxel.Value.X + x - 1, currentVoxel.Value.Y + y - 1, currentVoxel.Value.Z + z - 1);
                                // Two rules tried to grow at the same location. Use decision function
                                results.AddOrUpdate(positionCode, ruleResult[z, y, x], (key, old) => GamePlayUtils.GetStrongerVoxel(ref old, ref ruleResult[z, y, x]));
                            }
                        });
                    }
                }
            //);
            update(ref results, ref updateInstanceData, out newBiomass, out newParasites);
        }
    }
}
