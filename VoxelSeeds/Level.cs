using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public int CurrentBiomass { get { int mass = 0; for (VoxelType t = VoxelType.EMPTY; t < VoxelType.ENUM_END; ++t) if (TypeInformation.IsBiomass(t)) mass+=GetMap().GetNumVoxels(t); return mass; } }
        public int CurrentParasiteMass { get { int mass = 0; for (VoxelType t = VoxelType.EMPTY; t < VoxelType.ENUM_END; ++t) if (TypeInformation.IsParasite(t)) mass += GetMap().GetNumVoxels(t); return mass; } }
        public int FinalParasiteBiomass { get { return _finalParasiteMass; } protected set { _finalParasiteMass = value; } }
        public int Resources { get { return (int)_resources; } set { _resources = value; } }
        public string CountDown { get { return Math.Floor(_countDown).ToString("00") + ':' + Math.Ceiling(60.0f * (_countDown - Math.Floor(_countDown))).ToString("00"); } }

        public Vector3 LightDirection { get { return lightDirection; } protected set { lightDirection = value; } }
        Vector3 lightDirection = new Vector3(1.0f, -1.3f, 1.7f);

        public Level(VoxelRenderer voxelRenderer)
        {
            _countDown = 1.0f;
            Initialize();

            lightDirection.Normalize();
            voxelRenderer.Reset(GetMap(), lightDirection);
            SetInstanceUpdateMethod(voxelRenderer.Update);
        }

        /// <summary>
        /// Fill all variables and the automaton with meaningful variables.
        /// It is not necessary to allocate the _numRemainingSeeds array
        /// just fill it.
        /// </summary>
        public abstract void Initialize();

        public bool IsVictory() { return _countDown <= 0.0f; }
        public bool IsLost() { return (CurrentParasiteMass >= _finalParasiteMass) || (_resources < 81 && _automaton.NumLivingBiomass==0); }
        public bool TheClockIsTicking() { return CurrentBiomass > _targetBiomass; }

        public Map GetMap() { return _automaton.Map; }
        public void InsertSeed(int x, int y, int z, VoxelType type) { _automaton.InsertSeed(x, y, z, type); }
        void SetInstanceUpdateMethod(Action<IEnumerable<Voxel>, IEnumerable<Voxel>> updateInstanceData) { _automaton.SetInstanceUpdateMethod(ref updateInstanceData); }


        public void Tick()
        {
            int[] newWrittenVoxels = _automaton.Tick();

            if( newWrittenVoxels != null )
            for (VoxelType t = VoxelType.EMPTY; t < VoxelType.ENUM_END; ++t)
            {
                if (TypeInformation.IsWood(t))          _resources += newWrittenVoxels[(int)t] * 1.3f;
                else if (TypeInformation.IsBiomass(t))  _resources += newWrittenVoxels[(int)t] / 3.0f;
            }

            if (TheClockIsTicking())
                _countDown = Math.Max(0.0f, _countDown - 0.25f / 60.0f);
        }

        protected Automaton _automaton;
        protected int _targetBiomass;
        protected int _finalParasiteMass;
        protected float _resources;

        // Timespan the player must hold the _targetBiomass level in minutes.
        protected float _countDown;
    }
}
