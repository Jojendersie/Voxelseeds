using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    /// <summary>
    /// A jumping beetle.
    /// 
    /// Actions:
    ///  * hopp upwards
    ///  * at max height move one field horizontal
    ///  * fall
    ///  * eat if there is something in 4 neighbourhood
    /// Dies: If leaving world
    /// 
    /// Interpretions of values:
    ///     resources: upward accerlation
    /// </summary>
    class GrasshopperRule : IVoxelRule
    {
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(VoxelType.GRASSHOPPER)) return null;
            int gen = neighbourhood[1, 1, 1].Generation;
            int res = neighbourhood[1, 1, 1].Resources;

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];

            int x, y, z;
            if (FoodInDirection(neighbourhood, out z, out y, out x))
            {
                // Eat -> spawn in neighbourhood and keep the old one
                output[z, y, x] = new VoxelInfo(VoxelType.GRASSHOPPER, true, 0, 0, Random.Next(-10, -5));
            }
            else
            {
                // If hopper stays here that field will be overwritten - otherwise it moves
                output[1, 1, 1] = new VoxelInfo(VoxelType.EMPTY);

                // On ground jump
                if (neighbourhood[1, 0, 1].Type != VoxelType.EMPTY) res = Random.Next(1, 4);
                // On top of jump or if some obstacle move sidewards
                x = 1; z = 1;
                if (res == 0 || neighbourhood[1, 2, 1].Type != VoxelType.EMPTY)
                {
                    // Chose a random empty neighbourhood cell
                    int emptyCount = 0;
                    for (int k = 0; k < 3; ++k) for (int i = 0; i < 3; ++i)
                            if (neighbourhood[k, 1, i].Type == VoxelType.EMPTY) ++emptyCount;
                    emptyCount = Random.Next(0, emptyCount);
                    for (int k = 0; k < 3; ++k) for (int i = 0; i < 3; ++i)
                            if (neighbourhood[k, 1, i].Type == VoxelType.EMPTY && emptyCount-- == 0) { x = i; z = k; }
                }
                // "simulate" flow
                output[z, 1 + Math.Sign(res), x] = new VoxelInfo(VoxelType.GRASSHOPPER, true, 0, res - 1);
            }

            return output;
        }

        private bool IsWalkable(VoxelInfo voxelInfo)
        {
            return voxelInfo.Type == VoxelType.EMPTY;
        }

        private bool FoodInDirection(VoxelInfo[, ,] neighbourhood, out int z, out int y, out int x)
        {
            z = y = x = 1;
            // This method is biased since it always prefers directions.
            // Probably this effect is not noticeable.
            if (!TypeInformation.IsResistent(neighbourhood[0, 1, 1].Type, VoxelType.GRASSHOPPER)) { z = 0; return true; }
            if (!TypeInformation.IsResistent(neighbourhood[2, 1, 1].Type, VoxelType.GRASSHOPPER)) { z = 2; return true; }
            if (!TypeInformation.IsResistent(neighbourhood[1, 1, 0].Type, VoxelType.GRASSHOPPER)) { x = 0; return true; }
            if (!TypeInformation.IsResistent(neighbourhood[1, 1, 2].Type, VoxelType.GRASSHOPPER)) { x = 2; return true; }
            return false;
        }
    }
}
