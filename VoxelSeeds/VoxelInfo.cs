using SharpDX;
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
        ROCK,
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
        GRASSHOPPER,
        TEAK_LEAF,
        PINE_NEEDLE,
        SPRUCE_NEEDLE
    };

    class TypeInformation
    {   //same order as the VoxelType enum
        readonly static int[] prices = {0, 0, 0, 120, 90, 152, 68, 116, 956, 0, 0, 0, 0, 0, 0, 0 };
        readonly static String[,] strength = { { "", "" }, { "-", "-" }, { "-", "-" }, 
                                             { "White Rot", "Noble Rot" }, 
                                             { "Hesperophanes Cinnereus", "Grasshopper" }, 
                                             { "Hesperophanes Cinnereus", "Grasshopper" }, 
                                             { "House Longhorn Beetle", "Grasshopper" }, 
                                             { "House Longhorn Beetle", "Grasshopper" },
                                             { "Termites", "Grasshopper" },
                                             { "-", "-" }, { "-", "-" }, { "-", "-" }, { "-", "-" }, { "-", "-" }, { "-", "-" },
                                             {"-", "-"},{"-", "-"},{"-", "-"}};
        readonly static String[,] weakness = { { "", "" }, { "-", "-" }, { "-", "-" }, 
                                             { "Termites", "Hesperophanes Cinnereus" },
                                             { "White Rot", "Noble Rot" },
                                             { "Termites", "House Longhorn Beetle" },
                                             { "White Rot", "Noble Rot" },
                                             { "White Rot", "Hesperophanes Cinnereus" },
                                             { "White Rot", "Noble Rot" },
                                             { "-", "-" }, { "-", "-" }, { "-", "-" }, { "-", "-" }, { "-", "-" }, { "-", "-" },
                                             {"-", "-"},{"-", "-"},{"-", "-"}};
        readonly static String[] name = { "", "Ground", "Rock", "Teak", "Pine", "Spruce", "Beech", "Oak", "Redwood", "White Rot", "Noble Rot", "Termites", "House Longhorn Beetle", "Hesperophanes Cinnereus", "Grasshopper", "Leaf", "Needle", "Needle" };
        readonly static bool[] parasite = { false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, false, false, false };
        readonly static bool[] biomass = { false, false, false, true, true, true, true, true, true, false, false, false, false, false, false, true, true, true };
        readonly static IVoxelRule[] rules = { null, null, null,
                                                 new TeakWoodRule(),
                                                 new PineWoodRule(),
                                                 new SpruceWoodRule(),
                                                 new BeechWoodRule(),
                                                 new OakWoodRule(),
                                                 new TeakWoodRule(),
                                                 new WhiterotFungusRules(),
                                                 new NobleRotRule(),
                                                 new TeakWoodRule(),
                                                 new HouseLonghornBeetleRule(),
                                                 new HesperophanesCinnereusRule(),
                                                 new TeakWoodRule(),
                                                 new TeakLeafRule(),
                                                 new PineNeedleRule(),
                                                 new SpruceNeedleRule()};
        /// <summary>
        /// The maximum number of voxels of a type which can be simultaneously in the world
        /// </summary>
        readonly static int[] maxNumberOfVoxels = { 0, 131072, 131072, 131072, 131072, 131072, 131072, 131072, 131072, 131072, 131072, 512, 512, 512, 512, 131072, 131072, 131072 };
        /// <summary>
        /// A scaling factor for voxels it is used to display bugs and beetles smaller
        /// </summary>
        readonly static float[] scalingFactor = { 1.0f, 1.0f, 1.0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 0.5f, 0.5f, 0.5f, 1f, 1f, 1f};

        readonly static int[] growingSteps = { 0, 0, 0, 18, 30, 32, 48, 46, 16, 16, 16, 2, 2, 2, 2, 5 };
        //readonly static int[] growingSteps = { 0, 0, 0, 2, 4, 8, 2, 2, 2, 16, 2, 2, 2, 2, 2, 5, 3, 6 };

        readonly static int[] growHeight = { 0, 0, 0, 7, 10, 8, 6, 8, 19, 1, 1, 1, 1, 50, 1, 5, 2, 7 };

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

        public static bool IsBiomass(VoxelType voxeltype)
        {
            return biomass[(int)voxeltype];
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

        public static bool CanFungusGrowOn(VoxelType voxeltype)
        {
            return IsBiomass(voxeltype) || voxeltype == VoxelType.GROUND || voxeltype == VoxelType.ROCK;
        }

        public static bool IsWood(VoxelType voxeltype)
        {
            return (int)voxeltype >= (int)VoxelType.TEAK_WOOD && (int)voxeltype <= (int)VoxelType.REDWOOD;
        }

        public static bool IsInsect(VoxelType voxeltype)
        {
            return (int)voxeltype >= (int)VoxelType.TERMITES && (int)voxeltype <= (int)VoxelType.GRASSHOPPER;
        }

        public static bool IsGroundOrFungus(VoxelType voxeltype)
        {
            return voxeltype == VoxelType.ROCK || voxeltype == VoxelType.GROUND || voxeltype == VoxelType.WHITEROT_FUNGUS || voxeltype == VoxelType.NOBLEROT_FUNGUS;
        }

        public static bool IsNotWoodButBiomass(VoxelType voxeltype)
        {
            return IsBiomass(voxeltype) && !IsWood(voxeltype);
        }
    }

    enum Direction
    {
        SELF,
        LEFT,
        RIGHT,
        FOR,
        BACK,
        UP,
        DOWN,
        FOR_LEFT,
        FOR_UP_LEFT,
        FOR_DOWN_LEFT,
        FOR_RIGHT,
        FOR_UP_RIGHT,
        FOR_DOWN_RIGHT,
        FOR_UP,
        FOR_DOWN,
        BACK_LEFT,
        BACK_UP_LEFT,
        BACK_DOWN_LEFT,
        BACK_RIGHT,
        BACK_UP_RIGHT,
        BACK_DOWN_RIGHT,
        BACK_UP,
        BACK_DOWN,
        DOWN_LEFT,
        DOWN_RIGHT,
        UP_LEFT,
        UP_RIGHT
    };

    class DirectionConverter
    {
        public static Int3 FromDirection(Direction direction)
        {
            int t = 1;
            int h = 1;
            int b = 1;

            switch (direction)
            {
                case Direction.BACK: t = 2; break;
                case Direction.BACK_DOWN_LEFT: t = 2; h = 0; b = 0; break;
                case Direction.BACK_DOWN_RIGHT: t = 2; h = 0; b = 2; break;
                case Direction.BACK_LEFT: t = 2; b = 0; break;
                case Direction.BACK_RIGHT: t = 2; b = 2; break;
                case Direction.BACK_UP_LEFT: t = 2; h = 2; b = 0; break;
                case Direction.BACK_UP_RIGHT: t = 2; h = 2; b = 2; break;
                case Direction.BACK_DOWN: t = 2; h = 0; break;
                case Direction.BACK_UP: t = 2; h = 2; break;
                case Direction.FOR: t = 0; break;
                case Direction.FOR_DOWN_LEFT: t = 0; h = 0; b = 0; break;
                case Direction.FOR_DOWN_RIGHT: t = 0; h = 0; b = 2; break;
                case Direction.FOR_LEFT: t = 0; b = 0; break;
                case Direction.FOR_RIGHT: t = 0; b = 2; break;
                case Direction.FOR_UP_LEFT: t = 0; h = 2; b = 0; break;
                case Direction.FOR_UP_RIGHT: t = 0; h = 2; b = 2; break;
                case Direction.FOR_DOWN: t = 0; h = 0; break;
                case Direction.FOR_UP: t = 0; h = 2; break;
                case Direction.DOWN: h = 0; break;
                case Direction.DOWN_LEFT: h = 0; b = 0; break;
                case Direction.DOWN_RIGHT: h = 0; b = 2; break;
                case Direction.UP: h = 2; break;
                case Direction.UP_LEFT: h = 2; b = 0; break;
                case Direction.UP_RIGHT: h = 2; b = 2; break;
                case Direction.LEFT: b = 0; break;
                case Direction.RIGHT: b = 2; break;
                default: break;
            }

            return new Int3(t,h,b);
        }

        public static Direction ToOppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.BACK: return Direction.FOR;
                case Direction.BACK_DOWN_LEFT: return Direction.FOR_UP_RIGHT;
                case Direction.BACK_DOWN_RIGHT: return Direction.FOR_UP_LEFT;
                case Direction.BACK_LEFT: return Direction.FOR_RIGHT;
                case Direction.BACK_RIGHT: return Direction.FOR_LEFT;
                case Direction.BACK_UP_LEFT: return Direction.FOR_DOWN_RIGHT;
                case Direction.BACK_UP_RIGHT: return Direction.FOR_DOWN_LEFT;
                case Direction.BACK_DOWN: return Direction.FOR_UP;
                case Direction.BACK_UP: return Direction.FOR_DOWN;
                case Direction.FOR: return Direction.BACK;
                case Direction.FOR_DOWN_LEFT: return Direction.BACK_UP_RIGHT;
                case Direction.FOR_DOWN_RIGHT: return Direction.BACK_UP_LEFT;
                case Direction.FOR_LEFT: return Direction.BACK_RIGHT;
                case Direction.FOR_RIGHT: return Direction.BACK_LEFT;
                case Direction.FOR_UP_LEFT: return Direction.BACK_DOWN_RIGHT;
                case Direction.FOR_UP_RIGHT: return Direction.BACK_DOWN_LEFT;
                case Direction.FOR_DOWN: return Direction.BACK_UP;
                case Direction.FOR_UP: return Direction.BACK_DOWN;
                case Direction.DOWN: return Direction.UP;
                case Direction.DOWN_LEFT: return Direction.UP_RIGHT;
                case Direction.DOWN_RIGHT: return Direction.UP_LEFT;
                case Direction.UP: return Direction.DOWN;
                case Direction.UP_LEFT: return Direction.DOWN_RIGHT;
                case Direction.UP_RIGHT: return Direction.DOWN_LEFT;
                case Direction.LEFT: return Direction.RIGHT;
                case Direction.RIGHT: return Direction.LEFT;
                default: return Direction.SELF;
            }
        }

        public static Direction ToDirection(int t, int h, int b)
        {
            Direction result = Direction.SELF;
            if (t == 0)
            {
                result = Direction.FOR;
                if (h == 0)
                {
                    result = Direction.FOR_DOWN;
                    if (b == 0) result = Direction.FOR_DOWN_LEFT;
                    if (b == 2) result = Direction.FOR_DOWN_RIGHT;
                }
                else if (h == 2)
                {
                    result = Direction.FOR_UP;
                    if (b == 0) result = Direction.FOR_UP_LEFT;
                    if (b == 2) result = Direction.FOR_UP_RIGHT;
                }
                else
                {
                    if (b == 0) result = Direction.FOR_LEFT;
                    if (b == 2) result = Direction.FOR_RIGHT;
                }
            }
            else if (t == 2)
            {
                result = Direction.BACK;
                if (h == 0)
                {
                    result = Direction.BACK_DOWN;
                    if (b == 0) result = Direction.BACK_DOWN_LEFT;
                    if (b == 2) result = Direction.BACK_DOWN_RIGHT;
                }
                else if (h == 2)
                {
                    result = Direction.BACK_UP;
                    if (b == 0) result = Direction.BACK_UP_LEFT;
                    if (b == 2) result = Direction.BACK_UP_RIGHT;
                }
                else
                {
                    if (b == 0) result = Direction.BACK_LEFT;
                    if (b == 2) result = Direction.BACK_RIGHT;
                }
            }
            else
            {
                if (h == 0)
                {
                    result = Direction.DOWN;
                    if (b == 0) result = Direction.DOWN_LEFT;
                    if (b == 2) result = Direction.DOWN_RIGHT;
                }
                else if (h == 2)
                {
                    result = Direction.BACK_UP;
                    if (b == 0) result = Direction.UP_LEFT;
                    if (b == 2) result = Direction.UP_RIGHT;
                }
                else
                {
                    if (b == 0) result = Direction.LEFT;
                    if (b == 2) result = Direction.RIGHT;
                }
            }
            return result;
        }
    }

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
