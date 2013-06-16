using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    class PineWoodRule : IVoxelRule
    {
        Random random = new Random();
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            // Apply each 18-th turn
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(VoxelType.PINE_WOOD)) return null;

            int height = random.Next((int)(TypeInformation.GetGrowHeight(VoxelType.PINE_WOOD) * 0.75), (int)(TypeInformation.GetGrowHeight(VoxelType.PINE_WOOD) * 1.25));

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            int res = neighbourhood[1, 1, 1].Resources;
            if (gen == 0)
            {
                // Grow Cross and ovveride current generation
                output[1, 1, 1] = new VoxelInfo(VoxelType.PINE_WOOD, true, 1, 0, TypeInformation.GetGrowingSteps(VoxelType.PINE_WOOD) / 2);
                output[2, 1, 1] = new VoxelInfo(VoxelType.PINE_WOOD, true, 2, 30);
                output[0, 1, 1] = new VoxelInfo(VoxelType.PINE_WOOD, true, 2, 30);
                output[1, 1, 2] = new VoxelInfo(VoxelType.PINE_WOOD, true, 2, 30);
                output[1, 1, 0] = new VoxelInfo(VoxelType.PINE_WOOD, true, 2, 30);

                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[1, 0, 1].Type) || neighbourhood[1, 0, 1].Type == VoxelType.EMPTY)
                    output[1, 0, 1] = new VoxelInfo(VoxelType.PINE_WOOD, true, random.Next(1,height), 0, 0, Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[2, 0, 1].Type) || neighbourhood[2, 0, 1].Type == VoxelType.EMPTY)
                    output[2, 0, 1] = new VoxelInfo(VoxelType.PINE_WOOD, true, random.Next(1, height), 0, 0, Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[0, 0, 1].Type) || neighbourhood[0, 0, 1].Type == VoxelType.EMPTY)
                    output[0, 0, 1] = new VoxelInfo(VoxelType.PINE_WOOD, true, random.Next(1, height), 0, 0, Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[1, 0, 2].Type) || neighbourhood[1, 0, 2].Type == VoxelType.EMPTY)
                    output[1, 0, 2] = new VoxelInfo(VoxelType.PINE_WOOD, true, random.Next(1, height), 0, 0, Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[1, 0, 0].Type) || neighbourhood[1, 0, 0].Type == VoxelType.EMPTY)
                    output[1, 0, 0] = new VoxelInfo(VoxelType.PINE_WOOD, true, random.Next(1, height), 0, 0, Direction.UP);
            }
            else if (gen == -1)
            {
                output[1, 2, 1] = new VoxelInfo(VoxelType.PINE_NEEDLE, true);
                output[1, 1, 1] = new VoxelInfo(VoxelType.PINE_WOOD);
            }
            else if (gen < height)
            {
                if (res > 0 && gen > TypeInformation.GetGrowHeight(VoxelType.PINE_WOOD) / 2)
                {
                    if (random.Next(0, 11) > 7)
                    {
                        res -= 15;
                        Direction grow = GetBranchDirection(neighbourhood);
                        Int3 temp = DirectionConverter.FromDirection(grow);
                        if (neighbourhood[temp.X, 0, temp.Z].Type == VoxelType.EMPTY || TypeInformation.IsNotWoodButBiomass(neighbourhood[temp.X, 0, temp.Z].Type))
                        {
                            output[temp.X, temp.Y, temp.Z] = new VoxelInfo(VoxelType.PINE_WOOD, true, random.Next((int)(height * 0.75), height), 0, 0, DirectionConverter.ToOppositeDirection(grow));
                        }
                    }
                }
                Int3 coord = DirectionConverter.FromDirection(DirectionConverter.ToOppositeDirection(neighbourhood[1, 1, 1].From));
                // Grow in given direction
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[coord.X, coord.Y, coord.Z].Type) || neighbourhood[coord.X, coord.Y, coord.Z].Type == VoxelType.EMPTY)
                {
                    output[coord.X, coord.Y, coord.Z] = new VoxelInfo(VoxelType.PINE_WOOD, true, gen + 1, res, 0, neighbourhood[1, 1, 1].From);
                }
                output[1, 1, 1] = new VoxelInfo(VoxelType.PINE_WOOD);

            }
            else
            {
                // Grow upwards one last time
                if(neighbourhood[1,1,1].From != Direction.UP)
                    output[1, 2, 1] = new VoxelInfo(VoxelType.PINE_WOOD, true, -1, 0, TypeInformation.GetGrowingSteps(VoxelType.PINE_WOOD));
                output[1, 1, 1] = new VoxelInfo(VoxelType.PINE_WOOD);
            }
            return output;
        }

        private Direction GetBranchDirection(VoxelInfo[, ,] neighbourhood)
        {
            if (neighbourhood[2, 1, 1].Type == VoxelType.PINE_WOOD && neighbourhood[0, 1, 1].Type == VoxelType.EMPTY)
                return DirectionConverter.ToDirection(0, 1, 1);
            if (neighbourhood[0, 1, 1].Type == VoxelType.PINE_WOOD && neighbourhood[2, 1, 1].Type == VoxelType.EMPTY)
                return DirectionConverter.ToDirection(2, 1, 1);
            if (neighbourhood[1, 1, 2].Type == VoxelType.PINE_WOOD && neighbourhood[1, 1, 0].Type == VoxelType.EMPTY)
                return DirectionConverter.ToDirection(1, 1, 0);
            if (neighbourhood[1, 1, 0].Type == VoxelType.PINE_WOOD && neighbourhood[1, 1, 2].Type == VoxelType.EMPTY)
                return DirectionConverter.ToDirection(1, 1, 2);
            return Direction.SELF;
        }
    }
}
