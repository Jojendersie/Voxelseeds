using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    class NobleRotRule : IVoxelRule
    {
        Random random = new Random();
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            // Apply each 18-th turn
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(VoxelType.NOBLEROT_FUNGUS)) return null;

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            int min = 0;// TypeInformation.GetGrowingSteps(VoxelType.NOBLEROT_FUNGUS) / 2;
            int max = TypeInformation.GetGrowingSteps(VoxelType.NOBLEROT_FUNGUS) / 2;
            int growadd;

            if (gen == 0)
            {
                int fung = 0;
                VoxelType voxeltype;
                // check the neighbourhood
                for (int t = 0; t < 3; ++t)
                    for (int h = 0; h < 3; ++h)
                        for (int b = 0; b < 3; ++b) if (!(t == 1 && h == 1 && b == 1)) // if not the voxel itself
                            {
                                voxeltype = neighbourhood[t, h, b].Type;
                                if ((TypeInformation.IsBiomass(voxeltype) // there is a voxel in which the fungus can grow
                                    || voxeltype == VoxelType.EMPTY) // or the field is empty
                                    && !TypeInformation.IsGroundOrFungus(voxeltype))
                                {
                                    if (CanFungusGrowOn(t, h, b, neighbourhood)) // there is a voxel in d6 on which the fungus can grow on
                                    {
                                        // grow
                                        growadd = 0;
                                        if (voxeltype == VoxelType.ROCK || voxeltype == VoxelType.TEAK_WOOD) growadd = -10;
                                        if (voxeltype == VoxelType.PINE_WOOD || voxeltype == VoxelType.BEECH_WOOD ||
                                            voxeltype == VoxelType.REDWOOD) growadd = 3;

                                        output[t, h, b] = new VoxelInfo(VoxelType.NOBLEROT_FUNGUS, true, 0, 0, random.Next(min, max) + growadd);
                                    }
                                }
                            }
                // check for kill the fungus (if in d6 all places are occupied)
                if (TypeInformation.IsGroundOrFungus(neighbourhood[2, 1, 1].Type)) fung++;
                if (TypeInformation.IsGroundOrFungus(neighbourhood[0, 1, 1].Type)) fung++;
                if (TypeInformation.IsGroundOrFungus(neighbourhood[1, 2, 1].Type)) fung++;
                if (TypeInformation.IsGroundOrFungus(neighbourhood[1, 0, 1].Type)) fung++;
                if (TypeInformation.IsGroundOrFungus(neighbourhood[1, 1, 2].Type)) fung++;
                if (TypeInformation.IsGroundOrFungus(neighbourhood[1, 1, 0].Type)) fung++;
                if (fung == 6) output[2, 1, 1] = new VoxelInfo(VoxelType.NOBLEROT_FUNGUS);
            }

            return output;
        }

        private bool CanFungusGrowOn(int t, int h, int b, VoxelInfo[, ,] neighbourhood)
        {
            bool res = false;
            if (t < 2) res = res || TypeInformation.CanFungusGrowOn(neighbourhood[t + 1, h, b].Type);
            if (t > 0) res = res || TypeInformation.CanFungusGrowOn(neighbourhood[t - 1, h, b].Type);
            if (h < 2) res = res || TypeInformation.CanFungusGrowOn(neighbourhood[t, h + 1, b].Type);
            if (h > 0) res = res || TypeInformation.CanFungusGrowOn(neighbourhood[t, h - 1, b].Type);
            if (b < 2) res = res || TypeInformation.CanFungusGrowOn(neighbourhood[t, h, b + 1].Type);
            if (b > 0) res = res || TypeInformation.CanFungusGrowOn(neighbourhood[t, h, b - 1].Type);

            if(res) res = 5 < random.Next(0, 15);

            return res;
        }
    }
}
