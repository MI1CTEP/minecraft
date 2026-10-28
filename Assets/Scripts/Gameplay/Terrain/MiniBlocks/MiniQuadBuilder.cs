using UnityEngine;

namespace MyGame.Gameplay.Terrain.MiniBlock
{
    public sealed class MiniQuadBuilder
    {
        private readonly AllBlocks _allBlocks;
        private readonly Chunk _chunk;
        private readonly ChunkMesh _chunkMesh;

        public MiniQuadBuilder(AllBlocks allBlocks, Chunk chunk, ChunkMesh chunkMesh)
        {
            _allBlocks = allBlocks;
            _chunk = chunk;
            _chunkMesh = chunkMesh;
        }

        public void Build(Vector3Int position, Vector3 positionInBlock)
        {
            BuildLeftQuad(position.x, position.y, position.z, positionInBlock);
            BuildRightQuad(position.x, position.y, position.z, positionInBlock);
            BuildBackQuad(position.x, position.y, position.z, positionInBlock);
            BuildForwardQuad(position.x, position.y, position.z, positionInBlock);
            BuildDownQuad(position.x, position.y, position.z, positionInBlock);
            BuildUpQuad(position.x, position.y, position.z, positionInBlock);
        }

        private void BuildLeftQuad(int x, int y, int z, Vector3 positionInBlock)
        {
            if (x >= Settings.ChunkSize.x) return;
            int blockId = _chunk.BlockIds[x, y, z];
            if (blockId == 0) return;
            _chunkMesh.PositionQuads.Add(new(x - 1, y, z));
            _chunkMesh.OffsetQuads.Add(Vector3Int.right);
            Build(new Vector3[]
            {
                    new Vector3(x, y, z + 0.5f) + positionInBlock,
                    new Vector3(x, y + 0.5f, z + 0.5f) + positionInBlock,
                    new Vector3(x, y + 0.5f, z) + positionInBlock,
                    new Vector3(x, y, z) + positionInBlock
            }, Vector3.left, blockId, positionInBlock);
        }

        private void BuildRightQuad(int x, int y, int z, Vector3 positionInBlock)
        {
            if (x < 0) return;
            int blockId = _chunk.BlockIds[x, y, z];
            if (blockId == 0) return;
            _chunkMesh.PositionQuads.Add(new(x + 1, y, z));
            _chunkMesh.OffsetQuads.Add(Vector3Int.left);
            Build(new Vector3[]
            {
                    new Vector3(x + 0.5f, y, z) + positionInBlock,
                    new Vector3(x + 0.5f, y + 0.5f, z) + positionInBlock,
                    new Vector3(x + 0.5f, y + 0.5f, z + 0.5f) + positionInBlock,
                    new Vector3(x + 0.5f, y, z + 0.5f) + positionInBlock
            }, Vector3.right, blockId, positionInBlock);
        }

        private void BuildBackQuad(int x, int y, int z, Vector3 positionInBlock)
        {
            if (z >= Settings.ChunkSize.z) return;
            int blockId = _chunk.BlockIds[x, y, z];
            if (blockId == 0) return;
            _chunkMesh.PositionQuads.Add(new(x, y, z - 1));
            _chunkMesh.OffsetQuads.Add(Vector3Int.forward);
            Build(new Vector3[]
            {
                    new Vector3(x, y, z) + positionInBlock,
                    new Vector3(x, y + 0.5f, z) + positionInBlock,
                    new Vector3(x + 0.5f, y + 0.5f, z) + positionInBlock,
                    new Vector3(x + 0.5f, y, z) + positionInBlock
            }, Vector3.back, blockId, positionInBlock);
        }

        private void BuildForwardQuad(int x, int y, int z, Vector3 positionInBlock)
        {
            if (z < 0) return;
            int blockId = _chunk.BlockIds[x, y, z];
            if (blockId == 0) return;
            _chunkMesh.PositionQuads.Add(new(x, y, z + 1));
            _chunkMesh.OffsetQuads.Add(Vector3Int.back);
            Build(new Vector3[]
            {
                    new Vector3(x + 0.5f, y, z + 0.5f) + positionInBlock,
                    new Vector3(x + 0.5f, y + 0.5f, z + 0.5f) + positionInBlock,
                    new Vector3(x, y + 0.5f, z + 0.5f) + positionInBlock,
                    new Vector3(x, y, z + 0.5f )+ positionInBlock
            }, Vector3.forward, blockId, positionInBlock);
        }

        private void BuildDownQuad(int x, int y, int z, Vector3 positionInBlock)
        {
            if (y >= Settings.ChunkSize.y) return;
            int blockId = _chunk.BlockIds[x, y, z];
            if (blockId == 0) return;
            _chunkMesh.PositionQuads.Add(new(x, y - 1, z));
            _chunkMesh.OffsetQuads.Add(Vector3Int.up);
            Build(new Vector3[]
            {
                    new Vector3(x, y, z + 0.5f) + positionInBlock,
                    new Vector3(x, y, z) + positionInBlock,
                    new Vector3(x + 0.5f, y, z) + positionInBlock,
                    new Vector3(x + 0.5f, y, z + 0.5f) + positionInBlock
            }, Vector3.down, blockId, positionInBlock);
        }

        private void BuildUpQuad(int x, int y, int z, Vector3 positionInBlock)
        {
            if (y < 0) return;
            int blockId = _chunk.BlockIds[x, y, z];
            if (blockId == 0) return;
            _chunkMesh.PositionQuads.Add(new(x, y + 1, z));
            _chunkMesh.OffsetQuads.Add(Vector3Int.down);
            Build(new Vector3[]
            {
                    new Vector3(x, y + 0.5f, z) + positionInBlock,
                    new Vector3(x, y + 0.5f, z + 0.5f) + positionInBlock,
                    new Vector3(x + 0.5f, y + 0.5f, z + 0.5f) + positionInBlock,
                    new Vector3(x + 0.5f, y + 0.5f, z) + positionInBlock
            }, Vector3.up, blockId, positionInBlock);
        }

        private void Build(Vector3[] vertices, Vector3 side, int blockId, Vector3 positionInBlock)
        {
            _chunkMesh.Vertices.AddRange(vertices);
            CreateTriangles();
            CreateUvs(side, blockId - 1, positionInBlock);
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

        private void CreateUvs(Vector3 side, int blockId, Vector3 positionInBlock)
        {
            Vector2 position;
            if (side == Vector3.up)
                position = _allBlocks.GetBlock(blockId).upTexturePosition;
            else if (side == Vector3.down)
                position = _allBlocks.GetBlock(blockId).downTexturePosition;
            else
                position = _allBlocks.GetBlock(blockId).sideTexturePosition;

            if (side == Vector3.left)
                position += new Vector2(0.5f - positionInBlock.z, positionInBlock.y);
            else if (side == Vector3.right)
                position += new Vector2(positionInBlock.z, positionInBlock.y);
            else if (side == Vector3.back)
                position += new Vector2(positionInBlock.x, positionInBlock.y);
            else if (side == Vector3.forward)
                position += new Vector2(0.5f - positionInBlock.x, positionInBlock.y);
            else if (side == Vector3.up)
                position += new Vector2(positionInBlock.x, positionInBlock.z);
            else if (side == Vector3.down)
                position += new Vector2(positionInBlock.x, 0.5f - positionInBlock.z);
            else
                position = Vector2.zero;

            _chunkMesh.Uvs.AddRange(new Vector2[]
            {
                    new Vector2(position.x / _allBlocks.TextureSize.x, position.y / _allBlocks.TextureSize.y),
                    new Vector2(position.x / _allBlocks.TextureSize.x, (position.y + 0.5f) / _allBlocks.TextureSize.y),
                    new Vector2((position.x + 0.5f) / _allBlocks.TextureSize.x, (position.y + 0.5f) / _allBlocks.TextureSize.y),
                    new Vector2((position.x + 0.5f) / _allBlocks.TextureSize.x, position.y / _allBlocks.TextureSize.y)
            });
        }

        private void CreateUvs2()
        {
            _chunkMesh.Uvs2.AddRange(new Vector2[4]);
        }

        private void CreateUvs3()
        {
            _chunkMesh.Uvs3.AddRange(new Vector2[4]);
        }
    }
}