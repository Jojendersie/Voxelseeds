using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    /// <summary>
    /// This is building spherical leaf balls
    /// </summary>
    class OakLeafRule : IVoxelRule
    {
        
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            // Apply each n-th turn
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(VoxelType.OAK_LEAF)) return null;

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            int res = neighbourhood[1, 1, 1].Resources;

            if (gen < TypeInformation.GetGrowHeight(VoxelType.BEECH_LEAF))
            {
                // Grow to the all directions
                output[1, 1, 1] = new VoxelInfo(VoxelType.OAK_LEAF);
                if (CanPlace(2, 1, 1, neighbourhood)) output[2, 1, 1] = new VoxelInfo(VoxelType.OAK_LEAF, true, gen + Random.Next(1, 3), 0, Random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.OAK_LEAF)));
                if (CanPlace(0, 1, 1, neighbourhood)) output[0, 1, 1] = new VoxelInfo(VoxelType.OAK_LEAF, true, gen + Random.Next(1, 3), 0, Random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.OAK_LEAF)));
                if (CanPlace(1, 1, 2, neighbourhood)) output[1, 1, 2] = new VoxelInfo(VoxelType.OAK_LEAF, true, gen + Random.Next(1, 3), 0, Random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.OAK_LEAF)));
                if (CanPlace(1, 1, 0, neighbourhood)) output[1, 1, 0] = new VoxelInfo(VoxelType.OAK_LEAF, true, gen + Random.Next(1, 3), 0, Random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.OAK_LEAF)));
                if (CanPlace(1, 2, 1, neighbourhood)) output[1, 2, 1] = new VoxelInfo(VoxelType.OAK_LEAF, true, gen + Random.Next(1, 3), 0, Random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.OAK_LEAF)));
                if (CanPlace(1, 0, 1, neighbourhood)) output[1, 0, 1] = new VoxelInfo(VoxelType.OAK_LEAF, true, gen + Random.Next(1, 3), 0, Random.Next(0, TypeInformation.GetGrowingSteps(VoxelType.OAK_LEAF)));
            }
            else
            {
                // Stop growing
                output[1, 1, 1] = new VoxelInfo(VoxelType.OAK_LEAF);
            }
            return output;
        }

        bool CanPlace(int t, int h, int b, VoxelInfo[, ,] neighbourhood)
        {
            return (neighbourhood[t, h, b].Type == VoxelType.EMPTY);
            //    || TypeInformation.IsNotWoodButBiomass(neighbourhood[t, h, b].Type);
        }
    }
}
