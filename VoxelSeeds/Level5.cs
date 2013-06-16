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

            _resources = 1000;
            _finalParasiteMass = 1000;
            _targetBiomass = 1000;

         //   for(int i=15; i<300; i+=36)
            //     InsertParasite(13, Math.Max(GetMap().GetHeighest(13, i), 0) + 1, i, VoxelType.WHITEROT_FUNGUS);
        }
    }
}
