using UnityEngine;

namespace MyGame.Gameplay.Terrain.Block
{
    public sealed class QuadDestroyer
    {
        private readonly Chunk _chunk;
        private readonly ChunkMesh _chunkMesh;

        public QuadDestroyer(Chunk chunk, ChunkMesh chunkMesh)
        {
            _chunk = chunk;
            _chunkMesh = chunkMesh;
        }

        public void DestroyLeftQuad(int x, int y, int z)
        {
            if (x != 0 && _chunk.MiniBlocksValue[x - 1, y, z] < 255 || x == 0 && _chunk.LeftChunk.MiniBlocksValue[Settings.ChunkSize.x - 1, y, z] < 255)
            {
                Vector3[] vertices = new Vector3[]
                {
                    new Vector3(x, y, z + 1),
                    new Vector3(x, y + 1, z + 1),
                    new Vector3(x, y + 1, z),
                    new Vector3(x, y, z)
                };
                Destroy(vertices);
            }
        }

        public void DestroyRightQuad(int x, int y, int z)
        {
            if (x != Settings.ChunkSize.x - 1 && _chunk.MiniBlocksValue[x + 1, y, z] < 255 || x == Settings.ChunkSize.x - 1 && _chunk.RightChunk.MiniBlocksValue[0, y, z] < 255)
            {
                Vector3[] vertices = new Vector3[]
                {
                    new Vector3(x + 1, y, z),
                    new Vector3(x + 1, y + 1, z),
                    new Vector3(x + 1, y + 1, z + 1),
                    new Vector3(x + 1, y, z + 1)
                };
                Destroy(vertices);
            }
        }

        public void DestroyBackQuad(int x, int y, int z)
        {
            if (z != 0 && _chunk.MiniBlocksValue[x, y, z - 1] < 255 || z == 0 && _chunk.BackChunk.MiniBlocksValue[x, y, Settings.ChunkSize.z - 1] < 255)
            {
                Vector3[] vertices = new Vector3[]
                {
                    new Vector3(x, y, z),
                    new Vector3(x, y + 1, z),
                    new Vector3(x + 1, y + 1, z),
                    new Vector3(x + 1, y, z)
                };
                Destroy(vertices);
            }
        }

        public void DestroyForwardQuad(int x, int y, int z)
        {
            if (z != Settings.ChunkSize.z - 1 && _chunk.MiniBlocksValue[x, y, z + 1] < 255 || z == Settings.ChunkSize.z - 1 && _chunk.ForwardChunk.MiniBlocksValue[x, y, 0] < 255)
            {
                Vector3[] vertices = new Vector3[]
                {
                    new Vector3(x + 1, y, z + 1),
                    new Vector3(x + 1, y + 1, z + 1),
                    new Vector3(x, y + 1, z + 1),
                    new Vector3(x, y, z + 1)
                };
                Destroy(vertices);
            }
        }

        public void DestroyDownQuad(int x, int y, int z)
        {
            if (y != 0 && _chunk.MiniBlocksValue[x, y - 1, z] < 255)
            {
                Vector3[] vertices = new Vector3[]
                {
                    new Vector3(x, y, z + 1),
                    new Vector3(x, y, z),
                    new Vector3(x + 1, y, z),
                    new Vector3(x + 1, y, z + 1)
                };
                Destroy(vertices);
            }
        }

        public void DestroyUpQuad(int x, int y, int z)
        {
            if (y != Settings.ChunkSize.y - 1 && _chunk.MiniBlocksValue[x, y + 1, z] < 255)
            {
                Vector3[] vertices = new Vector3[]
                {
                    new Vector3(x, y + 1, z),
                    new Vector3(x, y + 1, z + 1),
                    new Vector3(x + 1, y + 1, z + 1),
                    new Vector3(x + 1, y + 1, z)
                };
                Destroy(vertices);
            }
        }

        private void Destroy(Vector3[] vertices)
        {
            int firstverticeId = int.MaxValue;
            for (int i = 0; i < _chunkMesh.Vertices.Count; i += 4)
            {
                if(_chunkMesh.Vertices[i] == vertices[0] &&
                    _chunkMesh.Vertices[i + 1] == vertices[1] &&
                    _chunkMesh.Vertices[i + 2] == vertices[2] &&
                    _chunkMesh.Vertices[i + 3] == vertices[3])
                {
                    firstverticeId = i;
                    break;
                }
            }
            if(firstverticeId != int.MaxValue)
            {
                int lastId = _chunkMesh.Vertices.Count - 4;
                for (int i = 0; i < 4; i++)
                {
                    (_chunkMesh.Vertices[firstverticeId + i], _chunkMesh.Vertices[lastId + i]) = (_chunkMesh.Vertices[lastId + i], _chunkMesh.Vertices[firstverticeId + i]);
                    (_chunkMesh.Uvs[firstverticeId + i], _chunkMesh.Uvs[lastId + i]) = (_chunkMesh.Uvs[lastId + i], _chunkMesh.Uvs[firstverticeId + i]);
                    (_chunkMesh.Uvs2[firstverticeId + i], _chunkMesh.Uvs2[lastId + i]) = (_chunkMesh.Uvs2[lastId + i], _chunkMesh.Uvs2[firstverticeId + i]);
                    (_chunkMesh.Uvs3[firstverticeId + i], _chunkMesh.Uvs3[lastId + i]) = (_chunkMesh.Uvs3[lastId + i], _chunkMesh.Uvs3[firstverticeId + i]);
                }

                (_chunkMesh.PositionQuads[firstverticeId / 4], _chunkMesh.PositionQuads[_chunkMesh.PositionQuads.Count - 1]) = (_chunkMesh.PositionQuads[_chunkMesh.PositionQuads.Count - 1], _chunkMesh.PositionQuads[firstverticeId / 4]);
                _chunkMesh.PositionQuads.RemoveAt(_chunkMesh.PositionQuads.Count - 1);

                (_chunkMesh.OffsetQuads[firstverticeId / 4], _chunkMesh.OffsetQuads[_chunkMesh.OffsetQuads.Count - 1]) = (_chunkMesh.OffsetQuads[_chunkMesh.OffsetQuads.Count - 1], _chunkMesh.OffsetQuads[firstverticeId / 4]);
                _chunkMesh.OffsetQuads.RemoveAt(_chunkMesh.OffsetQuads.Count - 1);

                _chunkMesh.Vertices.RemoveRange(lastId, 4);
                _chunkMesh.Uvs.RemoveRange(lastId, 4);
                _chunkMesh.Uvs2.RemoveRange(lastId, 4);
                _chunkMesh.Uvs3.RemoveRange(lastId, 4);

                lastId = _chunkMesh.Triangles.Count - 6;
                _chunkMesh.Triangles.RemoveRange(lastId, 6);
            }
        }
    }
}