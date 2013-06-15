using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    enum VoxelType
    {
        EMPTY = 0,
        GROUND,
        TEAK_WOOD,
        FUNGUS
    };

    class TypeInformation
    {   //empty,Ground,Wood,Fungus
        readonly static int[] prices = {0, 0, 5, 0 };
        readonly static String[,] strength = { {"",""}, { "-", "-" }, { "Water", "Earth" }, { "-", "-" } };
        readonly static String[,] weakness = { { "", "" }, { "-", "-" }, { "Fire", "Heaven" }, { "-", "-" } };

        public static String[] GetStrength(VoxelType voxeltype)
        {
            String[] result = { strength[(int)voxeltype, 0], strength[(int)voxeltype, 1] };
            return result;
        }

        public static String[] GetWeakness(VoxelType voxeltype)
        {
            String[] result = { weakness[(int)voxeltype, 0], weakness[(int)voxeltype, 1] };
            return result;
        }

        public static int GetPrice(VoxelType voxeltype)
        {
            return prices[(int)voxeltype];
        }

    }

    class VoxelInfo
    {
        /// <summary>
        /// Create a new voxel. A new voxel is always in living state.
        /// </summary>
        /// <param name="type">A type of the enumeration type. Do not used more than
        /// 128 different types in the enumeration.</param>
        public VoxelInfo(VoxelType type, bool living = false, int generation = 0, int resources = 0)
        {
            System.Diagnostics.Debug.Assert( (byte)type < 128 );
            // First bit == living or dead
            _type = type;
            _living = living;
            _resources = resources;
            _generation = generation;
            _ticks = 0;
        }

        public bool Living { get { return _living; } }
        public VoxelType Type { get { return _type; } }
        public int Resources { get { return _resources; } }
        public int Generation { get { return _generation; } }
        public int Ticks { get { return _ticks; } }
        public void IncrementTicks() { ++_ticks;  }

        readonly VoxelType _type;
        readonly bool _living;
        readonly int _resources;
        readonly int _generation;
        int _ticks;

        // TODO Ticks
    }

    /// <summary>
    /// Information for the communication to the graphic pipeline.
    /// </summary>
    struct Voxel
    {
        public Voxel(Int32 pCode, VoxelType t)
        {
            positionCode = pCode;
            type = t;
        }

        Int32 positionCode;
        VoxelType type;
    }
}
