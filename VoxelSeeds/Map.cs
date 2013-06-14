using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
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

        public byte Sample(int x, int y, int z)
        {
            throw new NotImplementedException();
        }
    }
}
