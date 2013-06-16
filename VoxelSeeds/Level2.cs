using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    class Level2 : Level
    {
        public Level2(VoxelRenderer voxelRenderer) : base(voxelRenderer)
        {
        }

        override public void Initialize()
        {
            _automaton = new Automaton(150, 40, 150, LevelType.PLAIN, 52384, 0.8f);

            _resources = 1000;
            _finalParasiteMass = 50;
            _targetBiomass = 1000;
            _currentBiomass = 1;
            _currentParasiteMass = 0;

            Random rand = new Random();
            for (int i = 0; i < 100; ++i)
            {
                _automaton.InsertSeed(rand.Next(GetMap().SizeX), rand.Next(GetMap().SizeY / 4) + 5, rand.Next(GetMap().SizeZ), VoxelType.HESPEROPHANES_CINNEREUS);
            }

            int x = GetMap().SizeX / 2;
            int z = GetMap().SizeZ / 2;
            int y = GetMap().GetHeighest(x, z);
            _automaton.InsertSeed(x, Math.Max(y, 0) + 1, z, VoxelType.WHITEROT_FUNGUS);
        }
    }
}
