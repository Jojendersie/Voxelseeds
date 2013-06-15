using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VoxelSeeds
{
    enum LevelType
    {
        PLAIN
    };

    /// <summary>
    /// The map is a 3D array of byte values.
    /// 
    /// Later the map gets a loader or generator for the different levels.
    /// 
    /// The most significant bit encodes if the voxel is living or dead.
    /// </summary>
    class Map
    {
        public Map(int sizeX, int sizeY, int sizeZ, LevelType lvlType, int seed)
        {
            _voxels = new byte[sizeX*sizeY*sizeZ];
            _sizeX = sizeX;
            _sizeY = sizeY;
            _sizeZ = sizeZ;

            // Create the ground
            Random rand = new Random(seed);
            switch (lvlType)
            {
                case LevelType.PLAIN: GeneratePlainLevel(ref rand); break;
            }
        }

        public Int32 SizeX { get { return _sizeX; } }
        public Int32 SizeY { get { return _sizeY; } }
        public Int32 SizeZ { get { return _sizeZ; } }

        // The pure data of the whole volume
        byte[] _voxels;
        readonly Int32 _sizeX;
        readonly Int32 _sizeY;
        readonly Int32 _sizeZ;

        /// <summary>
        /// Returns a unique number for each position.
        /// </summary>
        /// <param name="x">Voxel postion x</param>
        /// <param name="y">Voxel postion y</param>
        /// <param name="z">Voxel postion z</param>
        /// <returns>The PositionCode containting every information for the
        /// position.</returns>
        public Int32 EncodePosition(int x, int y, int z)
        {
            Debug.Assert((0 <= x) && (x < _sizeX));
            Debug.Assert((0 <= y) && (y < _sizeY));
            Debug.Assert((0 <= z) && (z < _sizeZ));
            return (Int32)(x+SizeX*(y+SizeY*z));
        }
        public Int32 EncodePosition(SharpDX.Int3 pos)
        {
            return EncodePosition(pos.X, pos.Y, pos.Z);
        }

        /// <summary>
        /// Calculates the original position from a position code.
        /// 
        /// This direction is slower than EncodePosition.
        /// </summary>
        /// <param name="positionCode"></param>
        public SharpDX.Int3 DecodePosition(Int32 positionCode)
        {
            return new SharpDX.Int3(
                positionCode % SizeX,
                (positionCode / SizeX) % SizeY,
                positionCode / (SizeX * SizeY));
        }

        public byte Sample(int x, int y, int z)
        {
            return _voxels[EncodePosition(x, y, z)];
        }


        public byte Sample(Int32 positionCode)
        {
            return _voxels[positionCode];
        }

        public VoxelType Get(Int32 positionCode)
        {
            return (VoxelType)(_voxels[positionCode] & 0x7f);
        }

        public VoxelType Get(SharpDX.Int3 position)
        {
            return Get(EncodePosition(position));
        }

        public void Set(Int32 positionCode, VoxelType type, bool living)
        {
            _voxels[positionCode] = (byte)((living ? 0x80 : 0) | (int)type);
        }

        /// <summary>
        /// Test if a voxel has 6 filled neighbours.
        /// </summary>
        /// <param name="positionCode"></param>
        /// <returns>True if voxel is not visible.</returns>
        public bool IsOccluded(Int32 positionCode)
        {
            return (_voxels[positionCode - 1] != 0) && (_voxels[positionCode + 1] != 0)
                && (_voxels[positionCode - SizeX] != 0) && (_voxels[positionCode + SizeX] != 0)
                && (_voxels[positionCode - SizeX * SizeY] != 0) && (_voxels[positionCode + SizeX * SizeY] != 0);
        }

        /// <returns>True if voxel is not set.</returns>
        public bool IsEmpty(Int32 positionCode)
        {
            return _voxels[positionCode] == 0;
        }

        public bool IsLiving(Int32 positionCode)
        {
            return (_voxels[positionCode] & 0x80) == 0x80;
        }


        /// <summary>
        /// Create a relatively flat ground.
        /// </summary>
        /// <param name="rand"></param>
        private void GeneratePlainLevel(ref Random rand)
        {
            // Fill half to test
            Int32 maxHeight = SizeY / 2;
            for( int z=0; z<SizeZ; ++z )
                for (int x = 0; x < SizeX; ++x)
                {
                    int height =  maxHeight + rand.Next(SizeY / 2 - 1) - SizeY / 4;
                    for (int y = 0; y < height; ++y)
                        _voxels[EncodePosition(x, y, z)] = (int)VoxelType.GROUND;
                }
        }

		// TODO: find a better name
		public static int getGoodVoxels()
        { 
            return 100;
        }

    }
}
