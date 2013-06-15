using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VoxelSeeds
{
    /// <summary>
    /// The map is a 3D array of byte values.
    /// 
    /// Later the map gets a loader or generator for the different levels.
    /// 
    /// The most significant bit encodes if the voxel is living or dead.
    /// </summary>
    class Map
    {
        public Map(int sizeX, int sizeY, int sizeZ)
        {
            _voxels = new byte[sizeX*sizeY*sizeZ];
            _sizeX = sizeX;
            _sizeY = sizeY;
            _sizeZ = sizeZ;
        }

        public int SizeX { get { return _sizeX; } }
        public int SizeY { get { return _sizeY; } }
        public int SizeZ { get { return _sizeZ; } }

        // The pure data of the whole volume
        byte[] _voxels;
        readonly int _sizeX;
        readonly int _sizeY;
        readonly int _sizeZ;

        /// <summary>
        /// Returns a unique number for each position.
        /// </summary>
        /// <param name="x">Voxel postion x</param>
        /// <param name="y">Voxel postion y</param>
        /// <param name="z">Voxel postion z</param>
        /// <returns>The PositionCode containting every information for the
        /// position.</returns>
        public int EncodePosition(int x, int y, int z)
        {
            Debug.Assert((0 <= x) && (x < _sizeX));
            Debug.Assert((0 <= y) && (x < _sizeY));
            Debug.Assert((0 <= z) && (x < _sizeZ));
            return x+SizeX*(y+SizeY*z);
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
        public void DecodePosition(int positionCode, out int x, out int y, out int z)
        {
            x = positionCode % SizeX;
            y = (positionCode / SizeX) % SizeY;
            z = positionCode / (SizeX * SizeY);
        }

        public byte Sample(int x, int y, int z)
        {
            return _voxels[EncodePosition(x, y, z)];
        }
    }
}
