using SharpDX;
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
            int height = Random.Next((int)(TypeInformation.GetGrowHeight(VoxelType.SPRUCE_WOOD) * 0.95), (int)(TypeInformation.GetGrowHeight(VoxelType.SPRUCE_WOOD) * 1.45));
            if (gen == 0)
            {
                // Grow 3x3 and override current generation
                output[1, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 1, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD) / 2);
                output[2, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD) / 4);
                output[0, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD) / 4);
                output[1, 1, 2] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD) / 4);
                output[1, 1, 0] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD) / 4);
                /*output[2, 1, 2] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2);
                output[2, 1, 0] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2);
                output[0, 1, 2] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2);
                output[0, 1, 0] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2);*/

                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[2, 0, 1].Type) || neighbourhood[2, 0, 1].Type == VoxelType.EMPTY)
                    output[2, 0, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD) / 4,Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[0, 0, 1].Type) || neighbourhood[0, 0, 1].Type == VoxelType.EMPTY)
                    output[0, 0, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD) / 4, Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[1, 0, 2].Type) || neighbourhood[1, 0, 2].Type == VoxelType.EMPTY)
                    output[1, 0, 2] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD) / 4, Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[1, 0, 0].Type) || neighbourhood[1, 0, 0].Type == VoxelType.EMPTY)
                    output[1, 0, 0] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2, 0, TypeInformation.GetGrowingSteps(VoxelType.SPRUCE_WOOD) / 4, Direction.UP);
                /*if (TypeInformation.IsNotWoodButBiomass(neighbourhood[2, 0, 2].Type) || neighbourhood[2, 0, 2].Type == VoxelType.EMPTY)
                    output[2, 0, 2] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2, 0,0,Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[2, 0, 0].Type) || neighbourhood[2, 0, 0].Type == VoxelType.EMPTY) 
                    output[2, 0, 0] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2,0,0,Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[0, 0, 2].Type) || neighbourhood[0, 0, 2].Type == VoxelType.EMPTY)
                    output[0, 0, 2] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2, 0, 0, Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[0, 0, 0].Type) || neighbourhood[0, 0, 0].Type == VoxelType.EMPTY)
                    output[0, 0, 0] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, 2, 0, 0, Direction.UP);*/
            }
            else if (gen < height)
            {
                // Grow upwards
                // Grow in given direction
                Int3 coord = DirectionConverter.FromDirection(DirectionConverter.ToOppositeDirection(neighbourhood[1, 1, 1].From));
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[coord.X, coord.Y, coord.Z].Type) || neighbourhood[coord.X, coord.Y, coord.Z].Type == VoxelType.EMPTY)
                {
                    output[coord.X, coord.Y, coord.Z] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, gen + 1, 0, 0, neighbourhood[1, 1, 1].From);
                }
                //output[1, 2, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD, true, gen + 1);
                output[1, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD);
            }
            else
            {
                // Grow upwards one last time
                //output[1, 2, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD);
                if (neighbourhood[1, 1, 1].From != Direction.UP)
                {
                    output[1, 2, 1] = new VoxelInfo(VoxelType.SPRUCE_NEEDLE, true);
                }
                output[1, 1, 1] = new VoxelInfo(VoxelType.SPRUCE_WOOD);
            }
            return output;
        }
    }
}
