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


        // All existing voxels in the current time step.
        Map _map;

        // Extra data for all living voxels.
        // TODO: hashmap... ersparrt auch das living flag in der map
        Dictionary<int,LivingVoxel> _livingVoxels;

        // TODO: return infos für Andreas
        public void Tick()
        {
            // There is just one map but nobody should write before all have
            // seen the current state -> collect all results first.
            ConcurrentBag<VoxelInfo[,,]> results = new ConcurrentBag<VoxelInfo[,,]>();
            Parallel.ForEach(_livingVoxels, currentVoxel =>
                {
                    // Create a local window for the rule algorithms
                    VoxelInfo[,,] localFrame = new VoxelInfo[3, 3, 3];
                    for (int z = 0; z < 3; ++z)
                        for (int y = 0; y < 3; ++y)
                            for (int x = 0; x < 3; ++x)
                            {
                                byte voxel = _map.Sample(currentVoxel.Value.X + x - 1, currentVoxel.Value.Y + y - 1, currentVoxel.Value.Z + z - 1);
                                // Living?
                                if ((0x80 & voxel) == 0x80)
                                {
                                    // Yes: query more information from the dictinary
                                    LivingVoxel VoxelInfo = _livingVoxels[currentVoxel.Value.X + _map.SizeX * (currentVoxel.Value.Y + _map.SizeY * currentVoxel.Value.Z)];
                                    localFrame[z, y, x] = new VoxelInfo((VoxelType)(voxel & 0x7f), true, VoxelInfo.Resources, VoxelInfo.Generation );
                                }
                                {
                                    // No uses directly
                                    localFrame[z, y, x] = new VoxelInfo((VoxelType)voxel, false);
                                }
                            }
                    results.Add(currentVoxel.Value.Rule.ApplyRule( localFrame ));
                }
            );
        }
    }
}
