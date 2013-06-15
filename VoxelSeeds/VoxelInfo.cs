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
        PINE_WOOD,
        SPRUCE_WOOD,
        BEECH_WOOD,
        OAK_WOOD,
        REDWOOD,
        WHITEROT_FUNGUS,
        NOBLEROT_FUNGUS,
        TERMITES,
        HOUSE_LONGHORN_BEETLE,
        HESPEROPHANES_CINNEREUS,
        GRASSHOPPER
    };

    class TypeInformation
    {   //same order as the VoxelType enum
        readonly static int[] prices = {0, 0, 120, 90, 152, 68, 116, 956, 0, 0, 0, 0, 0 };
        readonly static String[,] strength = { { "", "" }, { "-", "-" }, 
                                             { "White Rot", "Noble Rot" }, 
                                             { "Hesperophanes Cinnereus", "Grasshopper" }, 
                                             { "Hesperophanes Cinnereus", "Grasshopper" }, 
                                             { "House Longhorn Beetle", "Grasshopper" }, 
                                             { "House Longhorn Beetle", "Grasshopper" },
                                             { "Termites", "Grasshopper" },
                                             { "-", "-" }, { "-", "-" }, { "-", "-" }, { "-", "-" }, { "-", "-" }, { "-", "-" } };
        readonly static String[,] weakness = { { "", "" }, { "-", "-" }, 
                                             { "Termites", "Hesperophanes Cinnereus" },
                                             { "White Rot", "Noble Rot" },
                                             { "Termites", "House Longhorn Beetle" },
                                             { "White Rot", "Noble Rot" },
                                             { "White Rot", "Hesperophanes Cinnereus" },
                                             { "White Rot", "Noble Rot" },
                                             { "-", "-" }, { "-", "-" }, { "-", "-" }, { "-", "-" }, { "-", "-" }, { "-", "-" } };
        readonly static String[] name = { "", "Ground", "Teak", "Pine", "Spruce", "Beech", "Oak", "Redwood", "White Rot", "Noble Rot", "Termites", "House Longhorn Beetle", "Hesperophanes Cinnereus", "Grasshopper" };
        readonly static bool[] parasite = { false, false, false, false, false, false, false, false, true, true, true, true, true, true };
        readonly static IVoxelRule[] rules = { null, null,
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule() };

        public static String GetName(VoxelType voxeltype)
        {
            return name[(int)voxeltype];
        }

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

        public static bool IsParasite(VoxelType voxeltype)
        {
            return parasite[(int)voxeltype];
        }

        public static IVoxelRule GetRule(VoxelType voxeltype)
        {
            return rules[(int)voxeltype];
        }
    }

    class VoxelInfo
    {
        /// <summary>
        /// Create a new voxel. A new voxel is always in living state.
        /// </summary>
        /// <param name="type">A type of the enumeration type. Do not used more than
        /// 128 different types in the enumeration.</param>
        public VoxelInfo(VoxelType type, bool living = false, int generation = 0, int resources = 0, int ticks = 0)
        {
            System.Diagnostics.Debug.Assert( (byte)type < 128 );
            // First bit == living or dead
            _type = type;
            _living = living;
            _resources = resources;
            _generation = generation;
            _ticks = ticks;
        }

        public bool Living { get { return _living; } }
        public VoxelType Type { get { return _type; } }
        public int Resources { get { return _resources; } }
        public int Generation { get { return _generation; } }
        public int Ticks { get { return _ticks; } }

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
            PositionCode = pCode;
            Type = t;
        }

        public Int32 PositionCode;
        public VoxelType Type;
    }
}
