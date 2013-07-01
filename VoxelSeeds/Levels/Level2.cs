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
            _automaton = new Automaton(150, 40, 150, LevelType.PLAIN, 52386, 0.8f);

            _finalParasiteMass = 8000;
            _targetBiomass = 1500;
            _countDown = 0.75f;

            for (int i = 0; i < 80; ++i)
            {
                InsertSeed(Random.Next(GetMap().SizeX - 6) + 3, Random.Next(GetMap().SizeY / 4) + 5, Random.Next(GetMap().SizeZ - 6) + 3, VoxelType.HESPEROPHANES_CINNEREUS);
            }

            
            InsertSeed(137, Math.Max(GetMap().GetHeighest(137, 101), 0) + 1, 101, VoxelType.WHITEROT_FUNGUS);
        }
    }
}
