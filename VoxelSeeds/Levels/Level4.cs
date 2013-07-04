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
            _automaton = new Automaton(150, 80, 150, LevelType.MOUNTAINS, 51239479, 1.2f);

            _finalParasiteMass = 12000;
            _targetBiomass = 3400;
            _countDown = 0.5f;

            InsertSeed(125, Math.Max(GetMap().GetHeighest(125, 80), 0) + 1, 80, VoxelType.WHITEROT_FUNGUS);
            InsertSeed(61, Math.Max(GetMap().GetHeighest(61, 24), 0) + 1, 24, VoxelType.NOBLEROT_FUNGUS);

            for (int i = 0; i < 80; ++i)
            {
                int x = Random.Next(GetMap().SizeX - 6) + 3;
                int z = Random.Next(GetMap().SizeZ - 6) + 3;
                InsertSeed(x, Math.Max(GetMap().GetHeighest(x, z), 0) + 1, z, VoxelType.GRASSHOPPER);
            }
        }
    }
}
