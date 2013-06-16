using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    class TeakLeafRule : IVoxelRule
    {
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            // Apply each 18-th turn
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(VoxelType.TEAK_LEAF)) return null;

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            if (gen == 0)
            {
                output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_LEAF);
                if (neighbourhood[2, 1, 1].Type == VoxelType.EMPTY)
                    output[2, 1, 1] = new VoxelInfo(VoxelType.TEAK_LEAF, true, 1);
                if (neighbourhood[0, 1, 1].Type == VoxelType.EMPTY)
                    output[0, 1, 1] = new VoxelInfo(VoxelType.TEAK_LEAF, true, 1);
                if (neighbourhood[1, 1, 2].Type == VoxelType.EMPTY)
                    output[1, 1, 2] = new VoxelInfo(VoxelType.TEAK_LEAF, true, 1);
                if (neighbourhood[1, 1, 0].Type == VoxelType.EMPTY)
                    output[1, 1, 0] = new VoxelInfo(VoxelType.TEAK_LEAF, true, 1);
            }
            else if (gen < TypeInformation.GetGrowHeight(VoxelType.TEAK_LEAF))
            {
                // Grow to the side
                //output[1, 2, 1] = new VoxelInfo(VoxelType.TEAK_WOOD, true, gen + 1);
                //output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_WOOD);
                output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_LEAF);
                if (neighbourhood[2, 1, 1].Type == VoxelType.EMPTY)
                    output[2, 1, 1] = new VoxelInfo(VoxelType.TEAK_LEAF, true, gen+1);
                if (neighbourhood[0, 1, 1].Type == VoxelType.EMPTY)
                    output[0, 1, 1] = new VoxelInfo(VoxelType.TEAK_LEAF, true, gen+1);
                if (neighbourhood[1, 1, 2].Type == VoxelType.EMPTY)
                    output[1, 1, 2] = new VoxelInfo(VoxelType.TEAK_LEAF, true, gen+1);
                if (neighbourhood[1, 1, 0].Type == VoxelType.EMPTY)
                    output[1, 1, 0] = new VoxelInfo(VoxelType.TEAK_LEAF, true, gen+1);
            }
            else
            {
                // Grow upwards one last time
                //output[1, 2, 1] = new VoxelInfo(VoxelType.TEAK_WOOD);
                output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_LEAF);
            }
            return output;
        }
    }
}
