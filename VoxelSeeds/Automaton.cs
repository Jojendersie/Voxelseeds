using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace VoxelSeeds
{
    class Automaton
    {
        struct LivingVoxel
        {
            public int X;
            public int Y;
            public int Z;

            public int Generation;
            public int Resources;

            public IVoxelRule Rule;
        };

        Automaton()
        {
            _livingVoxels = new Dictionary<Int32,LivingVoxel>();
        }


        // All existing voxels in the current time step.
        Map _map;

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

        private void update(ref ConcurrentDictionary<Int32, VoxelInfo> results, ref Action<Voxel[],Voxel[]> updateInstanceData)
        {
            List<Voxel> deleteList = new List<Voxel>();
            List<Voxel> insertionList = new List<Voxel>();
            foreach( KeyValuePair<Int32, VoxelInfo> vox in results )
            {
                //_map.Sample(
            }
        }

        /// <summary>
        /// Simulates one step of the automaton.
        /// </summary>
        /// <param name="updateInstanceData">A function which takes an incremental
        /// update for all changed voxels. The first param </param>
        public void Tick(Action<Voxel[],Voxel[]> updateInstanceData)
        {
            // There is just one map but nobody should write before all have
            // seen the current state -> collect all results first.
            ConcurrentDictionary<Int32, VoxelInfo> results = new ConcurrentDictionary<Int32, VoxelInfo>();
            Parallel.ForEach(_livingVoxels, currentVoxel =>
                {
                    // Create a local window for the rule algorithms
                    VoxelInfo[,,] localFrame = new VoxelInfo[3, 3, 3];
                    IterateNeighbours( (x, y, z) =>
                    {
                        byte voxel = _map.Sample(currentVoxel.Value.X + x - 1, currentVoxel.Value.Y + y - 1, currentVoxel.Value.Z + z - 1);
                        // Living?
                        if ((0x80 & voxel) == 0x80)
                        {
                            // Yes: query more information from the dictinary
                            LivingVoxel VoxelInfo = _livingVoxels[_map.EncodePosition( currentVoxel.Value.X, currentVoxel.Value.Y, currentVoxel.Value.Z)];
                            localFrame[z, y, x] = new VoxelInfo((VoxelType)(voxel & 0x7f), true, VoxelInfo.Resources, VoxelInfo.Generation );
                        }
                        {
                            // No uses directly
                            localFrame[z, y, x] = new VoxelInfo((VoxelType)voxel, false);
                        }
                    });
                    // Apply the rule for currentVoxel
                    VoxelInfo[,,] ruleResult = currentVoxel.Value.Rule.ApplyRule( localFrame );
                    if( ruleResult != null )
                    {
                        // Add changes to the change collection
                        IterateNeighbours((x, y, z) =>
                        {
                            if (ruleResult[z, y, x] == null)
                            {
                                Int32 positionCode = _map.EncodePosition(currentVoxel.Value.X + x - 1, currentVoxel.Value.Y + y - 1, currentVoxel.Value.Z + z - 1);
                                // Two rules tried to grow at the same location. Use decision function
                                results.AddOrUpdate(positionCode, ruleResult[z, y, x], (key, old) => GamePlayUtils.GetStrongerVoxel(ref old, ref ruleResult[z, y, x]));
                            }
                        });
                     }
                }
            );
            update(ref results, ref updateInstanceData);
        }
    }
}
