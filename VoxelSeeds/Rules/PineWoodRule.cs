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

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            if (gen == 0)
            {
                // Grow Cross and ovveride current generation
                output[1, 1, 1] = new VoxelInfo(VoxelType.PINE_WOOD, true, 1, 0, TypeInformation.GetGrowingSteps(VoxelType.PINE_WOOD) / 2);
                output[2, 1, 1] = new VoxelInfo(VoxelType.PINE_WOOD, true, 1);
                output[0, 1, 1] = new VoxelInfo(VoxelType.PINE_WOOD, true, 1);
                output[1, 1, 2] = new VoxelInfo(VoxelType.PINE_WOOD, true, 1);
                output[1, 1, 0] = new VoxelInfo(VoxelType.PINE_WOOD, true, 1);

                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[1, 1, 1].Type) || neighbourhood[1, 1, 1].Type == VoxelType.EMPTY)
                    output[1, 1, 1] = new VoxelInfo(VoxelType.PINE_WOOD, true, 1, 0, 0, Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[2, 1, 1].Type) || neighbourhood[2, 1, 1].Type == VoxelType.EMPTY)
                    output[2, 1, 1] = new VoxelInfo(VoxelType.PINE_WOOD, true, 1, 0, 0, Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[0, 1, 1].Type) || neighbourhood[0, 1, 1].Type == VoxelType.EMPTY)
                    output[0, 1, 1] = new VoxelInfo(VoxelType.PINE_WOOD, true, 1, 0, 0, Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[1, 1, 2].Type) || neighbourhood[1, 1, 2].Type == VoxelType.EMPTY)
                    output[1, 1, 2] = new VoxelInfo(VoxelType.PINE_WOOD, true, 1, 0, 0, Direction.UP);
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[1, 1, 0].Type) || neighbourhood[1, 1, 0].Type == VoxelType.EMPTY)
                    output[1, 1, 0] = new VoxelInfo(VoxelType.PINE_WOOD, true, 1, 0, 0, Direction.UP);
            }
            else if (gen < TypeInformation.GetGrowHeight(VoxelType.PINE_WOOD))
            {
                Int3 coord = DirectionConverter.FromDirection(DirectionConverter.ToOppositeDirection(neighbourhood[1, 1, 1].From));
                // Grow in given direction
                if (TypeInformation.IsNotWoodButBiomass(neighbourhood[coord.X, coord.Y, coord.Z].Type) || neighbourhood[coord.X, coord.Y, coord.Z].Type == VoxelType.EMPTY)
                {
                    output[coord.X, coord.Y, coord.Z] = new VoxelInfo(VoxelType.PINE_WOOD, true, gen + 1, 0, 0, neighbourhood[1, 1, 1].From);
                }
                output[1, 1, 1] = new VoxelInfo(VoxelType.PINE_WOOD);
            }
            else
            {
                // Grow upwards one last time
                output[1, 2, 1] = new VoxelInfo(VoxelType.PINE_WOOD);
                output[1, 1, 1] = new VoxelInfo(VoxelType.PINE_WOOD);
            }
            return output;
        }
    }
}
