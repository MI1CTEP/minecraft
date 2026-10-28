using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using MyGame.Gameplay.Terrain.Block;
using MyGame.Gameplay.Terrain.MiniBlock;

namespace MyGame.Gameplay.Terrain
{
    public sealed class Chunk : MonoBehaviour
    {
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private ChunkMesh _chunkMesh;
        private BlockController _blockController;
        private MiniBlocksController _miniBlocksController;
        private ChunkLight _chunkLight;
        private ChunkSkyLight _chunkSkyLight;
        private bool _isInited;

        public ChunkMesh ChunkMesh => _chunkMesh;
        public Chunk LeftChunk { get; set; }
        public Chunk LeftBackChunk { get; set; }
        public Chunk LeftForwardChunk { get; set; }
        public Chunk RightChunk { get; set; }
        public Chunk RightBackChunk { get; set; }
        public Chunk RightForwardChunk { get; set; }
        public Chunk BackChunk { get; set; }
        public Chunk ForwardChunk { get; set; }
        public Vector2Int Id { get; set; }
        public int[,,] BlockIds { get; set; }
        public byte[,,] BlockLights { get; set; }
        public byte[,,] BlockSkyLights { get; set; }
        public byte[,,] MiniBlocksValue { get; set; }
        public TypeChunkStatus TypeChunk { get; set; }

        public void TryInit(Material material, AllBlocks allBlocks)
        {
            if (_isInited) return;

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshCollider = gameObject.AddComponent<MeshCollider>();

            _chunkMesh = new();
            _blockController = new(allBlocks, this, _chunkMesh);
            _miniBlocksController = new(allBlocks, this, _chunkMesh);
            _chunkLight = new(this);
            _chunkSkyLight = new(this);

            _isInited = true;
        }

        public async UniTask Build(bool isAsync, UnityAction<Chunk> onEndBuild)
        {
            _chunkMesh.Reset();
            for (int z = 0; z < Settings.ChunkSize.z; z++)
            {
                if (isAsync)
                    await UniTask.DelayFrame(1);

                for (int y = 0; y < Settings.ChunkSize.y; y++)
                    for (int x = 0; x < Settings.ChunkSize.x; x++)
                        if (BlockIds[x, y, z] != 0)
                            _blockController.Build(x, y, z);
            }
            _chunkSkyLight.StartCountLight();
            UpdateNeighborsMeshes();
            UpdateMesh();
            onEndBuild?.Invoke(this);
        }

        public void Click(Vector3 pointPostition, Vector3 normalPosition, bool isDestroy)
        {
            Vector3Int id = Vector3Int.RoundToInt(pointPostition - Vector3.one * 0.5f - normalPosition * 0.1f);
            Chunk chunk = this;
            if (isDestroy)
            {
                if (MiniBlocksValue[id.x, id.y, id.z] == 255 || MiniBlocksValue[id.x, id.y, id.z] == 0)
                    DeleteBlock(id);
                else
                    DeleteMiniBlock(id);

                _chunkSkyLight.OnDeleteBlock(id);
                UpdateNeighborsMeshes();
                UpdateMesh();
            }
            else
            {
                int blockId = CurrentBlock.Instance.GetBlockId;

                if (normalPosition.x == 1) id.x = Mathf.RoundToInt(pointPostition.x - normalPosition.x * 0.1f);
                else if (normalPosition.y == 1) id.y = Mathf.RoundToInt(pointPostition.y - normalPosition.y * 0.1f);
                else if (normalPosition.z == 1) id.z = Mathf.RoundToInt(pointPostition.z - normalPosition.z * 0.1f);
                else if (normalPosition.x == -1) id.x = Mathf.RoundToInt(pointPostition.x - normalPosition.x * 0.1f - 1);
                else if (normalPosition.y == -1) id.y = Mathf.RoundToInt(pointPostition.y - normalPosition.y * 0.1f - 1);
                else if (normalPosition.z == -1) id.z = Mathf.RoundToInt(pointPostition.z - normalPosition.z * 0.1f - 1);

                if (id.x < 0)
                {
                    chunk = LeftChunk;
                    id.x = Settings.ChunkSize.x - 1;
                }
                else if (id.x >= Settings.ChunkSize.x)
                {
                    chunk = RightChunk;
                    id.x = 0;
                }

                if (id.z < 0)
                {
                    chunk = chunk.BackChunk;
                    id.z = Settings.ChunkSize.z - 1;
                }
                else if (id.z >= Settings.ChunkSize.z)
                {
                    chunk = chunk.ForwardChunk;
                    id.z = 0;
                }
                chunk.AddBlock(pointPostition, normalPosition, id, blockId);
            }
        }

        public Chunk GetChunk(ref int x, ref int z)
        {
            if (x < 0)
            {
                x += Settings.ChunkSize.x;
                if (z < 0)
                {
                    z += Settings.ChunkSize.z;
                    return LeftBackChunk;
                }
                else if (z >= Settings.ChunkSize.z)
                {
                    z -= Settings.ChunkSize.z;
                    return LeftForwardChunk;
                }
                else return LeftChunk;
            }
            else if (x >= Settings.ChunkSize.x)
            {
                x -= Settings.ChunkSize.x;
                if (z < 0)
                {
                    z += Settings.ChunkSize.z;
                    return RightBackChunk;
                }
                else if (z >= Settings.ChunkSize.z)
                {
                    z -= Settings.ChunkSize.z;
                    return RightForwardChunk;
                }
                else return RightChunk;
            }
            else
            {
                if (z < 0)
                {
                    z += Settings.ChunkSize.z;
                    return BackChunk;
                }
                else if (z >= Settings.ChunkSize.z)
                {
                    z -= Settings.ChunkSize.z;
                    return ForwardChunk;
                }
                else return this;
            }
        }

        private void AddBlock(Vector3 pointPostition, Vector3 normalPosition, Vector3Int id, int blockId)
        {
            if(CurrentBlock.Instance.TypeBlock == 1 ||
               CurrentBlock.Instance.TypeBlock == 2 ||
               CurrentBlock.Instance.TypeBlock == 3)
            {
                BlockIds[id.x, id.y, id.z] = blockId;

                if (CurrentBlock.Instance.TypeBlock == 1)
                    _miniBlocksController.Build(pointPostition, normalPosition, id, blockId, TypeBlock.Rungs);
                else if (CurrentBlock.Instance.TypeBlock == 2)
                    _miniBlocksController.Build(pointPostition, normalPosition, id, blockId, TypeBlock.HalfBlock);
                else if (CurrentBlock.Instance.TypeBlock == 3)
                    _miniBlocksController.Build(pointPostition, normalPosition, id, blockId, TypeBlock.MiniCube);

                if (MiniBlocksValue[id.x, id.y, id.z] == 255)
                {
                    _miniBlocksController.Destroy(id);
                    AddCube(id, BlockIds[id.x, id.y, id.z]);
                    return;
                }
            }
            else AddCube(id, blockId);

            if (blockId == 3)
                _chunkLight.SetLamp(id);
            else
                _chunkLight.RecalculateLight(id);

            _chunkSkyLight.OnSetBlock(id);

            UpdateNeighborsMeshes();
            UpdateMesh();
        }

        private void DeleteBlock(Vector3Int id)
        {
            int blockId = BlockIds[id.x, id.y, id.z];
            BlockIds[id.x, id.y, id.z] = 0;
            MiniBlocksValue[id.x, id.y, id.z] = 0;

            _blockController.Destroy(id.x, id.y, id.z);

            _blockController.BuildRightQuad(id.x - 1, id.y, id.z);
            _blockController.BuildLeftQuad(id.x + 1, id.y, id.z);
            _blockController.BuildUpQuad(id.x, id.y - 1, id.z);
            _blockController.BuildDownQuad(id.x, id.y + 1, id.z);
            _blockController.BuildForwardQuad(id.x, id.y, id.z - 1);
            _blockController.BuildBackQuad(id.x, id.y, id.z + 1);

            if (id.x == 0)
                LeftChunk._blockController.BuildRightQuad(Settings.ChunkSize.x - 1, id.y, id.z);
            else if (id.x == Settings.ChunkSize.x - 1)
                RightChunk._blockController.BuildLeftQuad(0, id.y, id.z);

            if (id.z == 0)
                BackChunk._blockController.BuildForwardQuad(id.x, id.y, Settings.ChunkSize.z - 1);
            else if (id.z == Settings.ChunkSize.z - 1)
                ForwardChunk._blockController.BuildBackQuad(id.x, id.y, 0);

            if (blockId == 3)
                _chunkLight.DeleteLamp(id, 15);
            else
                _chunkLight.RecalculateLight(id);
        }

        private void DeleteMiniBlock(Vector3Int id)
        {
            int blockId = BlockIds[id.x, id.y, id.z];
            BlockIds[id.x, id.y, id.z] = 0;

            _miniBlocksController.Destroy(id);
            MiniBlocksValue[id.x, id.y, id.z] = 0;

            if (blockId == 3)
                _chunkLight.DeleteLamp(id, 15);
            else
                _chunkLight.RecalculateLight(id);
        }

        public void AddCube(Vector3Int id, int blockId)
        {
            BlockIds[id.x, id.y, id.z] = 0;

            _miniBlocksController.Destroy(id);
            MiniBlocksValue[id.x, id.y, id.z] = 0;

            _blockController.DestroyRightQuad(id.x - 1, id.y, id.z);
            _blockController.DestroyLeftQuad(id.x + 1, id.y, id.z);
            _blockController.DestroyUpQuad(id.x, id.y - 1, id.z);
            _blockController.DestroyDownQuad(id.x, id.y + 1, id.z);
            _blockController.DestroyForwardQuad(id.x, id.y, id.z - 1);
            _blockController.DestroyBackQuad(id.x, id.y, id.z + 1);

            if (id.x == 0)
               LeftChunk._blockController.DestroyRightQuad(Settings.ChunkSize.x - 1, id.y, id.z);
            else if (id.x == Settings.ChunkSize.x - 1)
                RightChunk._blockController.DestroyLeftQuad(0, id.y, id.z);

            if (id.z == 0)
                BackChunk._blockController.DestroyForwardQuad(id.x, id.y, Settings.ChunkSize.z - 1);
            else if (id.z == Settings.ChunkSize.z - 1)
                ForwardChunk._blockController.DestroyBackQuad(id.x, id.y, 0);

            BlockIds[id.x, id.y, id.z] = blockId;
            MiniBlocksValue[id.x, id.y, id.z] = 255;

            _blockController.Build(id.x, id.y, id.z);
        }

        private void UpdateNeighborsMeshes()
        {
            LeftChunk.UpdateMesh();
            LeftBackChunk.UpdateMesh();
            LeftForwardChunk.UpdateMesh();
            RightChunk.UpdateMesh();
            RightBackChunk.UpdateMesh();
            RightForwardChunk.UpdateMesh();
            BackChunk.UpdateMesh();
            ForwardChunk.UpdateMesh();
    }

        private void UpdateMesh()
        {
            if (TypeChunk == TypeChunkStatus.Disable || TypeChunk == TypeChunkStatus.DataOnly)
                return;

            _chunkMesh.ApplyMesh();
            _meshFilter.mesh = _chunkMesh.Mesh;
            _meshCollider.sharedMesh = _chunkMesh.Mesh;
        }
    }

    public enum TypeChunkStatus
    {
        Disable, DataOnly, BuildProcess, Enable
    }
}