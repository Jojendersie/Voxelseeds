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

            InsertSeed(125, Math.Max(GetMap().GetHeighest(125, 80), 0) + 1, 80, VoxelType.WHITEROT_FUNGUS);
            InsertSeed(61, Math.Max(GetMap().GetHeighest(61, 24), 0) + 1, 24, VoxelType.NOBLEROT_FUNGUS);
            InsertSeed(74, Math.Max(GetMap().GetHeighest(74, 109), 0) + 1, 109, VoxelType.WHITEROT_FUNGUS);

            for(int i=15; i<500; i+=36) InsertSeed(13, Math.Max(GetMap().GetHeighest(13, i), 0) + 1, i, VoxelType.HOUSE_LONGHORN_BEETLE);
        }
    }
}
