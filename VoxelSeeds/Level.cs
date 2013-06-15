using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    /// <summary>
    /// A level creates and maintains a map and the automaton for the living
    /// parts.
    /// 
    /// The level is a base class. To define a spezific level create a child
    /// class.
    /// </summary>
    class Level
    {
        public int TargetBiomass { get { return _targetBiomass; } }
        public int CurrentBiomass { get { return _targetBiomass; } }
        public int ParasiteBiomass { get { return _targetBiomass; } }

        Level()
        {
            _numRemainingSeeds = new int[Enum.GetValues(typeof(VoxelType)).Length];
        }

        /// <summary>
        /// Fill all variables and the automaton with meaningful variables.
        /// It is not necessary to allocate the _numRemainingSeeds array
        /// just fill it.
        /// </summary>
        virtual abstract void Initialize();

        bool IsVictory()    { return _currentBiomass >= _targetBiomass; }
        bool IsLost()       { return _parasiteMass >= _finalParasiteMass;  }

        void Tick(Action<Voxel[], Voxel[]> updateInstanceData)
        {
            _automaton.Tick(ref updateInstanceData);
        }

        Automaton _automaton;
        int _targetBiomass;
        int _currentBiomass;
        int _parasiteMass;
        int _finalParasiteMass;

        int[] _numRemainingSeeds;
    }
}
