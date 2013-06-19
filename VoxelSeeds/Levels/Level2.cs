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

            _finalParasiteMass = 20000;
            _targetBiomass = 1500;
            _countDown = 1.3333333f;

            for (int i = 0; i < 80; ++i)
            {
                InsertSeed(Random.Next(GetMap().SizeX), Random.Next(GetMap().SizeY / 4) + 5, Random.Next(GetMap().SizeZ), VoxelType.HESPEROPHANES_CINNEREUS);
            }

            
            InsertSeed(26, Math.Max(GetMap().GetHeighest(26, 38), 0) + 1, 38, VoxelType.WHITEROT_FUNGUS);
        }
    }
}
