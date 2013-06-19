using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    class Level5 : Level
    {
        public Level5(VoxelRenderer voxelRenderer)
            : base(voxelRenderer)
        {
        }

        override public void Initialize()
        {
            _automaton = new Automaton(100, 40, 300, LevelType.CANYON, 19351, 0.8f);

            _resources = 3000;
            _finalParasiteMass = 1000;
            _targetBiomass = 1000;

            InsertSeed(15, Math.Max(GetMap().GetHeighest(15, 180), 0) + 1, 180, VoxelType.WHITEROT_FUNGUS);
            InsertSeed(61, Math.Max(GetMap().GetHeighest(61, 24), 0) + 1, 24, VoxelType.NOBLEROT_FUNGUS);
            InsertSeed(74, Math.Max(GetMap().GetHeighest(74, 209), 0) + 1, 209, VoxelType.WHITEROT_FUNGUS);

            for (int i = 0; i < 100; ++i)
            {
                InsertSeed(Random.Next(GetMap().SizeX), Random.Next(GetMap().SizeY / 4) + 28, Random.Next(GetMap().SizeZ), VoxelType.HOUSE_LONGHORN_BEETLE);
            }
        }
    }
}
