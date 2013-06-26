using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    /// <summary>
    /// A trunc of a beech with large branches.
    /// Basic shapes: ( w-dead oak wood, or branch 1000+)
    ///     w w  w
    /// 0   w 0+ w
    ///     w w  w
    ///     
    /// Actions:
    ///     * Grow upward and increase resources
    ///     * Spawn branch if enough resources
    ///     * Spawn leaves at brach ends
    /// Grows in: Empty and Leaves.
    /// Dies: After spawn
    /// 
    /// Interpretions of values:
    ///     * Generation: A layer/type code (0+ - center, 1000+ - branch).
    ///         This contains the length since spawn for the different types.
    ///     * Resources: 0+: How long since last branching
    ///                  1000+: Coded target position.
    /// </summary>
    class BeechWoodRule : IVoxelRule
    {
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            // Apply each n-th turn
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(VoxelType.BEECH_WOOD)) return null;

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            int res = neighbourhood[1, 1, 1].Resources;

            if (gen < 1000)
            {
                // Grow upward and to the sides
                if (gen < TypeInformation.GetGrowHeight(VoxelType.BEECH_WOOD))
                {
                    if (CanPlace(0, 1, 0, neighbourhood)) output[0, 1, 0] = new VoxelInfo(VoxelType.BEECH_WOOD);
                    if (CanPlace(1, 1, 0, neighbourhood)) output[1, 1, 0] = new VoxelInfo(VoxelType.BEECH_WOOD);
                    if (CanPlace(2, 1, 0, neighbourhood)) output[2, 1, 0] = new VoxelInfo(VoxelType.BEECH_WOOD);
                    if (CanPlace(0, 1, 1, neighbourhood)) output[0, 1, 1] = new VoxelInfo(VoxelType.BEECH_WOOD);
                    if (CanPlace(1, 1, 1, neighbourhood)) output[1, 1, 1] = new VoxelInfo(VoxelType.BEECH_WOOD);
                    if (CanPlace(2, 1, 1, neighbourhood)) output[2, 1, 1] = new VoxelInfo(VoxelType.BEECH_WOOD);
                    if (CanPlace(0, 1, 2, neighbourhood)) output[0, 1, 2] = new VoxelInfo(VoxelType.BEECH_WOOD);
                    if (CanPlace(1, 1, 2, neighbourhood)) output[1, 1, 2] = new VoxelInfo(VoxelType.BEECH_WOOD);
                    if (CanPlace(2, 1, 2, neighbourhood)) output[2, 1, 2] = new VoxelInfo(VoxelType.BEECH_WOOD);

                    // Exchange one of them randomly if new branch should spawn
                    if (res >= 4)
                    {
                        int rndx = 1, rndz = 1;
                        while (rndx * rndz == 1)
                        {
                            rndx = Random.Next(0, 3);
                            rndz = Random.Next(0, 3);
                        }
                        if (CanPlace(rndz, 1, rndx, neighbourhood))
                        {
                            int tx = (rndx - 1) * 4 + Random.Next(-1, 2);
                            int ty = Random.Next(-2, 4);
                            int tz = (rndz - 1) * 4 + Random.Next(-1, 2);
                            output[rndz, 1, rndx] = new VoxelInfo(VoxelType.BEECH_WOOD, true, 1000 + Random.Next(4, 6), (100 * (tz + 50) + ty + 50) * 100 + tx + 50);
                            res -= 2;
                        }
                        if (CanPlace(2-rndz, 1, 2-rndx, neighbourhood))
                        {
                            int tx = -(rndx - 1) * 4 + Random.Next(-1, 2);
                            int ty = Random.Next(-2, 4);
                            int tz = -(rndz - 1) * 4 + Random.Next(-1, 2);
                            output[2-rndz, 1, 2-rndx] = new VoxelInfo(VoxelType.BEECH_WOOD, true, 1000 + Random.Next(4, 6), (100 * (tz + 50) + ty + 50) * 100 + tx + 50);
                            res -= 2;
                        }
                    }

                    if (CanPlace(1, 2, 1, neighbourhood))
                        output[1, 2, 1] = new VoxelInfo(VoxelType.BEECH_WOOD, true, gen + (Random.Next(6) == 1 ? 0 : 1), res + (Random.Next(3) == 1 ? 3 : 2));
                }
                else
                {
                    // Top level - kill wood and spanw leaves
                    if (CanPlace(0, 1, 1, neighbourhood)) output[0, 1, 1] = new VoxelInfo(VoxelType.BEECH_LEAF, true);
                    if (CanPlace(2, 1, 1, neighbourhood)) output[2, 1, 1] = new VoxelInfo(VoxelType.BEECH_LEAF, true);
                    if (CanPlace(1, 1, 0, neighbourhood)) output[1, 1, 0] = new VoxelInfo(VoxelType.BEECH_LEAF, true);
                    if (CanPlace(1, 1, 2, neighbourhood)) output[1, 1, 2] = new VoxelInfo(VoxelType.BEECH_LEAF, true);
                    if (CanPlace(1, 2, 1, neighbourhood)) output[1, 2, 1] = new VoxelInfo(VoxelType.BEECH_LEAF, true);
                    output[1, 1, 1] = new VoxelInfo(VoxelType.BEECH_WOOD);
                }
            }
            else if (gen == 1000)
            {
                // End of a branch
                output[1, 1, 1] = new VoxelInfo(VoxelType.BEECH_WOOD);
                if (CanPlace(1, 2, 1, neighbourhood))
                    output[1, 2, 1] = new VoxelInfo(VoxelType.BEECH_LEAF, true);
                else if(CanPlace(1, 0, 1, neighbourhood))
                    output[1, 0, 1] = new VoxelInfo(VoxelType.BEECH_LEAF, true);
            }
            else
            {
                // Inside branch go to the cell which is nearest to the target.
                int tx = res % 100 - 50;
                int ty = (res / 100) % 100 - 50;
                int tz = res / 10000 - 50;

                // Random step (more or less) in the given direction
                int x = Math.Sign(tx) * (Random.Next(0, 6) < Math.Abs(tx) ? 1 : 0) + 1;
                int y = Math.Sign(ty) * (Random.Next(0, 6) < Math.Abs(ty) ? 1 : 0) + 1;
                int z = Math.Sign(tz) * (Random.Next(0, 6) < Math.Abs(tz) ? 1 : 0) + 1;
                if (CanPlace(z, y, x, neighbourhood))
                    output[z, y, x] = new VoxelInfo(VoxelType.BEECH_WOOD, true, gen - 1, res);
                output[1, 1, 1] = new VoxelInfo(VoxelType.BEECH_WOOD);
            }

            return output;
        }

        private bool CanPlace(int z, int y, int x, VoxelInfo[, ,] neighbourhood)
        {
            return (neighbourhood[z, y, x].Type == VoxelType.EMPTY)
                || TypeInformation.IsNotWoodButBiomass(neighbourhood[z, y, x].Type);
        }
    }
}
