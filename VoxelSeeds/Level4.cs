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

            _resources = 10000;
            _finalParasiteMass = 20000;
            _targetBiomass = 50000;
            _currentBiomass = 0;
            _currentParasiteMass = 3;

            InsertParasite(125, Math.Max(GetMap().GetHeighest(125, 80), 0) + 1, 80, VoxelType.WHITEROT_FUNGUS);
            InsertParasite(61, Math.Max(GetMap().GetHeighest(61, 24), 0) + 1, 24, VoxelType.WHITEROT_FUNGUS);
            InsertParasite(74, Math.Max(GetMap().GetHeighest(74, 109), 0) + 1, 109, VoxelType.WHITEROT_FUNGUS);
        }
    }
}
