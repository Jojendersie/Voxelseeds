using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    class Level3 : Level
    {
        public Level3(VoxelRenderer voxelRenderer) : base(voxelRenderer)
        {
        }

        override public void Initialize()
        {
            _automaton = new Automaton(200, 60, 200, LevelType.BUBBLE, 3485704, 1.3f);

            _finalParasiteMass = 12500;
            _targetBiomass = 5000;
            _countDown = 0.5f;

            int x = GetMap().SizeX / 2 + 5;
            int z = GetMap().SizeZ / 2;
            int y = GetMap().GetHeighest(x, z);
            InsertSeed(x, Math.Max(y, 0) + 1, z, VoxelType.NOBLEROT_FUNGUS);

            for (int i = 0; i < 180; ++i)
            {
                InsertSeed(Random.Next(GetMap().SizeX - 6) + 3, Random.Next(GetMap().SizeY - 6) + 3, Random.Next(GetMap().SizeZ - 6) + 3, VoxelType.HOUSE_LONGHORN_BEETLE);
            }
        }
    }
}
