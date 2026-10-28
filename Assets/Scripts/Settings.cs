using UnityEngine;

namespace MyGame
{
    public static class Settings
    {
        public static Vector3Int ChunkSize { get; } = new(16, 128, 16);
        public static int WorldRadius { get; } = 4;
        public static float NoiseScale { get; } = 0.03f;
        public static int MinNoiseHeight { get; } = 80;
        public static int MaxNoiseHeight { get; } = 100;
    }
}