using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    /// <summary>
    /// A trunc of a redwood with large branches and subbranches.
    /// Basic shapes: (c - central voxel growing into a spezified direction)
    ///     ( r a voxel with a radial distance to the center which spawned that voxel.
    ///         
    ///       r r r
    ///     r r r r r
    /// c   r r c r r
    ///     r r r r r
    ///       r r r
    /// 
    /// Actions c:
    ///     * Grow upward
    ///     * Spawn branch with some probability
    /// Actions r:
    /// Grows in: Empty and Leaves, partially in redwood (c type growth in r type).
    /// Dies: After spawning new ones
    /// 
    /// Interpretions of values:
    ///     * Generation: A layer/type code (0 - seed, 1+ - center, 1000+ - r).
    ///         This contains the target width since spawn for r types.
    ///     * Resources: Coded target position (c) or source position.
    /// </summary>
    class RedWoodRule : IVoxelRule
    {
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            // Apply each n-th turn
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(VoxelType.REDWOOD)) return null;

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            int res = neighbourhood[1, 1, 1].Resources;

            // Always kill actual voxel
            output[1, 1, 1] = new VoxelInfo(VoxelType.REDWOOD);

            if (gen == 0)
            {
                int targetDistance = Random.Next(0, TypeInformation.GetGrowHeight(VoxelType.REDWOOD) / 3 + 1) + TypeInformation.GetGrowHeight(VoxelType.REDWOOD)*2/3;
                // Place a new targeted c-node on top and 4 r nodes for the first ring.
                if( CanPlaceCType( 1, 2, 1, neighbourhood ) ) output[1,2,1] = new VoxelInfo( VoxelType.REDWOOD, true, targetDistance,
                    GamePlayUtils.EncodeRelativeTarget(0, targetDistance, 0));

                int width = (targetDistance*targetDistance + 45) / (TypeInformation.GetGrowHeight(VoxelType.REDWOOD)*2);
                if (CanPlace(0, 1, 1, neighbourhood)) output[0, 1, 1] = new VoxelInfo(VoxelType.REDWOOD, true, 1000 + width, GamePlayUtils.EncodeRelativeTarget( 0, 0,-1 ), -Random.Next(3));
                if (CanPlace(2, 1, 1, neighbourhood)) output[2, 1, 1] = new VoxelInfo(VoxelType.REDWOOD, true, 1000 + width, GamePlayUtils.EncodeRelativeTarget( 0, 0, 1 ), -Random.Next(3));
                if (CanPlace(1, 1, 0, neighbourhood)) output[1, 1, 0] = new VoxelInfo(VoxelType.REDWOOD, true, 1000 + width, GamePlayUtils.EncodeRelativeTarget(-1, 0, 0 ), -Random.Next(3));
                if (CanPlace(1, 1, 2, neighbourhood)) output[1, 1, 2] = new VoxelInfo(VoxelType.REDWOOD, true, 1000 + width, GamePlayUtils.EncodeRelativeTarget( 1, 0, 0 ), -Random.Next(3));
            }
            else if (gen < 1000)
            {
                int tx, ty, tz;
                GamePlayUtils.DecodeRelativeTarget(res, out tx, out ty, out tz);
                if ( Math.Abs(tx) + Math.Abs(ty) + Math.Abs(tz) == 0) {
                    // Replace by leave (for simplicity)
                    output[1, 1, 1] = new VoxelInfo(VoxelType.REDWOOD_NEEDLE, true);
                    return output;
                }
                // Random step (more or less) in the given direction
                int sample = Random.Next(0, Math.Abs(tx) + Math.Abs(ty) + Math.Abs(tz));
                int x = 0, y = 0, z = 0;
                if( sample < Math.Abs(tx) ) x += Math.Sign(tx);
                else if( (sample - Math.Abs(tx)) < Math.Abs(ty) ) y += Math.Sign(ty);
                else z += Math.Sign(tz);
                // Place a new c-node in target direction
                if (CanPlaceCType(1+z, 1+y, 1+x, neighbourhood)) output[1+z, 1+y, 1+x] = new VoxelInfo(VoxelType.REDWOOD, true, Math.Max(1,gen-1),
                    GamePlayUtils.EncodeRelativeTarget(tx-x, ty-y, tz-z));

                int width = (gen*gen + 45) / (TypeInformation.GetGrowHeight(VoxelType.REDWOOD)*2);
                if (width > 0)
                {
                    // Place 4 r nodes tangential to the current step
                    if (x != 0 || y != 0)
                    {
                        if (CanPlace(0, 1, 1, neighbourhood)) output[0, 1, 1] = new VoxelInfo(VoxelType.REDWOOD, true, 1000 + width, GamePlayUtils.EncodeRelativeTarget( 0, 0,-1 ), -Random.Next(3));
                        if (CanPlace(2, 1, 1, neighbourhood)) output[2, 1, 1] = new VoxelInfo(VoxelType.REDWOOD, true, 1000 + width, GamePlayUtils.EncodeRelativeTarget( 0, 0, 1 ), -Random.Next(3));
                    }
                    if (x != 0 || z != 0)
                    {
                        if (CanPlace(1, 0, 1, neighbourhood)) output[1, 0, 1] = new VoxelInfo(VoxelType.REDWOOD, true, 1000 + width, GamePlayUtils.EncodeRelativeTarget( 0,-1, 0 ), -Random.Next(3));
                        if (CanPlace(1, 2, 1, neighbourhood)) output[1, 2, 1] = new VoxelInfo(VoxelType.REDWOOD, true, 1000 + width, GamePlayUtils.EncodeRelativeTarget( 0, 1, 0 ), -Random.Next(3));
                    }
                    if (y != 0 || z != 0)
                    {
                        if (CanPlace(1, 1, 0, neighbourhood)) output[1, 1, 0] = new VoxelInfo(VoxelType.REDWOOD, true, 1000 + width, GamePlayUtils.EncodeRelativeTarget(-1, 0, 0 ), -Random.Next(3));
                        if (CanPlace(1, 1, 2, neighbourhood)) output[1, 1, 2] = new VoxelInfo(VoxelType.REDWOOD, true, 1000 + width, GamePlayUtils.EncodeRelativeTarget( 1, 0, 0 ), -Random.Next(3));
                    }
                }

                // Place a branch randomly
                if (gen>1 && gen<12 && Random.Next(6)<4)
                {
                    double angle = Random.NextDouble() * 2 * Math.PI;
                    x = (int)(Random.Next(5, 8) * Math.Sin(angle)); y = Random.Next(-1, 2); z = (int)(Random.Next(5, 8) * Math.Cos(angle));
                    int px = 1 + (Math.Abs(x) > Math.Abs(z) ? Math.Sign(x) : 0);
                    x -= px - 1;
                    int pz = 1 + (Math.Abs(z) > Math.Abs(x) ? Math.Sign(z) : 0);
                    z -= pz - 1;
                    if (CanPlaceCType(pz, 1 + y, px, neighbourhood))
                        output[pz, 1 + y, px] = new VoxelInfo(VoxelType.REDWOOD, true, Math.Min(Math.Max(1, gen-3), (int)Math.Sqrt(x*x+y*y+z*z)), GamePlayUtils.EncodeRelativeTarget(x, y, z));
                }
            }
            else if (gen > 1000)
            {
                // r Voxels grow in all directions which are possible and not too far away from source
                int tx, ty, tz;
                GamePlayUtils.DecodeRelativeTarget(res, out tx, out ty, out tz);
                int widthsq = gen - 1000;

                if (CanPlace(1, 1, 0, neighbourhood) && (((tx - 1) * (tx - 1) + ty * ty + tz * tz) < widthsq))
                    output[1, 1, 0] = new VoxelInfo(VoxelType.REDWOOD, true, gen, GamePlayUtils.EncodeRelativeTarget(tx-1, ty, tz), -Random.Next(3));
                if (CanPlace(1, 1, 2, neighbourhood) && (((tx + 1) * (tx + 1) + ty * ty + tz * tz) < widthsq))
                    output[1, 1, 2] = new VoxelInfo(VoxelType.REDWOOD, true, gen, GamePlayUtils.EncodeRelativeTarget(tx+1, ty, tz), -Random.Next(3));
                if (CanPlace(1, 0, 1, neighbourhood) && ((tx * tx + (ty - 1) * (ty - 1) + tz * tz) < widthsq))
                    output[1, 0, 1] = new VoxelInfo(VoxelType.REDWOOD, true, gen, GamePlayUtils.EncodeRelativeTarget(tx, ty-1, tz), -Random.Next(3));
                if (CanPlace(1, 2, 1, neighbourhood) && ((tx * tx + (ty + 1) * (ty + 1) + tz * tz) < widthsq))
                    output[1, 2, 1] = new VoxelInfo(VoxelType.REDWOOD, true, gen, GamePlayUtils.EncodeRelativeTarget(tx, ty+1, tz), -Random.Next(3));
                if (CanPlace(0, 1, 1, neighbourhood) && ((tx * tx + ty * ty + (tz - 1) * (tz - 1)) < widthsq))
                    output[0, 1, 1] = new VoxelInfo(VoxelType.REDWOOD, true, gen, GamePlayUtils.EncodeRelativeTarget(tx, ty, tz-1), -Random.Next(3));
                if (CanPlace(2, 1, 1, neighbourhood) && ((tx * tx + ty * ty + (tz + 1) * (tz + 1)) < widthsq))
                    output[2, 1, 1] = new VoxelInfo(VoxelType.REDWOOD, true, gen, GamePlayUtils.EncodeRelativeTarget(tx, ty, tz+1), -Random.Next(3));
            }

            return output;
        }

        private bool CanPlaceCType(int z, int y, int x, VoxelInfo[, ,] neighbourhood)
        {
            return (neighbourhood[z, y, x].Type == VoxelType.EMPTY)
                || TypeInformation.IsNotWoodButBiomass(neighbourhood[z, y, x].Type)
                || (neighbourhood[z, y, x].Type == VoxelType.REDWOOD);
        }

        private bool CanPlace(int z, int y, int x, VoxelInfo[, ,] neighbourhood)
        {
            return (neighbourhood[z, y, x].Type == VoxelType.EMPTY)
                || TypeInformation.IsNotWoodButBiomass(neighbourhood[z, y, x].Type);
        }
    }
}
