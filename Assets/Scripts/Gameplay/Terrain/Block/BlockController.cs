using UnityEngine;

namespace MyGame.Gameplay.Terrain.Block
{
    public sealed class BlockController : MonoBehaviour
    {
        private readonly QuadBuilder _quadBuilder;
        private readonly QuadDestroyer _quadDestroyer;

        public BlockController(AllBlocks allBlocks, Chunk chunk, ChunkMesh chunkMesh)
        {
            _quadBuilder = new(allBlocks, chunk, chunkMesh);
            _quadDestroyer = new(chunk, chunkMesh);
        }

        public void Build(int x, int y, int z)
        {
            _quadBuilder.BuildLeftQuad(x, y, z);
            _quadBuilder.BuildRightQuad(x, y, z);
            _quadBuilder.BuildDownQuad(x, y, z);
            _quadBuilder.BuildUpQuad(x, y, z);
            _quadBuilder.BuildBackQuad(x, y, z);
            _quadBuilder.BuildForwardQuad(x, y, z);
        }
        public void BuildLeftQuad(int x, int y, int z) => _quadBuilder.BuildLeftQuad(x, y, z);
        public void BuildRightQuad(int x, int y, int z) => _quadBuilder.BuildRightQuad(x, y, z);
        public void BuildDownQuad(int x, int y, int z) => _quadBuilder.BuildDownQuad(x, y, z);
        public void BuildUpQuad(int x, int y, int z) => _quadBuilder.BuildUpQuad(x, y, z);
        public void BuildBackQuad(int x, int y, int z) => _quadBuilder.BuildBackQuad(x, y, z);
        public void BuildForwardQuad(int x, int y, int z) => _quadBuilder.BuildForwardQuad(x, y, z);

        public void Destroy(int x, int y, int z)
        {
            _quadDestroyer.DestroyLeftQuad(x, y, z);
            _quadDestroyer.DestroyRightQuad(x, y, z);
            _quadDestroyer.DestroyDownQuad(x, y, z);
            _quadDestroyer.DestroyUpQuad(x, y, z);
            _quadDestroyer.DestroyBackQuad(x, y, z);
            _quadDestroyer.DestroyForwardQuad(x, y, z);
        }
        public void DestroyLeftQuad(int x, int y, int z) => _quadDestroyer.DestroyLeftQuad(x, y, z);
        public void DestroyRightQuad(int x, int y, int z) => _quadDestroyer.DestroyRightQuad(x, y, z);
        public void DestroyDownQuad(int x, int y, int z) => _quadDestroyer.DestroyDownQuad(x, y, z);
        public void DestroyUpQuad(int x, int y, int z) => _quadDestroyer.DestroyUpQuad(x, y, z);
        public void DestroyBackQuad(int x, int y, int z) => _quadDestroyer.DestroyBackQuad(x, y, z);
        public void DestroyForwardQuad(int x, int y, int z) => _quadDestroyer.DestroyForwardQuad(x, y, z);
    }
}