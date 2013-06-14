using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    enum VoxelType
    {
        GROUND,
        WOOD,
        FUNGUS
    };

    struct VoxelInfo
    {
        /// <summary>
        /// Create a new voxel. A new voxel is always in living state.
        /// </summary>
        /// <param name="type">A type of the enumeration type. Do not used more than
        /// 128 different types in the enumeration.</param>
        public VoxelInfo(VoxelType type, bool living = true, int resources = 0, int generation = 0)
        {
            System.Diagnostics.Debug.Assert( (byte)type < 128 );
            // First bit == living or dead
            _type = type;
            _living = living;
            _resources = resources;
            _generation = generation;
        }

        public bool Living { get { return _living; } }
        public VoxelType Type { get { return _type; } }
        public int Resources { get { return _resources; } }
        public int Generation { get { return _generation; } }

        readonly VoxelType _type;
        readonly bool _living;
        readonly int _resources;
        readonly int _generation;
    }
}
