using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    class TeakWoodRule : IVoxelRule
    {
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            // TODO: only iff ticks == 18
            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            if (gen == 0)
            {
                output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_WOOD, true, 1);
                output[2, 1, 1] = new VoxelInfo(VoxelType.TEAK_WOOD, true, 1);
                output[1, 1, 2] = new VoxelInfo(VoxelType.TEAK_WOOD, true, 1);
                output[2, 1, 2] = new VoxelInfo(VoxelType.TEAK_WOOD, true, 1);
            }
            else if (gen < 7)
            {
                output[1, 2, 1] = new VoxelInfo(VoxelType.TEAK_WOOD, true, gen + 1);
                output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_WOOD);
            }
            else
            {
                output[1, 2, 1] = new VoxelInfo(VoxelType.TEAK_WOOD);
                output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_WOOD);
            }
            return output;
        }
    }
}
