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
            _automaton = new Automaton(100, 50, 100, LevelType.PLAIN, 1234);

            _resources = 1000;
            _finalParasiteMass = 1000;
            _targetBiomass = 1000;
            _currentBiomass = 1;
            _currentParasiteMass = 0;
        }
    }
}
