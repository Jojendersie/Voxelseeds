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
            public LivingVoxel(VoxelType type, int x, int y, int z, int generation, int resources, int ticks, Direction from)
            {
                X = x;
                Y = y;
                Z = z;
                Generation = generation;
                Resources = resources;
                Ticks = ticks;
                From = from;
                Type = type;
            }

            public int X;
            public int Y;
            public int Z;

            public VoxelType Type;  // Duplicate information to make some things easier
            public int Generation;
            public int Resources;

            public int Ticks;

            public Direction From;
        };



        // All existing voxels in the current time step.
        Map _map;
        public Map Map { get { return _map; } }

        // Extra data for all living voxels.
        Dictionary<Int32, LivingVoxel> _livingVoxels;
        int _numLivingBiomass;
        int _numLivingParasites;

        // Voxelbuffer for things added with InsertSeed (asynchronous tasks)
        System.Collections.Queue _pendingVoxels;

        // Buffer for changes which are not uploaded to the GPU.
        HashSet<Voxel> _deleteList;
        HashSet<Voxel> _insertionList;

        public int NumLivingParasites { get { return _numLivingParasites; } }
        public int NumLivingBiomass { get { return _numLivingBiomass; } }

        // Method for incremental update of GPU resources
        Action<IEnumerable<Voxel>, IEnumerable<Voxel>> _updateInstanceData;

        public Automaton(int sizeX, int sizeY, int sizeZ, LevelType lvlType, int seed, float heightoffset)
        {
            _map = new Map(sizeX, sizeY, sizeZ, lvlType, seed, heightoffset);
            _livingVoxels = new Dictionary<Int32,LivingVoxel>();
            _simTask = null;

            _deleteList = new HashSet<Voxel>();
            _insertionList = new HashSet<Voxel>();

            _pendingVoxels = new System.Collections.Queue();
        }

        /// <param name="updateInstanceData">A function which takes an incremental
        /// update for all changed voxels. The first param </param>
        public void SetInstanceUpdateMethod( ref Action<IEnumerable<Voxel>, IEnumerable<Voxel>> updateInstanceData )
        {
            _updateInstanceData = updateInstanceData;
        }

        public void Upload()
        {
            if (_updateInstanceData != null )
            {
                _updateInstanceData(_deleteList, _insertionList);
            }
            _deleteList.Clear();
            _insertionList.Clear();
        }

        /// <summary>
        /// Add to map and to living list. Does nothing if tries to set outside.
        /// </summary>
        private void InsertVoxel(LivingVoxel voxel, bool living)
        {
            int pos = _map.EncodePosition(voxel.X, voxel.Y, voxel.Z);
            if (_map.IsInside(voxel.X, voxel.Y, voxel.Z))
            {
                if (_map.Get(pos) != VoxelType.EMPTY)
                    RemoveVoxel(pos);

                _map.Set(pos, voxel.Type, living);
                if (voxel.Type != VoxelType.EMPTY)
                {
                    // Insert to instance data only if visible
                    if (!_map.IsOccluded(pos))
                        _insertionList.Add(new Voxel(pos, voxel.Type));
                    RemoveOccludedNeighbours(pos);
                }
                if (living)
                {
                    Debug.Assert(!_livingVoxels.ContainsKey(pos));
                    _livingVoxels.Add(pos, voxel);
                    if (TypeInformation.IsBiomass(voxel.Type)) ++_numLivingBiomass;
                    else if (TypeInformation.IsParasite(voxel.Type)) ++_numLivingParasites;
                }
            }
        }

        /// <summary>
        /// Remove from map and from living list
        /// </summary>
        /// <param name="positionCode"></param>
        private void RemoveVoxel(Int32 positionCode)
        {
            // Static part
            var pos = _map.DecodePosition(positionCode);
            VoxelType old = _map.Get(positionCode);

            _map.Set(positionCode, VoxelType.EMPTY, false);

            // Grafic part
            ReinsertVisibleNeighbours(positionCode);
            Voxel v = new Voxel(positionCode, old);
            _deleteList.Add(v);
            _insertionList.Remove(v);

            // Dynamic part
            if (_livingVoxels.ContainsKey(positionCode))
            {
                _livingVoxels.Remove(positionCode);
                if (TypeInformation.IsBiomass(old)) --_numLivingBiomass;
                else if (TypeInformation.IsParasite(old)) --_numLivingParasites;
                Debug.Assert(_numLivingBiomass >= 0);
                Debug.Assert(_numLivingParasites >= 0);
            }
        }

        private void RemoveOccludedNeighbours(Int32 positionCode)
        {
            // The map set can cause neighboured voxels to be occluded
            // if so delete that from GPU.
            Action<Int32> checkAndRemoveNeighbour = (Int32 pos) => { if (!_map.IsEmpty(pos) && _map.IsOccluded(pos)) { Voxel v = new Voxel(pos, _map.Get(pos)); _deleteList.Add(v); _insertionList.Remove(v); } };
            checkAndRemoveNeighbour(positionCode - 1);
            checkAndRemoveNeighbour(positionCode + 1);
            checkAndRemoveNeighbour(positionCode - _map.SizeX);
            checkAndRemoveNeighbour(positionCode + _map.SizeX);
            checkAndRemoveNeighbour(positionCode - _map.SizeX * _map.SizeY);
            checkAndRemoveNeighbour(positionCode + _map.SizeX * _map.SizeY);
        }

        private void ReinsertVisibleNeighbours(Int32 positionCode)
        {
            // If a voxel should be deleted insert all currently invisible
            // neighbours first. They are visible afterwards
            Action<Int32> checkAndAddNeighbour = (Int32 pos) => { if (!_map.IsEmpty(pos) && !_map.IsOccluded(pos)) _insertionList.Add(new Voxel(pos, _map.Get(pos))); };
            checkAndAddNeighbour(positionCode - 1);
            checkAndAddNeighbour(positionCode + 1);
            checkAndAddNeighbour(positionCode - _map.SizeX);
            checkAndAddNeighbour(positionCode + _map.SizeX);
            checkAndAddNeighbour(positionCode - _map.SizeX * _map.SizeY);
            checkAndAddNeighbour(positionCode + _map.SizeX * _map.SizeY);
        }

        /// <summary>
        /// Inserts a new voxel and updates the instance buffers.
        /// </summary>
        public void InsertSeed(int x, int y, int z, VoxelType type, Direction from = Direction.DOWN)
        {
            _pendingVoxels.Enqueue( new LivingVoxel(type, x, y, z, 0, 0, 0, from ) );

           // InsertVoxel(pos, type, 0, true, 0, 0, from);

            // Immediate upload (assuming ~1 seed per frame)
           // Upload();
        }



        private void IterateNeighbours(Action<int, int, int> func )
        {
            for (int z = 0; z < 3; ++z)
                for (int y = 0; y < 3; ++y)
                    for (int x = 0; x < 3; ++x)
                        func(x, y, z);
        }

        private int[] update(ref ConcurrentDictionary<Int32, VoxelInfo> results)
        {
            int[] numWrittenVoxels = new int[TypeInformation.GetNumTypes()];
            // Apply results from simulation
            foreach( KeyValuePair<Int32, VoxelInfo> vox in results )
            {
                var po = _map.DecodePosition(vox.Key);
                Debug.Assert( _map.IsInside(po.X, po.Y, po.Z) );

                InsertVoxel(new LivingVoxel(vox.Value.Type, po.X, po.Y, po.Z, vox.Value.Generation, vox.Value.Resources, vox.Value.Ticks, vox.Value.From), vox.Value.Living);
                ++numWrittenVoxels[(int)vox.Value.Type];
            }
            // User actions
            while (_pendingVoxels.Count > 0)
                InsertVoxel((LivingVoxel)_pendingVoxels.Dequeue(), true);

            // To GPU
            Upload();
            return numWrittenVoxels;
        }

        /// <summary>
        /// The core of the simulation. This function applies all rules to all
        /// living voxels.
        /// </summary>
        /// <returns>Every change which have to be made to apply the latest changes.</returns>
        private ConcurrentDictionary<Int32, VoxelInfo> SimulateAsync()
        {
            // There is just one map but nobody should write before all have
            // seen the current state -> collect all results first.
            ConcurrentDictionary<Int32, VoxelInfo> results = new ConcurrentDictionary<Int32, VoxelInfo>();
            //Parallel.ForEach(_livingVoxels, currentVoxel =>
            foreach( KeyValuePair<Int32, LivingVoxel> currentVoxel in _livingVoxels )
            {
                ++currentVoxel.Value.Ticks;
                // Create a local window for the rule algorithms
                VoxelInfo[, ,] localFrame = new VoxelInfo[3, 3, 3];
                IterateNeighbours((x, y, z) =>
                {
                    Int32 pos = _map.EncodePosition(currentVoxel.Value.X + x - 1, currentVoxel.Value.Y + y - 1, currentVoxel.Value.Z + z - 1);
                    VoxelType voxel = _map.Get(pos);
                    // Living?
                    if (_map.IsLiving(pos))
                    {
                        // Yes: query more information from the dictinary
                        LivingVoxel voxelInfo = _livingVoxels[pos];
                        localFrame[z, y, x] = new VoxelInfo(voxel, true, voxelInfo.Generation, voxelInfo.Resources, voxelInfo.Ticks, voxelInfo.From);
                    }
                    else
                    {
                        // No uses directly
                        localFrame[z, y, x] = new VoxelInfo((VoxelType)voxel);
                    }
                });
                // Apply the rule for currentVoxel
                VoxelInfo[, ,] ruleResult = TypeInformation.GetRule(localFrame[1, 1, 1].Type).ApplyRule(localFrame);
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
            }//);

            return results;
        }

        Task<ConcurrentDictionary<Int32, VoxelInfo>> _simTask;

        /// <summary>
        /// Simulates on step of automaton in parallel.
        /// The results from the last tick are uploaded now.
        /// </summary>
        /// <returns>Number of new written voxels. This is not necessarily the
        /// amount of new mass because they could have overwritten some old.</returns>
        public int[] Tick()
        {
            /* synchronous
            int[] numWrittenVoxels = null;
            ConcurrentDictionary<Int32, VoxelInfo> results = SimulateAsync();
            numWrittenVoxels = update(ref results);//*/
            

            int[] numWrittenVoxels = null;
            if (_simTask != null)
            {
                try
                {
                    _simTask.Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                ConcurrentDictionary<Int32, VoxelInfo> results = _simTask.Result;
                // Do a synchronus update
                numWrittenVoxels = update(ref results);
            }
            _simTask = new Task<ConcurrentDictionary<int, VoxelInfo>>(() => SimulateAsync());
            // Start next turn
            _simTask.Start();//*/
            return numWrittenVoxels;
        }
    }
}
