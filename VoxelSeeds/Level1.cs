using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    class Level1: Level
    {
        public Level1(VoxelRenderer voxelRenderer)
            : base(voxelRenderer)
        {
        }

        override public void Initialize()
        {
            _automaton = new Automaton(100, 50, 100, LevelType.PLAIN, 34857024);
            _numRemainingSeeds[(int)VoxelType.TEAK_WOOD] = 3;

            _resources = 10000;
            _finalParasiteMass = 1000;
            _targetBiomass = 500;
            _currentBiomass = 1;
            _currentParasiteMass = 1;
            int x = GetMap().SizeX / 2;
            int z = GetMap().SizeZ / 2;
            int y;
            for (y = GetMap().SizeY - 1; y > 0; --y)
            {
                if (GetMap().Get(GetMap().EncodePosition(x, y, z)) == VoxelType.GROUND)
                {
                    break;
                }
            }
            _automaton.InsertSeed(x,y+1,z,VoxelType.WHITEROT_FUNGUS);
        }
    }
}
