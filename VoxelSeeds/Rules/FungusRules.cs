using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    class FungusRules : IVoxelRule
    {
        private VoxelType _thisType;
        public FungusRules(VoxelType fungusType)
        {
            _thisType = fungusType;
        }
        
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            // Apply each x-th turn
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(_thisType)) return null;

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            int min = 0;// TypeInformation.GetGrowingSteps(_thisType) / 2;
            int max = TypeInformation.GetGrowingSteps(_thisType) - 2;
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
                                if (voxeltype == VoxelType.EMPTY || //
                                    (TypeInformation.GetResistance(voxeltype, _thisType) < Random.Next(101)) // or there is a voxel in which the fungus can grow
                                   )
                                {
                                    if (CanFungusGrowOn(t, h, b, neighbourhood)) // there is a voxel in d6 on which the fungus can grow on
                                    {
                                        // grow
                                        growadd = Random.Next(min, max) - (TypeInformation.GetResistance(voxeltype, _thisType) / 4);
                                      /*  if (voxeltype == VoxelType.ROCK || voxeltype == VoxelType.TEAK_WOOD) growadd = -30;
                                        if (voxeltype == VoxelType.PINE_WOOD || voxeltype == VoxelType.BEECH_WOOD ||
                                            voxeltype == VoxelType.OAK_WOOD || voxeltype == VoxelType.REDWOOD) growadd = 10;*/
                                        output[t, h, b] = new VoxelInfo(_thisType, true, 0, 0, growadd);
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
                if (fung == 6) output[2, 1, 1] = new VoxelInfo(_thisType);
            }

            return output;
        }

        private bool CanFungusGrowOn(int t, int h, int b, VoxelInfo[, ,] neighbourhood)
        {
            bool res = false;
            if (t < 2) res = res || TypeInformation.CanFungusGrowOn(neighbourhood[t + 1, h, b].Type, _thisType);
            if (t > 0) res = res || TypeInformation.CanFungusGrowOn(neighbourhood[t - 1, h, b].Type, _thisType);
            if (h < 2) res = res || TypeInformation.CanFungusGrowOn(neighbourhood[t, h + 1, b].Type, _thisType);
            if (h > 0) res = res || TypeInformation.CanFungusGrowOn(neighbourhood[t, h - 1, b].Type, _thisType);
            if (b < 2) res = res || TypeInformation.CanFungusGrowOn(neighbourhood[t, h, b + 1].Type, _thisType);
            if (b > 0) res = res || TypeInformation.CanFungusGrowOn(neighbourhood[t, h, b - 1].Type, _thisType);

//            if (res) res = 5 < Random.Next(0, 15);

            return res;
        }

    }
}
