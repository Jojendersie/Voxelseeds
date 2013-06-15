using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    class SpruceWoodRule : IVoxelRule
    {
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            // Apply each 18-th turn
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD)) return null;

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            if (gen == 0)
            {
                // Grow 3x3 and ovveride current generation
                output[1, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 1, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD) / 2);
                output[2, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 1, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD) / 4);
                output[0, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 1, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD) / 4);
                output[1, 1, 2] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 1, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD) / 4);
                output[1, 1, 0] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 1, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD) / 4);
                output[2, 1, 2] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 1);
                output[2, 1, 0] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 1);
                output[0, 1, 2] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 1);
                output[0, 1, 0] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 1);
            }
            else if (gen < 9)
            {
                // Grow upwards
                output[1, 2, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, gen + 1);
                output[1, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD);
            }
            else
            {
                // Grow upwards one last time
                output[1, 2, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD);
                output[1, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD);
            }
            return output;
        }
    }
}
