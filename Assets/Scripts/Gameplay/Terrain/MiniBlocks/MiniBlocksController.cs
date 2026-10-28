using UnityEngine;

namespace MyGame.Gameplay.Terrain.MiniBlock
{
    public sealed class MiniBlocksController
    {
        private readonly Chunk _chunk;
        private readonly MiniQuadBuilder _miniQuadBuilder;
        private readonly MiniQuadDestroyer _miniQuadDestroyer;
        private readonly MiniBlockBuilder _miniBlockBuilder;
        private readonly HalfBlockBuilder _halfBlockBuilder;
        private readonly RungsBuilder _rungsBuilder;

        public MiniBlocksController(AllBlocks allBlocks, Chunk chunk, ChunkMesh chunkMesh)
        {
            _chunk = chunk;
            _miniQuadBuilder = new(allBlocks, chunk, chunkMesh);
            _miniQuadDestroyer = new(chunk, chunkMesh);
            _miniBlockBuilder = new(this);
            _halfBlockBuilder = new(this);
            _rungsBuilder = new(this);
        }

        public void Build(Vector3 pointPostition, Vector3 normalPosition, Vector3Int id, int blockId, TypeBlock typeBlock)
        {
            if (_chunk.BlockIds[id.x, id.y, id.z] != 0 && _chunk.BlockIds[id.x, id.y, id.z] != blockId)
                return;

            if (typeBlock == TypeBlock.MiniCube)
                _miniBlockBuilder.Build(pointPostition, normalPosition, id);
            else if(typeBlock == TypeBlock.HalfBlock)
                _halfBlockBuilder.Build(pointPostition, normalPosition, id);
            else if(typeBlock == TypeBlock.Rungs)
                _rungsBuilder.Build(pointPostition, normalPosition, id);
        }

        public void TryBuildMiniCube(Vector3Int id, Vector3 positionInBlock)
        {
            byte value = 1;
            int pos = 0;
            if (positionInBlock.x > 0.01f)
            {
                value *= 2;
                pos += 1;
            }
            if (positionInBlock.y > 0.01f)
            {
                value *= 4;
                pos += 2;
            }
            if (positionInBlock.z > 0.01f)
            {
                value *= 16;
                pos += 4;
            }
            if (((_chunk.MiniBlocksValue[id.x, id.y, id.z] >> pos) & 1) == 0)
            {
                _chunk.MiniBlocksValue[id.x, id.y, id.z] += value;
                _miniQuadBuilder.Build(id, positionInBlock);
            }
        }

        public void Destroy(Vector3Int id)
        {
            _miniQuadDestroyer.Destroy(id.x, id.y, id.z);
        }
    }

    public enum TypeBlock
    {
        MiniCube, Rungs, HalfBlock
    }
}