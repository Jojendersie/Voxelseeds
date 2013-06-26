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

            _finalParasiteMass = 1000;
            _targetBiomass = 20000;
            _countDown = 2.5f;

            // No parasites - just see the growth
        }
    }
}
