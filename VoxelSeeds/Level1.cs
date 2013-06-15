using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    class Level1: Level
    {
        override public void Initialize()
        {
            _automaton = new Automaton(100, 50, 100, LevelType.PLAIN, 34857024);
            _numRemainingSeeds[(int)VoxelType.TEAK_WOOD] = 3;

            _resources = 10000;
            _finalParasiteMass = 1000;
            _targetBiomass = 500;
            _currentBiomass = 1;
            _currentParasiteMass = 0;

            _automaton.InsertSeed(50, 35, 50, VoxelType.TEAK_WOOD);
        }
    }
}
