using System;
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
        /// Enough space is defined as:
        /// The weighted area sum of a circle of a spezified size must be at
        /// least 90% on the y-level where the seed should be set and the level - 1
        /// has to contain at least 50% Ground (again weighted).
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static public bool IsThereEnoughSpaceFor(Map map, VoxelType type, int x, int y, int z)
        {
            float num = 0f;
            float numAvailable = 0f;
            float numGroundBelow = 0f;
            int radius = TypeInformation.GetRequiredSpace(type);
            for (int w = -radius; w <= radius; ++w)
                for (int u = -radius; u <= radius; ++u)
                {
                    // The center gets the largest weight. Decreasing to zero at edges
                    float fWeight = (float)(radius-Math.Sqrt(u * u + w * w));
                    if (fWeight >= 0)
                    {
                        num += fWeight;
                        if (map.IsEmpty(map.EncodePosition(x + u, y, z + w)))
                            numAvailable += fWeight;
                        if (map.Get(map.EncodePosition(x + u, y - 1, z + w)) == VoxelType.GROUND)
                            numGroundBelow += fWeight;
                    }
                }

            return ((numAvailable/num) >= 0.90f)
                && ((numGroundBelow/num) >= 0.5f);
        }
    }
}
