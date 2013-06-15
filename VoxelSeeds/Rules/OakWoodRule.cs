using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    class OakWoodRule : IVoxelRule
    {
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            // Apply each 18-th turn
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(VoxelType.OAK_WOOD)) return null;

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            int res = neighbourhood[1, 1, 1].Resources;
            if (gen == 0)
            {
                // Grow Cross and ovveride current generation
                output[1, 1, 1] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1, 0, TypeInformation.GetGrowingSteps(VoxelType.OAK_WOOD) / 2);
                output[2, 1, 1] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1, 10);
                output[0, 1, 1] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1, 10);
                output[1, 1, 2] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1, 10);
                output[1, 1, 0] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1, 10);
            }
            else if (gen == 1 && res == 10)
            {
                // Grow Cross and ovveride current generation
                output[1, 1, 1] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1, 0, TypeInformation.GetGrowingSteps(VoxelType.OAK_WOOD) / 2);
                if (neighbourhood[2, 1, 1].Type == VoxelType.EMPTY)
                    output[2, 1, 1] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1, 20);
                if (neighbourhood[0, 1, 1].Type == VoxelType.EMPTY)
                    output[0, 1, 1] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1, 20);
                if (neighbourhood[1, 1, 2].Type == VoxelType.EMPTY)
                    output[1, 1, 2] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1, 20);
                if (neighbourhood[1, 1, 0].Type == VoxelType.EMPTY)
                    output[1, 1, 0] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1, 20);
            }
            else if (gen == 1 && res == 20)
            {
                output[1, 1, 1] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1, 0, TypeInformation.GetGrowingSteps(VoxelType.OAK_WOOD) / 4);
                if (neighbourhood[1, 1, 0].Type != VoxelType.OAK_WOOD && neighbourhood[1, 1, 2].Type != VoxelType.OAK_WOOD)
                {
                    if (neighbourhood[1, 1, 2].Type == VoxelType.EMPTY)
                        output[1, 1, 2] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1);
                    if (neighbourhood[1, 1, 0].Type == VoxelType.EMPTY)
                        output[1, 1, 0] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1);
                }
                else if (neighbourhood[0, 1, 1].Type != VoxelType.OAK_WOOD && neighbourhood[2, 1, 1].Type != VoxelType.OAK_WOOD)
                {
                    if (neighbourhood[2, 1, 1].Type == VoxelType.EMPTY)
                        output[2, 1, 1] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1);
                    if (neighbourhood[0, 1, 1].Type == VoxelType.EMPTY)
                        output[0, 1, 1] = new VoxelInfo(VoxelType.OAK_WOOD, true, 1);
                }
            }
            else if (gen < TypeInformation.GetGrowHeight(VoxelType.OAK_WOOD))
            {
                // Grow upwards
                output[1, 2, 1] = new VoxelInfo(VoxelType.OAK_WOOD, true, gen + 1);
                output[1, 1, 1] = new VoxelInfo(VoxelType.OAK_WOOD);
            }
            else
            {
                // Grow upwards one last time
                output[1, 2, 1] = new VoxelInfo(VoxelType.OAK_WOOD);
                output[1, 1, 1] = new VoxelInfo(VoxelType.OAK_WOOD);
            }
            return output;
        }
    }
}
