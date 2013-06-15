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
    abstract class Level
    {
        public int TargetBiomass { get { return _targetBiomass; } protected set { _targetBiomass = value; } }
        public int CurrentBiomass { get { return _currentBiomass; } protected set { _currentBiomass = value; } }
        public int ParasiteBiomass { get { return _parasiteMass; } protected set { _parasiteMass = value; } }
        public int FinalParasiteBiomass { get { return _finalParasiteMass; } protected set { _finalParasiteMass = value; } }

        public Level()
        {
            _numRemainingSeeds = new int[Enum.GetValues(typeof(VoxelType)).Length];
        }

        /// <summary>
        /// Fill all variables and the automaton with meaningful variables.
        /// It is not necessary to allocate the _numRemainingSeeds array
        /// just fill it.
        /// </summary>
        public abstract void Initialize();

        public bool IsVictory()    { return _currentBiomass >= _targetBiomass; }
        public bool IsLost() { return _parasiteMass >= _finalParasiteMass; }

        public void Tick(Action<Voxel[], Voxel[]> updateInstanceData)
        {
            _automaton.Tick(ref updateInstanceData);
        }

        protected Automaton _automaton;
        protected int _targetBiomass;
        protected int _currentBiomass;
        protected int _parasiteMass;
        protected int _finalParasiteMass;

        protected int[] _numRemainingSeeds;
    }
}
