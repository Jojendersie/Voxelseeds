﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using System.Diagnostics;

namespace VoxelSeeds
{
    static class GamePlayUtils
    {
        static public VoxelInfo GetStrongerVoxel(ref VoxelInfo a, ref VoxelInfo b)
        {
            return a;
        }

        static public bool GetEmptyTarget(VoxelInfo[, ,] neighbourhood, out int t, out int h, out int b)
        {
            t = -1; h = -1; b = -1;
            // Search twice (first count than choose with Random index
            int emptyCount = 0;
            for (int z = 0; z < 3; ++z) for (int y = 0; y < 3; ++y) for (int x = 0; x < 3; ++x)
                if (neighbourhood[z, y, x].Type == VoxelType.EMPTY)
                    ++emptyCount;
            if (emptyCount == 0) return false;
            emptyCount = Random.Next(emptyCount);
            for (int z = 0; z < 3; ++z) for (int y = 0; y < 3; ++y) for (int x = 0; x < 3; ++x)
            {
                if (neighbourhood[z, y, x].Type == VoxelType.EMPTY)
                    if (emptyCount == 0) { t = z; h = y; b = x; return true; } 
                    else --emptyCount;
            }
            Debug.Assert(false);
            return false;
        }

        /// <summary>
        /// If there is someting a species of type type can eat it will be chosen.
        /// If there is more than one the choice is Random.
        /// If there is nothing a Random position is reported too.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="neighbourhood"></param>
        /// <param name="t"></param>
        /// <param name="h"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static public VoxelType GetEatableTarget(VoxelType type, VoxelInfo[, ,] neighbourhood, out int t, out int h, out int b)
        {
            // Search twice (first count than choose with Random index
            int eatableCount = 0;
            for (int z = 0; z < 3; ++z) for (int y = 0; y < 3; ++y) for (int x = 0; x < 3; ++x)
                if (!TypeInformation.IsResistent(neighbourhood[z, y, x].Type, type))
                    ++eatableCount;
            if (eatableCount == 0)
            {
                // Random fall back target
                t = Random.Next(0, 3);
                h = Random.Next(0, 3);
                b = Random.Next(0, 3);
                return neighbourhood[t, h, b].Type;
            }
            eatableCount = Random.Next(eatableCount);
            for (int z = 0; z < 3; ++z) for (int y = 0; y < 3; ++y) for (int x = 0; x < 3; ++x)
            {
                if (!TypeInformation.IsResistent(neighbourhood[z, y, x].Type, type))
                    if (eatableCount == 0) { t = z; h = y; b = x; return neighbourhood[z, y, x].Type; }
                    else --eatableCount;
            }
            Debug.Assert(false);
            t = -1; h = -1; b = -1;
            return VoxelType.EMPTY;
        }

        /// <summary>
        /// If there is something allergic in any direction report that
        /// </summary>
        /// <param name="neighbourhood"></param>
        /// <returns></returns>
     /*   static public Int3 FoodInDirection(VoxelInfo[, ,] neighbourhood)
        {
            for (int t = 0; t < 3; ++t)
                for (int h = 0; h < 3; ++h)
                    for (int b = 0; b < 3; ++b) if (!(t == 1 && h == 1 && b == 1)) // if not the voxel itself
                        {
                            if (TypeInformation.IsBiomass(neighbourhood[t, h, b].Type) && !TypeInformation.IsResistent(neighbourhood[t, h, b].Type, neighbourhood[1, 1, 1].Type))
                                return new Int3(t, h, b);
                        }
            return null;
        }*/

        /// <summary>
        /// Enough space is defined as:
        /// A circle of a spezified size must be empty on the y-level where the
        /// seed should be set. And the level-1 has to contain at least 30% Ground.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static public bool IsThereEnoughSpaceFor(Map map, VoxelType type, int x, int y, int z)
        {
            int num = 0;
            int numAvailable = 0;
            int numGroundBelow = 0;
            int radius = TypeInformation.GetRequiredSpace(type);
            for (int w = -radius; w <= radius; ++w)
                for (int u = -radius; u <= radius; ++u) if(u*u+w*w <= radius*radius)
                {
                    ++num;
                    if (map.IsEmpty(map.EncodePosition(x + u, y, z + w)))
                        ++numAvailable;
                    if (map.Get(map.EncodePosition(x + u, y - 1, z + w)) == VoxelType.GROUND)
                        ++numGroundBelow;
                }

            return numAvailable == num
                && ((numGroundBelow/(float)num) >= 0.3f);
        }
    }
}
