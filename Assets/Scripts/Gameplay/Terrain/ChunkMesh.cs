using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Gameplay.Terrain
{
    public sealed class ChunkMesh
    {
        public Mesh Mesh { get; }
        public List<Vector3> Vertices { get; }
        public List<Vector3Int> PositionQuads { get; }
        public List<Vector3Int> OffsetQuads { get; }
        public List<int> Triangles { get; }
        public List<Vector2> Uvs { get; }
        public List<Vector2> Uvs2 { get; }
        public List<Vector2> Uvs3 { get; }

        public ChunkMesh()
        {
            Mesh = new();
            Mesh.MarkDynamic();
            Vertices = new();
            PositionQuads = new();
            OffsetQuads = new();
            Triangles = new();
            Uvs = new();
            Uvs2 = new();
            Uvs3 = new();
        }

        public void Reset()
        {
            Mesh.Clear();
            Vertices.Clear();
            PositionQuads.Clear();
            OffsetQuads.Clear();
            Triangles.Clear();
            Uvs.Clear();
            Uvs2.Clear();
            Uvs3.Clear();
        }

        public void ApplyMesh()
        {
            Mesh.Clear();
            Mesh.vertices = Vertices.ToArray();
            Mesh.triangles = Triangles.ToArray();
            Mesh.uv = Uvs.ToArray();
            Mesh.uv2 = Uvs2.ToArray();
            Mesh.uv3 = Uvs3.ToArray();
            Mesh.RecalculateNormals();
        }
    }
}