using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    class HesperophanesCinnereusRule: IVoxelRule
    {
        Random random = new Random();
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(VoxelType.HESPEROPHANES_CINNEREUS)) return null;

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            int res = neighbourhood[1, 1, 1].Resources;

            int t = random.Next(0, 3);
            int h = random.Next(0, 3);
            int b = random.Next(0, 3);

            if (gen == 0)
            {
                if (IsWalkable(neighbourhood[t, h, b]))
                {
                    output[1, 1, 1] = new VoxelInfo(VoxelType.EMPTY); ;
                    output[t, h, b] = new VoxelInfo(VoxelType.HESPEROPHANES_CINNEREUS, true, 1);
                }
                else
                {
                    output[1, 1, 1] = new VoxelInfo(VoxelType.HESPEROPHANES_CINNEREUS, true, 1);
                }
            }
            else if (gen < TypeInformation.GetGrowHeight(VoxelType.HESPEROPHANES_CINNEREUS) && res == 10)
            {
                Int3 moveTo = DirectionConverter.FromDirection(neighbourhood[1, 1, 1].From);
                t = moveTo.X;
                h = moveTo.Y;
                b = moveTo.Z;
                output[t, h, b] = new VoxelInfo(VoxelType.HESPEROPHANES_CINNEREUS, true, 0);
                if (random.Next(0, 11) > 7)
                    output[1, 1, 1] = new VoxelInfo(VoxelType.HESPEROPHANES_CINNEREUS, true, gen + random.Next(1, 5), 0, TypeInformation.GetGrowingSteps(VoxelType.HESPEROPHANES_CINNEREUS), Direction.SELF);
                else
                    output[1, 1, 1] = new VoxelInfo(VoxelType.EMPTY);
            }
            else// if (gen < TypeInformation.GetGrowHeight(VoxelType.HESPEROPHANES_CINNEREUS))
            {
                Direction food = FoodInDirection(neighbourhood);
                if (food != Direction.SELF)
                {
                    Int3 foodPos = DirectionConverter.FromDirection(food);
                    int eattime = -10;
                    if (neighbourhood[foodPos.X, foodPos.Y, foodPos.Z].Type == VoxelType.PINE_WOOD || neighbourhood[foodPos.X, foodPos.Y, foodPos.Z].Type == VoxelType.SPRUCE_WOOD) eattime = -20;
                    if (neighbourhood[foodPos.X, foodPos.Y, foodPos.Z].Type == VoxelType.TEAK_WOOD || neighbourhood[foodPos.X, foodPos.Y, foodPos.Z].Type == VoxelType.OAK_WOOD) eattime = -5;

                    output[1, 1, 1] = new VoxelInfo(VoxelType.HESPEROPHANES_CINNEREUS, true, gen + random.Next(0, 3), 10, eattime, food);
                }
                else if (IsWalkable(neighbourhood[t, h, b]))
                {
                    output[1, 1, 1] = new VoxelInfo(VoxelType.EMPTY);
                    output[t, h, b] = new VoxelInfo(VoxelType.HESPEROPHANES_CINNEREUS, true, gen + random.Next(1, 5));
                }
                else
                {
                    output[1, 1, 1] = new VoxelInfo(VoxelType.HESPEROPHANES_CINNEREUS, true, gen + random.Next(1, 5));
                }
            }
           /* else
            {
                output[1, 1, 1] = new VoxelInfo(VoxelType.EMPTY);
            }*/

            return output;
        }

        private bool IsWalkable(VoxelInfo voxelInfo)
        {
            return voxelInfo.Type == VoxelType.EMPTY;
        }

        private Direction FoodInDirection(VoxelInfo[, ,] neighbourhood)
        {
            for (int t = 0; t < 3; ++t)
                    for (int h = 0; h < 3; ++h)
                        for (int b = 0; b < 3; ++b) if (!(t == 1 && h == 1 && b == 1)) // if not the voxel itself
                            {
                                if (TypeInformation.IsBiomass(neighbourhood[t, h, b].Type))
                                    return DirectionConverter.ToDirection(t, h, b);
                            }
            return Direction.SELF;
        }
    }
}
