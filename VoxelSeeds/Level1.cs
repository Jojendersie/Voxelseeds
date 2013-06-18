using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    class Level1: Level
    {
        public Level1(VoxelRenderer voxelRenderer)
            : base(voxelRenderer)
        {
        }

        override public void Initialize()
        {
            _automaton = new Automaton(100, 50, 100, LevelType.PLAIN, 34857024, 0.25f);
            //_automaton = new Automaton(100, 50, 100, LevelType.STEP, 33333333, 0.25f);

            _resources = 100;
            _finalParasiteMass = 3500;
            _targetBiomass = 1000;
            _countDown = 1.0f;

            int x = GetMap().SizeX / 2;
            int z = GetMap().SizeZ / 2;
            int y = GetMap().GetHeighest(x, z);
            InsertSeed(x, Math.Max(y, 0) + 1, z, VoxelType.WHITEROT_FUNGUS);
        }
    }
}
