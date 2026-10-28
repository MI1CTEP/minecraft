using UnityEngine;

namespace MyGame.Gameplay.Terrain
{
    public sealed class TerrainGenerator
    {
        public int[,,] GetBlockIds(Vector2Int chunkId, out byte[,,] miniBlocksValue)
        {
            int[,,] ids = new int[Settings.ChunkSize.x, Settings.ChunkSize.y, Settings.ChunkSize.z];
            miniBlocksValue = new byte[Settings.ChunkSize.x, Settings.ChunkSize.y, Settings.ChunkSize.z];

            for (int z = 0; z < Settings.ChunkSize.z; z++)
            {
                for (int x = 0; x < Settings.ChunkSize.x; x++)
                {
                    int posX = x + chunkId.x * Settings.ChunkSize.x;
                    int posZ = z + chunkId.y * Settings.ChunkSize.z;
                    float noiseValue = Mathf.PerlinNoise(posX * Settings.NoiseScale, posZ * Settings.NoiseScale);
                    int height = Settings.MinNoiseHeight + Mathf.FloorToInt(noiseValue * (Settings.MaxNoiseHeight - Settings.MinNoiseHeight));

                    for (int y = Settings.ChunkSize.y - 1; y >= 0; y--)
                    {
                        if (y > height)
                        {
                            ids[x, y, z] = 0;
                            miniBlocksValue[x, y, z] = 0;
                        }
                        else if (ids[x, y + 1, z] == 0)
                        {
                            ids[x, y, z] = 1;
                            miniBlocksValue[x, y, z] = 255;
                        }
                        else
                        {
                            ids[x, y, z] = 2;
                            miniBlocksValue[x, y, z] = 255;
                        }
                    }
                }
            }
            return ids;
        }
    }
}