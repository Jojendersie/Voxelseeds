using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    class ValueNoise
    {
        const int WHITE_NOISE_SIZE = 128;
        float[,] whiteNoise;
        int lowOctave;
        int hightOctave;

        public ValueNoise(ref Random rand, int low, int height)
        {
            whiteNoise = new float[WHITE_NOISE_SIZE, WHITE_NOISE_SIZE];
            for (int y = 0; y < WHITE_NOISE_SIZE; ++y)
                for (int x = 0; x < WHITE_NOISE_SIZE; ++x)
                {
                    whiteNoise[y, x] = rand.Next(1000)/1000.0f;
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

            return lrp(lrp(whiteNoise[ix % WHITE_NOISE_SIZE, iy % WHITE_NOISE_SIZE], whiteNoise[(ix + 1) % WHITE_NOISE_SIZE, iy % WHITE_NOISE_SIZE], u),
                    lrp(whiteNoise[ix % WHITE_NOISE_SIZE, (iy + 1) % WHITE_NOISE_SIZE], whiteNoise[(ix + 1) % WHITE_NOISE_SIZE, (iy + 1) % WHITE_NOISE_SIZE], u), v);
        }

        /// <summary>
        /// 2D value noise sampler
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>A random height in [0,1]</returns>
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
    }
}
