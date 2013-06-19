using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    class Level4 : Level
    {
        public Level4(VoxelRenderer voxelRenderer)
            : base(voxelRenderer)
        {
        }

        override public void Initialize()
        {
            _automaton = new Automaton(150, 80, 150, LevelType.MOUNTAINS, 51239478, 1.3f);

            _resources = 30000;
            _finalParasiteMass = 10000;
            _targetBiomass = 10000;

            InsertSeed(125, Math.Max(GetMap().GetHeighest(125, 80), 0) + 1, 80, VoxelType.WHITEROT_FUNGUS);
            InsertSeed(61, Math.Max(GetMap().GetHeighest(61, 24), 0) + 1, 24, VoxelType.NOBLEROT_FUNGUS);
            InsertSeed(74, Math.Max(GetMap().GetHeighest(74, 109), 0) + 1, 109, VoxelType.WHITEROT_FUNGUS);
        }
    }
}
