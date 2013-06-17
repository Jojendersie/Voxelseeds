using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    class ValueNoise
    {
        const int WHITE_NOISE_SIZE_XZ = 64;
        const int WHITE_NOISE_SIZE_Z = 32;
        float[,,] whiteNoise;
        int lowOctave;
        int hightOctave;

        public ValueNoise(ref System.Random rand, int low, int height)
        {
            whiteNoise = new float[WHITE_NOISE_SIZE_XZ, WHITE_NOISE_SIZE_XZ, WHITE_NOISE_SIZE_Z];
            for (int y = 0; y < WHITE_NOISE_SIZE_XZ; ++y)
                for (int x = 0; x < WHITE_NOISE_SIZE_XZ; ++x)
                    for (int z = 0; z < WHITE_NOISE_SIZE_Z; ++z)
                    {
                        whiteNoise[y, x, z] = rand.Next(1000)/1000.0f;
                    }

            lowOctave = low;
            hightOctave = height;
        }

        private float lrp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        private float Sample(float x, float y)
        {
            int ix = (int)Math.Floor((double)x);
            int iy = (int)Math.Floor((double)y);
            float u = x - ix;
            float v = y - iy;

            return lrp(lrp(whiteNoise[ix % WHITE_NOISE_SIZE_XZ, iy % WHITE_NOISE_SIZE_XZ, 0], whiteNoise[(ix + 1) % WHITE_NOISE_SIZE_XZ, iy % WHITE_NOISE_SIZE_XZ, 0], u),
                    lrp(whiteNoise[ix % WHITE_NOISE_SIZE_XZ, (iy + 1) % WHITE_NOISE_SIZE_XZ, 0], whiteNoise[(ix + 1) % WHITE_NOISE_SIZE_XZ, (iy + 1) % WHITE_NOISE_SIZE_XZ, 0], u), v);
        }
        
        private float Sample(float x, float y, float z)
        {
            int ix = (int)Math.Floor((double)x);
            int iy = (int)Math.Floor((double)y);
            int iz = (int)Math.Floor((double)z);
            float u = x - ix;
            float v = y - iy;
            float w = z - iz;

            int z1 = iz % WHITE_NOISE_SIZE_Z;
            int z2 = (iz + 1) % WHITE_NOISE_SIZE_Z;

            return lrp(lrp(lrp(whiteNoise[ix % WHITE_NOISE_SIZE_XZ, iy % WHITE_NOISE_SIZE_XZ, z1], whiteNoise[(ix + 1) % WHITE_NOISE_SIZE_XZ, iy % WHITE_NOISE_SIZE_XZ, z1], u),
                           lrp(whiteNoise[ix % WHITE_NOISE_SIZE_XZ, (iy + 1) % WHITE_NOISE_SIZE_XZ, z1], whiteNoise[(ix + 1) % WHITE_NOISE_SIZE_XZ, (iy + 1) % WHITE_NOISE_SIZE_XZ, z1], u), v),

                       lrp(lrp(whiteNoise[ix % WHITE_NOISE_SIZE_XZ, iy % WHITE_NOISE_SIZE_XZ, z2], whiteNoise[(ix + 1) % WHITE_NOISE_SIZE_XZ, iy % WHITE_NOISE_SIZE_XZ, z2], u),
                           lrp(whiteNoise[ix % WHITE_NOISE_SIZE_XZ, (iy + 1) % WHITE_NOISE_SIZE_XZ, z2], whiteNoise[(ix + 1) % WHITE_NOISE_SIZE_XZ, (iy + 1) % WHITE_NOISE_SIZE_XZ, z2], u), v), w);
        }
        
    
        /// <summary>
        /// 2D value noise sampler
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>A Random height in [0,1]</returns>
        public float Get(int x, int y)
        {
            const float PERSISTENCE = 0.5f;

            float fRes = 0.0f;
	        float fAmplitude = 1.0f;
            float fFrequence = (float)(1 << lowOctave) * 0.01f;
            for (int i = lowOctave; i <= hightOctave; ++i)
	        {
                fRes += fAmplitude * Sample(x * fFrequence, y * fFrequence);
		        fAmplitude *= PERSISTENCE;
		        fFrequence *= 2.0f;
	        }

	        // Transform to [0,1]
	        return fRes*(1.0f-PERSISTENCE)/(1.0f-fAmplitude);
        }

        public float Get(int x, int y, int z)
        {
            const float PERSISTENCE = 0.5f;

            float fRes = 0.0f;
            float fAmplitude = 1.0f;
            float fFrequence = (float)(1 << lowOctave) * 0.01f;
            for (int i = lowOctave; i <= hightOctave; ++i)
            {
                fRes += fAmplitude * Sample(x * fFrequence, y * fFrequence, z * fFrequence);
                fAmplitude *= PERSISTENCE;
                fFrequence *= 2.0f;
            }

            // Transform to [0,1]
            return fRes * (1.0f - PERSISTENCE) / (1.0f - fAmplitude);
        }
    }
}
