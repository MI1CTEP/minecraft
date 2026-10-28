using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Gameplay.Terrain
{
    public sealed class ChunksPool
    {
        private readonly Stack<Chunk> _chunks = new();

        public Chunk GetChunk()
        {
            Chunk chunk;
            if(_chunks.Count == 0)
            {
                GameObject gameObject = new();
                chunk = gameObject.AddComponent<Chunk>();
            }
            else
            {
                chunk = _chunks.Pop();
            }
            return chunk;
        }

        public void ReturnChunk(Chunk chunk)
        {
            _chunks.Push(chunk);
        }
    }
}