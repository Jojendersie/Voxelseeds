using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    class TeakWoodRule : IVoxelRule
    {
        Random random = new Random();
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            // Apply each 18-th turn
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(VoxelType.TEAK_WOOD)) return null;

            int height = random.Next((int)(TypeInformation.GetGrowHeight(VoxelType.TEAK_WOOD) *0.75), (int)(TypeInformation.GetGrowHeight(VoxelType.TEAK_WOOD) * 1.25));

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            if (gen == 0)
            {
                // Grow a 2x2 quad (override current generation)
                output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_WOOD, true, 1);
                output[2, 1, 1] = new VoxelInfo(VoxelType.TEAK_WOOD, true, 1);
                output[1, 1, 2] = new VoxelInfo(VoxelType.TEAK_WOOD, true, 1);
                output[2, 1, 2] = new VoxelInfo(VoxelType.TEAK_WOOD, true, 1);

                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[1, 0, 1].Type) || neighbourhood[1, 0, 1].Type == VoxelType.EMPTY)
                    output[1, 0, 1] = new VoxelInfo(VoxelType.TEAK_WOOD, true, 1, 0, 0, Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[2, 0, 1].Type) || neighbourhood[2, 0, 1].Type == VoxelType.EMPTY)
                    output[2, 0, 1] = new VoxelInfo(VoxelType.TEAK_WOOD, true, 1, 0, 0, Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[1, 0, 2].Type) || neighbourhood[1, 0, 2].Type == VoxelType.EMPTY)
                    output[1, 0, 2] = new VoxelInfo(VoxelType.TEAK_WOOD, true, 1, 0, 0, Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[2, 0, 2].Type) || neighbourhood[2, 0, 2].Type == VoxelType.EMPTY)
                    output[2, 0, 2] = new VoxelInfo(VoxelType.TEAK_WOOD, true, 1, 0, 0, Direction.UP);
            }
            else if (gen < height)
            {
                Int3 coord = DirectionConverter.FromDirection(DirectionConverter.ToOppositeDirection(neighbourhood[1, 1, 1].From));
                // Grow in given direction
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[coord.X, coord.Y, coord.Z].Type) || neighbourhood[coord.X, coord.Y, coord.Z].Type == VoxelType.EMPTY)
                {
                    output[coord.X, coord.Y, coord.Z] = new VoxelInfo(VoxelType.TEAK_WOOD, true, gen + 1,0,0,neighbourhood[1,1,1].From);
                }
                output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_WOOD);
            }
            else
            {
                if (neighbourhood[1, 1, 1].From == Direction.DOWN)
                {
                    if (TypeInformation.IsNotWoodButBiomass(neighbourhood[1, 2, 1].Type) || neighbourhood[1, 2, 1].Type == VoxelType.EMPTY)
                    {
                        output[1, 2, 1] = new VoxelInfo(VoxelType.TEAK_LEAF, true, 1);
                    }
                }
                output[1, 1, 1] = new VoxelInfo(VoxelType.TEAK_WOOD);
            }
            return output;
        }
    }
}
