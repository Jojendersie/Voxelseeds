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
            Debug.Assert((0 <= y) && (x < _sizeY));
            Debug.Assert((0 <= z) && (x < _sizeZ));
            return (Int32)(x+SizeX*(y+SizeY*z));
        }

        /// <summary>
        /// Calculates the original position from a position code.
        /// 
        /// This direction is slower than EncodePosition.
        /// </summary>
        /// <param name="positionCode"></param>
        /// <param name="x">Output of position x</param>
        /// <param name="y">Output of position y</param>
        /// <param name="z">Output of position z</param>
        public void DecodePosition(Int32 positionCode, out int x, out int y, out int z)
        {
            x = positionCode % SizeX;
            y = (positionCode / SizeX) % SizeY;
            z = positionCode / (SizeX * SizeY);
        }

        public byte Sample(int x, int y, int z)
        {
            return _voxels[EncodePosition(x, y, z)];
        }


        public byte Sample(Int32 positionCode)
        {
            return _voxels[positionCode];
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
                    for (int y = 0; y < maxHeight + rand.Next(4)-2; ++y)
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
