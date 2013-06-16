using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    class SpruceNeedleRule : IVoxelRule
    {
        Random random = new Random();
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            // Apply each 18-th turn
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_NEEDLE)) return null;

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            int res = neighbourhood[1, 1, 1].Resources;
            if (gen == 0)
            {
                output[1, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[1, 2, 1].Type) || neighbourhood[1, 2, 1].Type == VoxelType.EMPTY)
                    output[1, 2, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, -1, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_NEEDLE)/2);
                if (CanPlace(2, 1, 1, neighbourhood))
                    output[2, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, 1);
                if (CanPlace(0, 1, 1, neighbourhood))
                    output[0, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, 1);
                if (CanPlace(1, 1, 2, neighbourhood))
                    output[1, 1, 2] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, 1);
                if (CanPlace(1, 1, 0, neighbourhood))
                    output[1, 1, 0] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, 1);
            }
            else if (gen == -1)
            {
                output[1, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[1, 2, 1].Type) || neighbourhood[1, 2, 1].Type == VoxelType.EMPTY)
                    output[1, 2, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE);
            }
            else if (gen < TypeInformation.GetGrowHeight(VoxelType.SPRUCE_NEEDLE) && gen % 3 == 1)
            {
                // Grow to the side
                //output[1, 2, 1] = new VoxelInfo(VoxelType.TEAK_WOOD, true, gen + 1);
                //output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_WOOD);
                output[1, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE);
                if (CanPlace(2, 0, 2, neighbourhood))
                    output[2, 0, 2] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + 1, 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
                if (CanPlace(1, 0, 2, neighbourhood))
                    output[1, 0, 2] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + 1, 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
                if (CanPlace(0, 0, 2, neighbourhood))
                    output[0, 0, 2] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + 1, 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
                if (CanPlace(0, 0, 1, neighbourhood))
                    output[0, 0, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + 1, 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
                if (CanPlace(1, 0, 1, neighbourhood))
                    output[1, 0, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + 1, 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
                if (CanPlace(2, 0, 1, neighbourhood))
                    output[2, 0, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + 1, 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
                if (CanPlace(1, 0, 0, neighbourhood))
                    output[1, 0, 0] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + 1, 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
                if (CanPlace(2, 0, 0, neighbourhood))
                    output[2, 0, 0] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + 1, 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
            }
            else if (gen < TypeInformation.GetGrowHeight(VoxelType.SPRUCE_NEEDLE))
            {
                // Grow to the side
                //output[1, 2, 1] = new VoxelInfo(VoxelType.TEAK_WOOD, true, gen + 1);
                //output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_WOOD);
                output[1, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE);
                if (CanPlace(1, 0, 1, neighbourhood))
                    output[1, 0, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + 1, 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
                /*if (CanPlace(1, 0, 2, neighbourhood))
                    output[1, 0, 2] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + random.Next(1, 3), 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
                if (CanPlace(0, 0, 2, neighbourhood))
                    output[0, 0, 2] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + random.Next(1, 3), 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
                if (CanPlace(0, 0, 1, neighbourhood))
                    output[0, 0, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + random.Next(1, 3), 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
                if (CanPlace(1, 0, 1, neighbourhood))
                    output[1, 0, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + random.Next(1, 3), 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
                if (CanPlace(2, 0, 1, neighbourhood))
                    output[2, 0, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + random.Next(1, 3), 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
                if (CanPlace(1, 0, 0, neighbourhood))
                    output[1, 0, 0] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + random.Next(1, 3), 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));
                if (CanPlace(2, 0, 0, neighbourhood))
                    output[2, 0, 0] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true, gen + random.Next(1, 3), 0, random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.PINE_NEEDLE)));*/
            }
            else
            {
                // Grow upwards one last time
                //output[1, 2, 1] = new VoxelInfo(VoxelType.TEAK_WOOD);
                output[1, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE);
            }
            return output;
        }

        bool CanPlace(int t, int h, int b, VoxelInfo[, ,] neighbourhood)
        {
            bool res = neighbourhood[t, h, b].Type == VoxelType.EMPTY;
            //if (h < 2) res = res && neighbourhood[t, h + 1, b].Type != VoxelType.SPRUCE_NEEDLE;
            //if (h > 0) res = res && neighbourhood[t, h - 1, b].Type != VoxelType.SPRUCE_NEEDLE;
            return res;// && neighbourhood[1, 1, 1].Resources < 51;
        }
    }
}
