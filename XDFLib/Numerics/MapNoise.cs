using System;
using System.Numerics;
using System.Threading.Tasks;
using XDFLib.XRandom;

namespace XDFLib.Numerics
{
    public interface INoiseSetup
    {
        float Scale { get; } // default 1.0f, controls the zoom level of the noise
        int Octaves { get; } // default 1, number of layers of noise to combine
        float Persistence { get; } // default 0.5f, controls the amplitude of each octave
        float Lacunarity { get; } // default 2.0f, controls the frequency of each octave
    }

    public static class MapNoise
    {
        public static void Generate<T>(float[,] result, Vector2 mapSize, int seed, Perlin perlin, T noiseSetup, Vector2 offset, float time)
            where T : INoiseSetup
        {
            Generate(result, mapSize, seed, perlin, noiseSetup.Scale, noiseSetup.Octaves, noiseSetup.Persistence, noiseSetup.Lacunarity, offset, time);
        }

        public static void Generate<T>(float[,] result, Vector2 mapSize, int seed, T noiseSetup, Vector2 offset, float time)
            where T : INoiseSetup
        {
            Generate(result, mapSize, seed, noiseSetup.Scale, noiseSetup.Octaves, noiseSetup.Persistence, noiseSetup.Lacunarity, offset, time);
        }

        public static void Generate(float[,] result, Vector2 mapSize, int seed, float noiseScale, int octaves,
            float persistance, float lacunarity, Vector2 offset, float time)
        {
            var perlin = new Perlin(seed);
            Generate(result, mapSize, seed, perlin, noiseScale, octaves, persistance, lacunarity, offset, time);
        }

        public static void Generate(float[,] result, Vector2 mapSize, int seed, Perlin perlin, float noiseScale, int octaves,
            float persistance, float lacunarity, Vector2 offset, float time)
        {
            var resX = result.GetLength(0);
            var resY = result.GetLength(1);

            var centerX = resX / 2f;
            var centerY = resY / 2f;

            var freq = Vector2.One;
            freq.X = MathF.Max(mapSize.X / noiseScale, 0.0001f);
            freq.Y = MathF.Max(mapSize.Y / noiseScale, 0.0001f);

            offset *= freq;
            float maxPossibleHeight = 0;
            float amplitude = 1;

            Vector2[] octaveOffset = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                var randX = SplitMix32.Random(ref seed, -0.5f, 0.5f);
                float octaveOffsetX = (randX * 100000) + offset.X;

                var randY = SplitMix32.Random(ref seed, -0.5f, 0.5f);
                float octaveOffsetY = (randY * 100000) + offset.Y;

                octaveOffset[i] = new Vector2(octaveOffsetX, octaveOffsetY);
                offset *= lacunarity;
                maxPossibleHeight += amplitude;
                amplitude *= persistance;
            }

            float normalizeFactor = 1f / (maxPossibleHeight * XMath.Sqrt2);
            Parallel.For(0, resX * resY, (index) =>
            {
                var y = index / resX;
                var x = index - (y * resX);

                float currAmplitude = 1;
                float frequencyFactor = 1;
                float height = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float rateX = (x - centerX) / (resX - 1);
                    float sampleX = rateX * freq.X * frequencyFactor - octaveOffset[i].X;
                    float rateY = (y - centerY) / (resY - 1);
                    float sampleY = rateY * freq.Y * frequencyFactor + octaveOffset[i].Y;

                    float perlinValue = perlin.Noise(sampleX, sampleY, time) * 2 - 1;

                    height += perlinValue * currAmplitude;

                    currAmplitude *= persistance;
                    frequencyFactor *= lacunarity;
                }

                result[x, y] = (height + 1) * normalizeFactor;
            });
        }

    }
}
