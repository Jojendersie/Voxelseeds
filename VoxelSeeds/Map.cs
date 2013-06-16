using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SharpDX;

namespace VoxelSeeds
{
    enum LevelType
    {
        PLAIN,
        BUBBLE,
        MOUNTAINS,
        CANYON,
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
        public Map(int sizeX, int sizeY, int sizeZ, LevelType lvlType, int seed, float heightoffset)
        {
            _voxels = new byte[sizeX*sizeY*sizeZ];
            _sizeX = sizeX;
            _sizeY = sizeY;
            _sizeZ = sizeZ;

            // Create the ground
            Random rand = new Random(seed);
            switch (lvlType)
            {
                case LevelType.PLAIN:
                    GeneratePlainLevel(ref rand, heightoffset);
                    break;
                case LevelType.BUBBLE:
                    GenerateBubbleLevel(ref rand, heightoffset);
                    break;
                case LevelType.MOUNTAINS:
                    GenerateMountainsLevel(ref rand, heightoffset);
                    break;
                case LevelType.CANYON:
                    GenerateCanoynsLevel(ref rand, heightoffset);
                    break;
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
            Int3 p;
            p = DecodePosition(positionCode);
            _voxels[positionCode] = (byte)((living ? 0x80 : 0) | (int)type);
        }

        /// <summary>
        /// Test if a voxel has 6 filled neighbours.
        /// </summary>
        /// <param name="positionCode"></param>
        /// <returns>True if voxel is not visible.</returns>
        public bool IsOccluded(Int32 positionCode)
        {
            // Pixels on the boundary are never occluded.
            var pos = DecodePosition(positionCode);
            if (!IsInside(pos.X, pos.Y, pos.Z)) return false;
            return (_voxels[positionCode - 1] != 0) && (_voxels[positionCode + 1] != 0)
                && (_voxels[positionCode - SizeX] != 0) && (_voxels[positionCode + SizeX] != 0)
                && (_voxels[positionCode - SizeX * SizeY] != 0) && (_voxels[positionCode + SizeX * SizeY] != 0);
        }

        /// <summary>
        /// Finds the voxel with the largest y coordinate.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns>Reports the position of the highest voxel or -1.</returns>
        public int GetHeighest(int x, int z)
        {
            Int32 pos = EncodePosition(x, SizeY - 1, z);
            for (int y = SizeY - 1; y > 0; --y, pos-=SizeX)
            {
                if (Get(pos) != VoxelType.EMPTY)
                    return y;
            }
            return -1;
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
        /// Testing if a voxel can be set at a certain position.
        /// 
        /// This method preserves a border of one which do not count as inside.
        /// </summary>
        /// <returns>True if the position is truly inside and not on the boundary.</returns>
        public bool IsInside(int x, int y, int z)
        {
            return x >= 1 && x < SizeX-1 && y >= 1 && y < SizeY-1 && z >= 1 && z < SizeZ-1;
        }

        public bool PickPosition(Ray pickingRay, out Int3 pickedPosition)
        {
            // context knowledge:
            // - map is centered on x and z
            // - cubes size is 0.5 lin every direction               

            Vector3 currentPosition = pickingRay.Position;
            currentPosition.X += (float)SizeX / 2;
            currentPosition.Z += (float)SizeZ / 2;

            float startOffset = Math.Max(Math.Max((MathUtil.Clamp(currentPosition.X, 0.5f, SizeX - 0.5f) - currentPosition.X) / pickingRay.Direction.X,
                                                  (MathUtil.Clamp(currentPosition.Y, 0.5f, SizeY - 0.5f) - currentPosition.Y) / pickingRay.Direction.Y),
                                                  (MathUtil.Clamp(currentPosition.Z, 0.5f, SizeZ - 0.5f) - currentPosition.Z) / pickingRay.Direction.Z);
            if (startOffset > 0.000001f)
                startOffset += 0.01f; 
            currentPosition += pickingRay.Direction * startOffset;

            Vector3 step = pickingRay.Direction * 0.1f;


            pickedPosition = new Int3((int)(currentPosition.X + 0.5f),
                                     (int)(currentPosition.Y + 0.5f),
                                     (int)(currentPosition.Z + 0.5f));

            while (currentPosition.X > -0.5f &&
                  currentPosition.Y > -0.5f &&
                  currentPosition.Z > -0.5f &&
                  currentPosition.X < SizeX - 0.5f &&
                  currentPosition.Y < SizeY - 0.5f &&
                  currentPosition.Z < SizeZ - 0.5f)
            {
                Int3 currentVoxel = new Int3((int)(currentPosition.X + 0.5f),
                                            (int)(currentPosition.Y + 0.5f),
                                            (int)(currentPosition.Z + 0.5f));
                if (Get(currentVoxel) != VoxelType.EMPTY)
                    return true;

                pickedPosition = currentVoxel;
                currentPosition += step;
            }

            return false;
        }

        /// <summary>
        /// Create a relatively flat ground.
        /// </summary>
        /// <param name="rand"></param>
        private void GeneratePlainLevel(ref Random rand, float heightOffset = 0.2f)
        {
            ValueNoise noise = new ValueNoise(ref rand, 3, 8);
            // Fill half to test
            Int32 maxHeight = Math.Min( 8, SizeY-1 );
            Parallel.For( 0, SizeZ, (z) => {
                for (int x = 0; x < SizeX; ++x)
                {
                    float radialHeight = 0.02f * (float)Math.Sqrt((x - SizeX / 2) * (x - SizeX / 2) + (z - SizeZ / 2) * (z - SizeZ / 2));
                    int height = (int)(maxHeight * (noise.Get(x, z) - radialHeight + heightOffset));
                    for (int y = 0; y < height; ++y)
                        _voxels[EncodePosition(x, y, z)] = (int)VoxelType.GROUND;
                }
            });
        }

        private void GenerateBubbleLevel(ref Random rand, float heightOffset)
        {
            ValueNoise noise = new ValueNoise(ref rand, 3, 8);
            int sizeYscaled = SizeY / 3;
            // Fill half to test
            Parallel.For( 0, SizeZ, (z) => {
                for (int x = 0; x < SizeX; ++x)
                    for (int d = 0; d < SizeY; ++d)
                    {
                        float radialHeight = 8.0f / SizeX * (float)Math.Sqrt((x - SizeX / 2) * (x - SizeX / 2) + (z - SizeZ / 2) * (z - SizeZ / 2));
                        float value = SizeY * (noise.Get(x, z, d) - noise.Get(x, z)) / radialHeight - heightOffset - radialHeight - Math.Max(0, d - sizeYscaled)*2;
                        // for (int y = 0; y < height; ++y)
                        if (value > 0)
                            _voxels[EncodePosition(x, d, z)] = (int)VoxelType.GROUND;
                    }
            });

            Rockyfy();
        }

        private void GenerateMountainsLevel(ref Random rand, float heightOffset)
        {
            ValueNoise noise = new ValueNoise(ref rand, 3, 8);
            int sizeYscaled = SizeY / 3;
            // Fill half to test
            Parallel.For( 0, SizeZ, (z) => {
                for (int x = 0; x < SizeX; ++x)
                    for (int d = 0; d < SizeY; ++d)
                    {
                        float radialHeight = Math.Max(0.0f, (float)Math.Sqrt((x - SizeX / 2) * (x - SizeX / 2) + (z - SizeZ / 2) * (z - SizeZ / 2)) - SizeX * 3 / 8) * 2.0f;
                        float value = (noise.Get(x, z, d) * 3 - heightOffset + noise.Get(x / 2, z / 2)) * sizeYscaled - d;
                        value -= radialHeight;
                        if (value > 0)
                            _voxels[EncodePosition(x, d, z)] = (int)VoxelType.GROUND;
                    }
            });

            Rockyfy();
        }

        private void GenerateCanoynsLevel(ref Random rand, float heightOffset)
        {
            ValueNoise noise = new ValueNoise(ref rand, 3, 8);
            int sizeYscaled = SizeY / 3;
            // Fill half to test
            Parallel.For( 0, SizeZ, (z) => {
                for (int x = 0; x < SizeX; ++x)
                {
                    _voxels[EncodePosition(x, 0, z)] = (byte)VoxelType.GROUND;
                    for (int d = 1; d < sizeYscaled; ++d)
                    {
                        //float radialHeight = 0.08f * (float)Math.Sqrt((x - SizeX / 2) * (x - SizeX / 2) + (z - SizeZ / 2) * (z - SizeZ / 2));
                        //float value = SizeY * (noise.Get(x, z, d) + noise.Get(x, z) - d*3 / sizeYscaled);// -heightOffset;// -radialHeight;
                        float value = noise.Get(x / 2, z / 2, d / 5) - 0.5f;
                        if (value > 0)
                            _voxels[EncodePosition(x, d, z)] = (byte)VoxelType.ROCK;
                    }
                    for (int d = sizeYscaled; d < sizeYscaled+5; ++d)
                    {
                        float value = noise.Get(x / 2, z / 2, d / 5) - 0.6f;
                        if (value > 0)
                            _voxels[EncodePosition(x, d, z)] = (byte)VoxelType.ROCK;
                    }
                }
            });
        }

        private void Rockyfy()
        {
            Parallel.For( 0, SizeZ, (z) => {
                for (int x = 0; x < SizeX; ++x)
                {
                    int streak = 0;
                    for (int d = SizeY-1; d >= 0; --d)
                    {
                        Int32 pos = EncodePosition(x, d, z);
                        byte value = Sample(pos);
                        if (value != 0)
                        {
                            if (streak > 1) _voxels[pos] = (int)VoxelType.ROCK;
                            ++streak;
                        }
                        else streak = 0;
                    }
                }
            });
        }
    }
}
