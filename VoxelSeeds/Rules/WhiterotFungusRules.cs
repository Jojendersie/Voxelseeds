﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds.Rules
{
    class WhiterotFungusRules : IVoxelRule
    {
        Random random = new Random();
        public VoxelInfo[, ,] ApplyRule(VoxelInfo[, ,] neighbourhood)
        {
            // Apply each 18-th turn
            if (neighbourhood[1, 1, 1].Ticks < TypeInformation.GetGrowingSteps(VoxelType.WHITEROT_FUNGUS)) return null;

            VoxelInfo[, ,] output = new VoxelInfo[3, 3, 3];
            int gen = neighbourhood[1, 1, 1].Generation;
            int min = TypeInformation.GetGrowingSteps(VoxelType.WHITEROT_FUNGUS) / 2;
            int max = TypeInformation.GetGrowingSteps(VoxelType.WHITEROT_FUNGUS);

            if (gen == 0)
            {
                int fung = 0;
                // Grow Cross
                if (neighbourhood[2, 1, 1].Type != VoxelType.WHITEROT_FUNGUS && neighbourhood[2, 0, 1].Type != VoxelType.WHITEROT_FUNGUS && neighbourhood[2, 0, 1].Type != VoxelType.EMPTY)
                    output[2, 1, 1] = new VoxelInfo(VoxelType.WHITEROT_FUNGUS, true, 0, 0, random.Next(min, max));
                else if (neighbourhood[2, 1, 1].Type == VoxelType.WHITEROT_FUNGUS) fung++;
                if (neighbourhood[0, 1, 1].Type != VoxelType.WHITEROT_FUNGUS && neighbourhood[0, 0, 1].Type != VoxelType.WHITEROT_FUNGUS && neighbourhood[0, 0, 1].Type != VoxelType.EMPTY)
                    output[0, 1, 1] = new VoxelInfo(VoxelType.WHITEROT_FUNGUS, true, 0, 0, random.Next(min, max));
                else if (neighbourhood[0, 1, 1].Type == VoxelType.WHITEROT_FUNGUS) fung++;
                if (neighbourhood[1, 1, 2].Type != VoxelType.WHITEROT_FUNGUS && neighbourhood[1, 0, 2].Type != VoxelType.WHITEROT_FUNGUS && neighbourhood[1, 0, 2].Type != VoxelType.EMPTY)
                    output[1, 1, 2] = new VoxelInfo(VoxelType.WHITEROT_FUNGUS, true, 0, 0, random.Next(min, max));
                else if (neighbourhood[1, 1, 2].Type == VoxelType.WHITEROT_FUNGUS) fung++;
                if (neighbourhood[1, 1, 0].Type != VoxelType.WHITEROT_FUNGUS && neighbourhood[1, 0, 0].Type != VoxelType.WHITEROT_FUNGUS && neighbourhood[1, 0, 0].Type != VoxelType.EMPTY)
                    output[1, 1, 0] = new VoxelInfo(VoxelType.WHITEROT_FUNGUS, true, 0, 0, random.Next(min, max));
                else if (neighbourhood[1, 1, 0].Type == VoxelType.WHITEROT_FUNGUS) fung++;
                if (neighbourhood[1, 2, 1].Type != VoxelType.WHITEROT_FUNGUS && neighbourhood[1, 2, 1].Type != VoxelType.EMPTY && neighbourhood[1, 2, 1].Type != VoxelType.GROUND)
                {
                    output[1, 2, 1] = new VoxelInfo(VoxelType.WHITEROT_FUNGUS, true, 0, 0, random.Next(min, max));
                }
                else if (neighbourhood[1, 2, 1].Type == VoxelType.WHITEROT_FUNGUS)
                    fung++;
                if (neighbourhood[1, 0, 1].Type != VoxelType.WHITEROT_FUNGUS /*&& neighbourhood[1, 0, 1].Type != VoxelType.EMPTY*/ && neighbourhood[1, 0, 1].Type != VoxelType.GROUND)
                {
                    output[1, 0, 1] = new VoxelInfo(VoxelType.WHITEROT_FUNGUS, true, 0, 0, random.Next(min, max));
                }
                else fung++;
                if (fung == 6) output[2, 1, 1] = new VoxelInfo(VoxelType.WHITEROT_FUNGUS);
            }

            return output;
        }

    }
}