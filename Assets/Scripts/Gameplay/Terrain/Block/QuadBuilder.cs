using UnityEngine;

namespace MyGame.Gameplay.Terrain.Block
{
    public sealed class QuadBuilder
    {
        private readonly AllBlocks _allBlocks;
        private readonly Chunk _chunk;
        private readonly ChunkMesh _chunkMesh;

        public QuadBuilder(AllBlocks allBlocks, Chunk chunk, ChunkMesh chunkMesh)
        {
            _allBlocks = allBlocks;
            _chunk = chunk;
            _chunkMesh = chunkMesh;
        }

        public void BuildLeftQuad(int x, int y, int z)
        {
            if (x >= Settings.ChunkSize.x) return;
            int blockId = _chunk.BlockIds[x, y, z];
            if (blockId == 0 || _chunk.MiniBlocksValue[x, y, z] > 0 && _chunk.MiniBlocksValue[x, y, z] < 255) return;
            if (x == 0 && _chunk.LeftChunk.MiniBlocksValue[Settings.ChunkSize.x - 1, y, z] < 255 || x > 0 && _chunk.MiniBlocksValue[x - 1, y, z] < 255)
            {
                _chunkMesh.PositionQuads.Add(new(x - 1, y, z));
                _chunkMesh.OffsetQuads.Add(Vector3Int.right);
                Build(new Vector3[]
                {
                    new Vector3(x, y, z + 1),
                    new Vector3(x, y + 1, z + 1),
                    new Vector3(x, y + 1, z),
                    new Vector3(x, y, z)
                }, Vector3.left, blockId);
            }
        }

        public void BuildRightQuad(int x, int y, int z)
        {
            if (x < 0) return;
            int blockId = _chunk.BlockIds[x, y, z];
            if (blockId == 0 || _chunk.MiniBlocksValue[x, y, z] > 0 && _chunk.MiniBlocksValue[x, y, z] < 255) return;
            if (x == Settings.ChunkSize.x - 1 && _chunk.RightChunk.MiniBlocksValue[0, y, z] < 255 || x < Settings.ChunkSize.x - 1 && _chunk.MiniBlocksValue[x + 1, y, z] < 255)
            {
                _chunkMesh.PositionQuads.Add(new(x + 1, y, z));
                _chunkMesh.OffsetQuads.Add(Vector3Int.left);
                Build(new Vector3[]
                {
                    new Vector3(x + 1, y, z),
                    new Vector3(x + 1, y + 1, z),
                    new Vector3(x + 1, y + 1, z + 1),
                    new Vector3(x + 1, y, z + 1)
                }, Vector3.right, blockId);
            }
        }

        public void BuildBackQuad(int x, int y, int z)
        {
            if (z >= Settings.ChunkSize.z) return;
            int blockId = _chunk.BlockIds[x, y, z];
            if (blockId == 0 || _chunk.MiniBlocksValue[x, y, z] > 0 && _chunk.MiniBlocksValue[x, y, z] < 255) return;
            if (z == 0 && _chunk.BackChunk.MiniBlocksValue[x, y, Settings.ChunkSize.z - 1] < 255 || z > 0 && _chunk.MiniBlocksValue[x, y, z - 1] < 255)
            {
                _chunkMesh.PositionQuads.Add(new(x, y, z - 1));
                _chunkMesh.OffsetQuads.Add(Vector3Int.forward);
                Build(new Vector3[]
                {
                    new Vector3(x, y, z),
                    new Vector3(x, y + 1, z),
                    new Vector3(x + 1, y + 1, z),
                    new Vector3(x + 1, y, z)
                }, Vector3.back, blockId);
            }
        }

        public void BuildForwardQuad(int x, int y, int z)
        {
            if (z < 0) return;
            int blockId = _chunk.BlockIds[x, y, z];
            if (blockId == 0 || _chunk.MiniBlocksValue[x, y, z] > 0 && _chunk.MiniBlocksValue[x, y, z] < 255) return;
            if (z == Settings.ChunkSize.z - 1 && _chunk.ForwardChunk.MiniBlocksValue[x, y, 0] < 255 || z < Settings.ChunkSize.z - 1 && _chunk.MiniBlocksValue[x, y, z + 1] < 255)
            {
                _chunkMesh.PositionQuads.Add(new(x, y, z + 1));
                _chunkMesh.OffsetQuads.Add(Vector3Int.back);
                Build(new Vector3[]
                {
                    new Vector3(x + 1, y, z + 1),
                    new Vector3(x + 1, y + 1, z + 1),
                    new Vector3(x, y + 1, z + 1),
                    new Vector3(x, y, z + 1)
                }, Vector3.forward, blockId);
            }
        }

        public void BuildDownQuad(int x, int y, int z)
        {
            if (y >= Settings.ChunkSize.y) return;
            int blockId = _chunk.BlockIds[x, y, z];
            if (blockId == 0 || _chunk.MiniBlocksValue[x, y, z] > 255 && _chunk.MiniBlocksValue[x, y, z] < 255) return;
            if (y != 0 && _chunk.MiniBlocksValue[x, y - 1, z] < 255)
            {
                _chunkMesh.PositionQuads.Add(new(x, y - 1, z));
                _chunkMesh.OffsetQuads.Add(Vector3Int.up);
                Build(new Vector3[]
                {
                    new Vector3(x, y, z + 1),
                    new Vector3(x, y, z),
                    new Vector3(x + 1, y, z),
                    new Vector3(x + 1, y, z + 1)
                }, Vector3.down, blockId);
            }
        }

        public void BuildUpQuad(int x, int y, int z)
        {
            if (y < 0) return;
            int blockId = _chunk.BlockIds[x, y, z];
            if (blockId == 0 || _chunk.MiniBlocksValue[x, y, z] > 0 && _chunk.MiniBlocksValue[x, y, z] < 255) return;
            if (y == Settings.ChunkSize.y - 1 || _chunk.MiniBlocksValue[x, y + 1, z] < 255)
            {
                _chunkMesh.PositionQuads.Add(new(x, y + 1, z));
                _chunkMesh.OffsetQuads.Add(Vector3Int.down);
                Build(new Vector3[]
                {
                    new Vector3(x, y + 1, z),
                    new Vector3(x, y + 1, z + 1),
                    new Vector3(x + 1, y + 1, z + 1),
                    new Vector3(x + 1, y + 1, z)
                }, Vector3.up, blockId);
            }
        }

        private void Build(Vector3[] vertices, Vector3 side, int blockId)
        {
            _chunkMesh.Vertices.AddRange(vertices);
            CreateTriangles();
            CreateUvs(side, blockId - 1);
            CreateUvs2();
            CreateUvs3();
        }

        private void CreateTriangles()
        {
            _chunkMesh.Triangles.AddRange(new int[]
            {
                _chunkMesh.Vertices.Count - 4,
                _chunkMesh.Vertices.Count - 3,
                _chunkMesh.Vertices.Count - 2,
                _chunkMesh.Vertices.Count - 4,
                _chunkMesh.Vertices.Count - 2,
                _chunkMesh.Vertices.Count - 1
            });
        }

        private void CreateUvs(Vector3 side, int blockId)
        {
            Vector2Int position;
            if (side == Vector3.up)
                position = _allBlocks.GetBlock(blockId).upTexturePosition;
            else if (side == Vector3.down)
                position = _allBlocks.GetBlock(blockId).downTexturePosition;
            else
                position = _allBlocks.GetBlock(blockId).sideTexturePosition;

            _chunkMesh.Uvs.AddRange(new Vector2[]
            {
                    new Vector2(position.x / _allBlocks.TextureSize.x, position.y / _allBlocks.TextureSize.y),
                    new Vector2(position.x / _allBlocks.TextureSize.x, (position.y + 1) / _allBlocks.TextureSize.y),
                    new Vector2((position.x + 1) / _allBlocks.TextureSize.x, (position.y + 1) / _allBlocks.TextureSize.y),
                    new Vector2((position.x + 1) / _allBlocks.TextureSize.x, position.y / _allBlocks.TextureSize.y)
            });
        }

        private void CreateUvs2()
        {
            _chunkMesh.Uvs2.AddRange(new Vector2[]
                {
                    Vector2.zero,
                    Vector2.zero,
                    Vector2.zero,
                    Vector2.zero
                });
        }

        private void CreateUvs3()
        {
            _chunkMesh.Uvs3.AddRange(new Vector2[]
                {
                    Vector2.zero,
                    Vector2.zero,
                    Vector2.zero,
                    Vector2.zero
                });
        }
    }
}