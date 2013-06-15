using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    class BeechWoodRule : IVoxelRule
    {
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            // Apply each 18-th turn
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(VoxelType.BEECH_WOOD)) return null;

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            if (gen == 0)
            {
                // Grow 2x2 and ovveride current generation
                output[1, 1, 1] = new VoxelInfo(VoxelType.BEECH_WOOD, true, 1, 10);
                output[2, 1, 1] = new VoxelInfo(VoxelType.BEECH_WOOD, true, 1, 10);
                output[1, 1, 2] = new VoxelInfo(VoxelType.BEECH_WOOD, true, 1, 10);
                output[2, 1, 2] = new VoxelInfo(VoxelType.BEECH_WOOD, true, 1, 10);
            }
            else if (gen == 1 && neighbourhood[1, 1, 1].Resources == 10)
            {
                // Grow Cross and ovveride current generation
                output[1, 1, 1] = new VoxelInfo(VoxelType.BEECH_WOOD, true, 1, 0, TypeInformation.GetGrowingSteps(VoxelType.PINE_WOOD) / 2);
                if (neighbourhood[2, 1, 1].Type == VoxelType.EMPTY)
                    output[2, 1, 1] = new VoxelInfo(VoxelType.BEECH_WOOD, true, 1);
                if (neighbourhood[0, 1, 1].Type == VoxelType.EMPTY)
                    output[0, 1, 1] = new VoxelInfo(VoxelType.BEECH_WOOD, true, 1);
                if (neighbourhood[1, 1, 2].Type == VoxelType.EMPTY)
                    output[1, 1, 2] = new VoxelInfo(VoxelType.BEECH_WOOD, true, 1);
                if (neighbourhood[1, 1, 0].Type == VoxelType.EMPTY)
                    output[1, 1, 0] = new VoxelInfo(VoxelType.BEECH_WOOD, true, 1);
            }
            else if(gen < TypeInformation.GetGrowHeight(VoxelType.BEECH_WOOD))
            {
                // Grow upwards
                output[1, 2, 1] = new VoxelInfo(VoxelType.BEECH_WOOD, true, gen + 1);
                output[1, 1, 1] = new VoxelInfo(VoxelType.BEECH_WOOD);
            }
            else
            {
                // Grow upwards one last time
                output[1, 2, 1] = new VoxelInfo(VoxelType.BEECH_WOOD);
                output[1, 1, 1] = new VoxelInfo(VoxelType.BEECH_WOOD);
            }
            return output;
        }
    }
}
