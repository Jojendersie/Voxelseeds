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
            _automaton = new Automaton(300, 80, 300, LevelType.PLAIN, 51239478, 0.01f);

            _resources = 1000;
            _finalParasiteMass = 1000;
            _targetBiomass = 1000;
            _currentBiomass = 1;
            _currentParasiteMass = 0;

/*            int x = GetMap().SizeX / 4;
            int z = GetMap().SizeZ / 2;
            int y = GetMap().GetHeighest(x, z);
            _automaton.InsertSeed(x, Math.Max(y, 0) + 1, z, VoxelType.WHITEROT_FUNGUS);
            x = GetMap().SizeX - GetMap().SizeX / 4;
            _automaton.InsertSeed(x, Math.Max(y, 0) + 1, z, VoxelType.NOBLEROT_FUNGUS); */
        }
    }
}
