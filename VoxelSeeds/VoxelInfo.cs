using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelSeeds.Rules;

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
                                                 new PineWoodRule(),
                                                 new SpruceWoodRule(),
                                                 new BeechWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new TeakWoodRule() };
        /// <summary>
        /// The maximum number of voxels of a type which can be simultaneously in the world
        /// </summary>
        readonly static int[] maxNumberOfVoxels = { 0, 131072, 131072, 131072, 131072, 131072, 131072, 131072, 131072, 131072, 512, 512, 512, 512 };
        /// <summary>
        /// A scaling factor for voxels it is used to display bugs and beetles smaller
        /// </summary>
        readonly static float[] scalingFactor = { 1.0f, 1.0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 0.5f, 0.5f, 0.5f};

        //readonly static int[] growingSteps = { 0, 0, 18, 30, 32, 48, 46, 16, 6, 6, 2, 2, 2, 2 };
        readonly static int[] growingSteps = { 0, 0, 2, 4, 8, 2, 2, 2, 2, 2, 2, 2, 2, 2 };

        readonly static int[] growHeight = { 0, 0, 7, 10, 8, 6, 8, 19, 1, 1, 1, 1, 1, 1 };

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

        public static int GetMaxNumberOfVoxels(VoxelType voxeltype)
        {
            return maxNumberOfVoxels[(int)voxeltype];
        }

        public static float GetScalingFactor(VoxelType voxeltype)
        {
            return scalingFactor[(int)voxeltype];
        }

        public static int GetGrowingSteps(VoxelType voxeltype)
        {
            return growingSteps[(int)voxeltype];
        }

        public static int GetGrowHeight(VoxelType voxeltype)
        {
            return growHeight[(int)voxeltype];
        }
    }

    enum Direction
    {
        LEFT,
        RIGHT,
        FOR,
        BACK,
        UP,
        DOWN,
    };

    class VoxelInfo
    {
        /// <summary>
        /// Create a new voxel. A new voxel is always in living state.
        /// </summary>
        /// <param name="type">A type of the enumeration type. Do not used more than
        /// 128 different types in the enumeration.</param>
        public VoxelInfo(VoxelType type, bool living = false, int generation = 0, int resources = 0, int ticks = 0, Direction from = Direction.DOWN)
        {
            System.Diagnostics.Debug.Assert( (byte)type < 128 );
            // First bit == living or dead
            _type = type;
            _living = living;
            _resources = resources;
            _generation = generation;
            _ticks = ticks;
            _from = from;
        }

        public bool Living { get { return _living; } }
        public VoxelType Type { get { return _type; } }
        public int Resources { get { return _resources; } }
        public int Generation { get { return _generation; } }
        public int Ticks { get { return _ticks; } }
        public Direction From { get { return _from; } }

        readonly VoxelType _type;
        readonly bool _living;
        readonly int _resources;
        readonly int _generation;
        readonly Direction _from;
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
