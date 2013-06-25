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
            int res = neighbourhood[1,1,1].Resources;
            if (gen == 0)
            {
                output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_LEAF);
                if (CanPlace(2,1,1,neighbourhood))
                    output[2, 1, 1] = new VoxelInfo(VoxelType.TEAK_LEAF, true, 1);
                if (CanPlace(0, 1, 1, neighbourhood))
                    output[0, 1, 1] = new VoxelInfo(VoxelType.TEAK_LEAF, true, 1);
                if (CanPlace(1, 1, 2, neighbourhood))
                    output[1, 1, 2] = new VoxelInfo(VoxelType.TEAK_LEAF, true, 1);
                if (CanPlace(1, 1, 0, neighbourhood))
                    output[1, 1, 0] = new VoxelInfo(VoxelType.TEAK_LEAF, true, 1);
            }
            else if (gen < TypeInformation.GetGrowHeight(VoxelType.TEAK_LEAF))
            {
                // Grow to the side
                //output[1, 2, 1] = new VoxelInfo(VoxelType.TEAK_WOOD, true, gen + 1);
                //output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_WOOD);
                output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_LEAF);
                if (CanPlace(2, 1, 1, neighbourhood))
                    output[2, 1, 1] = new VoxelInfo(VoxelType.TEAK_LEAF, true, gen + Random.Next(1, 3), 0, Random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.TEAK_LEAF)));
                if (CanPlace(0, 1, 1, neighbourhood))
                    output[0, 1, 1] = new VoxelInfo(VoxelType.TEAK_LEAF, true, gen + Random.Next(1, 3), 0, Random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.TEAK_LEAF)));
                if (CanPlace(1, 1, 2, neighbourhood))
                    output[1, 1, 2] = new VoxelInfo(VoxelType.TEAK_LEAF, true, gen + Random.Next(1, 3), 0, Random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.TEAK_LEAF)));
                if (CanPlace(1, 1, 0, neighbourhood))
                    output[1, 1, 0] = new VoxelInfo(VoxelType.TEAK_LEAF, true, gen + Random.Next(1, 3), 0, Random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.TEAK_LEAF)));
            }
            else
            {
                // Stop growing
                output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_LEAF);
            }
            return output;
        }

        bool CanPlace(int t, int h, int b, VoxelInfo[, ,] neighbourhood)
        {
            bool res = neighbourhood[t, h, b].Type == VoxelType.EMPTY;
            if (h < 2) res &= neighbourhood[t, h + 1, b].Type != VoxelType.TEAK_LEAF;
            if (h > 0) res &= neighbourhood[t, h - 1, b].Type != VoxelType.TEAK_LEAF;
            return res;
        }
    }
}
