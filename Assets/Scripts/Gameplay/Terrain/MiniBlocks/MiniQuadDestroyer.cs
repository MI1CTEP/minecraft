using UnityEngine;

namespace MyGame.Gameplay.Terrain.MiniBlock
{
    public sealed class MiniQuadDestroyer
    {
        private readonly Chunk _chunk;
        private readonly ChunkMesh _chunkMesh;
        private readonly Vector3[] _offsets =
        {
            new Vector3(0, 0, 0),
            new Vector3(0.5f, 0, 0),
            new Vector3(0, 0.5f, 0),
            new Vector3(0.5f, 0.5f, 0),
            new Vector3(0, 0, 0.5f),
            new Vector3(0.5f, 0, 0.5f),
            new Vector3(0, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f)
        };

        public MiniQuadDestroyer(Chunk chunk, ChunkMesh chunkMesh)
        {
            _chunk = chunk;
            _chunkMesh = chunkMesh;
        }

        public void Destroy(int x, int y, int z)
        {
            for (int i = 0; i < _offsets.Length; i++)
            {
                if (((_chunk.MiniBlocksValue[x, y, z] >> i) & 1) == 1)
                    DestroyMiniCube(x, y, z, _offsets[i]);
            }
        }

        private void DestroyMiniCube(int x, int y, int z, Vector3 offset)
        {
            DestroyLeftQuad(x, y, z, offset);
            DestroyRightQuad(x, y, z, offset);
            DestroyBackQuad(x, y, z, offset);
            DestroyForwardQuad(x, y, z, offset);
            DestroyDownQuad(x, y, z, offset);
            DestroyUpQuad(x, y, z, offset);
        }

        private void DestroyLeftQuad(int x, int y, int z, Vector3 offset)
        {
            Vector3[] vertices = new Vector3[]
                {
                    new Vector3(x, y, z + 0.5f) + offset,
                    new Vector3(x, y + 0.5f, z + 0.5f) + offset,
                    new Vector3(x, y + 0.5f, z) + offset,
                    new Vector3(x, y, z) + offset
                };
            Destroy(vertices);
        }

        private void DestroyRightQuad(int x, int y, int z, Vector3 offset)
        {
            Vector3[] vertices = new Vector3[]
                {
                    new Vector3(x + 0.5f, y, z) + offset,
                    new Vector3(x + 0.5f, y + 0.5f, z) + offset,
                    new Vector3(x + 0.5f, y + 0.5f, z + 0.5f) + offset,
                    new Vector3(x + 0.5f, y, z + 0.5f) + offset
                };
            Destroy(vertices);
        }

        private void DestroyBackQuad(int x, int y, int z, Vector3 offset)
        {
            Vector3[] vertices = new Vector3[]
                {
                    new Vector3(x, y, z) + offset,
                    new Vector3(x, y + 0.5f, z) + offset,
                    new Vector3(x + 0.5f, y + 0.5f, z) + offset,
                    new Vector3(x + 0.5f, y, z) + offset
                };
            Destroy(vertices);
        }

        private void DestroyForwardQuad(int x, int y, int z, Vector3 offset)
        {
            Vector3[] vertices = new Vector3[]
                {
                    new Vector3(x + 0.5f, y, z + 0.5f) + offset,
                    new Vector3(x + 0.5f, y + 0.5f, z + 0.5f) + offset,
                    new Vector3(x, y + 0.5f, z + 0.5f) + offset,
                    new Vector3(x, y, z + 0.5f) + offset
                };
            Destroy(vertices);
        }

        private void DestroyDownQuad(int x, int y, int z, Vector3 offset)
        {
            Vector3[] vertices = new Vector3[]
                {
                    new Vector3(x, y, z + 0.5f) + offset,
                    new Vector3(x, y, z) + offset,
                    new Vector3(x + 0.5f, y, z) + offset,
                    new Vector3(x + 0.5f, y, z + 0.5f) + offset
                };
            Destroy(vertices);
        }

        private void DestroyUpQuad(int x, int y, int z, Vector3 offset)
        {
            Vector3[] vertices = new Vector3[]
                {
                    new Vector3(x, y + 0.5f, z) + offset,
                    new Vector3(x, y + 0.5f, z + 0.5f) + offset,
                    new Vector3(x + 0.5f, y + 0.5f, z + 0.5f) + offset,
                    new Vector3(x + 0.5f, y + 0.5f, z) + offset
                };
            Destroy(vertices);
        }

        private void Destroy(Vector3[] vertices)
        {
            int firstverticeId = int.MaxValue;
            for (int i = 0; i < _chunkMesh.Vertices.Count; i += 4)
            {
                if (_chunkMesh.Vertices[i] == vertices[0] &&
                    _chunkMesh.Vertices[i + 1] == vertices[1] &&
                    _chunkMesh.Vertices[i + 2] == vertices[2] &&
                    _chunkMesh.Vertices[i + 3] == vertices[3])
                {
                    firstverticeId = i;
                    break;
                }
            }
            if (firstverticeId != int.MaxValue)
            {
                int lastId = _chunkMesh.Vertices.Count - 4;
                for (int i = 0; i < 4; i++)
                {
                    (_chunkMesh.Vertices[firstverticeId + i], _chunkMesh.Vertices[lastId + i]) = (_chunkMesh.Vertices[lastId + i], _chunkMesh.Vertices[firstverticeId + i]);
                    (_chunkMesh.Uvs[firstverticeId + i], _chunkMesh.Uvs[lastId + i]) = (_chunkMesh.Uvs[lastId + i], _chunkMesh.Uvs[firstverticeId + i]);
                    (_chunkMesh.Uvs2[firstverticeId + i], _chunkMesh.Uvs2[lastId + i]) = (_chunkMesh.Uvs2[lastId + i], _chunkMesh.Uvs2[firstverticeId + i]);
                    (_chunkMesh.Uvs3[firstverticeId + i], _chunkMesh.Uvs3[lastId + i]) = (_chunkMesh.Uvs3[lastId + i], _chunkMesh.Uvs3[firstverticeId + i]);
                }

                (_chunkMesh.PositionQuads[firstverticeId / 4], _chunkMesh.PositionQuads[_chunkMesh.PositionQuads.Count - 1]) = (_chunkMesh.PositionQuads[^1], _chunkMesh.PositionQuads[firstverticeId / 4]);
                _chunkMesh.PositionQuads.RemoveAt(_chunkMesh.PositionQuads.Count - 1);

                (_chunkMesh.OffsetQuads[firstverticeId / 4], _chunkMesh.OffsetQuads[^1]) = (_chunkMesh.OffsetQuads[^1], _chunkMesh.OffsetQuads[firstverticeId / 4]);
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