using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Gameplay.Terrain
{
    public sealed class TerrainController : MonoBehaviour
    {
        [SerializeField] private AllBlocks _allBlocks;
        [SerializeField] private Material _material;

        private PlayerController _playerController;
        private ChunksPool _chunksPool;
        private TerrainGenerator _terrainGenerator;
        private readonly Dictionary<Vector2Int, Chunk> _chunks = new();
        private readonly List<Vector2Int> _chunksIdForBuild = new();
        private readonly List<Vector2Int> _chunksIdForDelete = new();
        private Vector2Int _centerChunk = Vector2Int.zero;
        private bool _isBuildProcess;

        public void Init(PlayerController playerController)
        {
            _playerController = playerController;
            _chunksPool = new();
            _playerController.OnUpdatePosition += UpdateChunks;
            _terrainGenerator = new();
            AddIdChunksForBuild();
            BuildNearestChunk(false);
        }

        private void UpdateChunks(Vector2Int centerPosition)
        {
            _centerChunk = centerPosition;
            TryDeleteOldChunks();
            AddIdChunksForBuild();
            if (!_isBuildProcess)
                BuildNearestChunk(true);
        }

        private void TryDeleteOldChunks()
        {
            Vector2Int chunkId;
            foreach (var chunk in _chunks)
            {
                chunkId = chunk.Key;
                if (chunkId.x > _centerChunk.x + Settings.WorldRadius ||
                    chunkId.x < _centerChunk.x - Settings.WorldRadius ||
                    chunkId.y > _centerChunk.y + Settings.WorldRadius ||
                    chunkId.y < _centerChunk.y - Settings.WorldRadius)
                {
                    if (!_chunksIdForDelete.Contains(chunkId))
                        _chunksIdForDelete.Add(chunkId);
                }
            }
            for (int i = _chunksIdForDelete.Count - 1; i >= 0; i--)
            {
                chunkId = _chunksIdForDelete[i];
                if (_chunks[chunkId].TypeChunk != TypeChunkStatus.BuildProcess)
                {
                    _chunks[chunkId].gameObject.SetActive(false);
                    _chunksPool.ReturnChunk(_chunks[chunkId]);
                    _chunks[chunkId].TypeChunk = TypeChunkStatus.Disable;
                    _chunks.Remove(chunkId);
                    _chunksIdForDelete.RemoveAt(i);
                }
            }
        }

        private void AddIdChunksForBuild()
        {
            Vector2Int chunkId;
            for (int z = -Settings.WorldRadius; z <= Settings.WorldRadius; z++)
            {
                for (int x = -Settings.WorldRadius; x <= Settings.WorldRadius; x++)
                {
                    chunkId = new Vector2Int(x, z) + _centerChunk;
                    if (_chunks.ContainsKey(chunkId) && _chunks[chunkId].TypeChunk != TypeChunkStatus.DataOnly || _chunksIdForBuild.Contains(chunkId))
                        continue;
                    _chunksIdForBuild.Add(chunkId);
                }
            }
        }

        private void BuildNearestChunk(bool isAsync)
        {
            _isBuildProcess = true;

            Vector2Int chunkId = GetNearestChunkId();
            Chunk chunk;
            if (_chunks.ContainsKey(chunkId))
                chunk = _chunks[chunkId];
            else
                chunk = AddChunkData(chunkId);

            if (_chunks.ContainsKey(chunkId + Vector2Int.left))
                chunk.LeftChunk = _chunks[chunkId + Vector2Int.left];
            else
                chunk.LeftChunk = AddChunkData(chunkId + Vector2Int.left);

            if (_chunks.ContainsKey(chunkId + Vector2Int.left + Vector2Int.down))
                chunk.LeftBackChunk = _chunks[chunkId + Vector2Int.left + Vector2Int.down];
            else
                chunk.LeftBackChunk = AddChunkData(chunkId + Vector2Int.left + Vector2Int.down);

            if (_chunks.ContainsKey(chunkId + Vector2Int.left + Vector2Int.up))
                chunk.LeftForwardChunk = _chunks[chunkId + Vector2Int.left + Vector2Int.up];
            else
                chunk.LeftForwardChunk = AddChunkData(chunkId + Vector2Int.left + Vector2Int.up);

            if (_chunks.ContainsKey(chunkId + Vector2Int.right))
                chunk.RightChunk = _chunks[chunkId + Vector2Int.right];
            else
                chunk.RightChunk = AddChunkData(chunkId + Vector2Int.right);

            if (_chunks.ContainsKey(chunkId + Vector2Int.right + Vector2Int.down))
                chunk.RightBackChunk = _chunks[chunkId + Vector2Int.right + Vector2Int.down];
            else
                chunk.RightBackChunk = AddChunkData(chunkId + Vector2Int.right + Vector2Int.down);

            if (_chunks.ContainsKey(chunkId + Vector2Int.right + Vector2Int.up))
                chunk.RightForwardChunk = _chunks[chunkId + Vector2Int.right + Vector2Int.up];
            else
                chunk.RightForwardChunk = AddChunkData(chunkId + Vector2Int.right + Vector2Int.up);

            if (_chunks.ContainsKey(chunkId + Vector2Int.down))
                chunk.BackChunk = _chunks[chunkId + Vector2Int.down];
            else
                chunk.BackChunk = AddChunkData(chunkId + Vector2Int.down);

            if (_chunks.ContainsKey(chunkId + Vector2Int.up))
                chunk.ForwardChunk = _chunks[chunkId + Vector2Int.up];
            else
                chunk.ForwardChunk = AddChunkData(chunkId + Vector2Int.up);

            chunk.TypeChunk = TypeChunkStatus.BuildProcess;
            chunk.Build(isAsync, OnEndBuildChunk);
        }

        private Chunk AddChunkData(Vector2Int chunkId)
        {
            Chunk chunk = _chunksPool.GetChunk();
            chunk.TryInit(_material, _allBlocks);
            chunk.transform.position = new Vector3((chunkId.x - 0.5f) * Settings.ChunkSize.x, 0, (chunkId.y - 0.5f) * Settings.ChunkSize.z);
            chunk.Id = chunkId;
            byte[,,] miniBlocksValue;
            chunk.BlockIds = _terrainGenerator.GetBlockIds(chunkId, out miniBlocksValue);
            chunk.BlockLights = new byte[Settings.ChunkSize.x, Settings.ChunkSize.y, Settings.ChunkSize.z];
            chunk.BlockSkyLights = new byte[Settings.ChunkSize.x, Settings.ChunkSize.y, Settings.ChunkSize.z];
            chunk.MiniBlocksValue = miniBlocksValue;
            chunk.TypeChunk = TypeChunkStatus.DataOnly;
            _chunks.Add(chunkId, chunk);
            return chunk;
        }

        private void OnEndBuildChunk(Chunk chunk)
        {
            _chunksIdForBuild.Remove(chunk.Id);
            chunk.gameObject.SetActive(true);
            chunk.TypeChunk = TypeChunkStatus.Enable;

            if (_chunksIdForBuild.Count == 0)
                _isBuildProcess = false;
            else
                BuildNearestChunk(true);
        }

        //Возвращает Id ближайшего к игроку чанка
        private Vector2Int GetNearestChunkId()
        {
            float minDisance = float.MaxValue;
            float currentDistance;
            Vector2Int chunkId = Vector2Int.zero;
            for (int i = 0; i < _chunksIdForBuild.Count; i++)
            {
                currentDistance = Vector2.Distance(_centerChunk, _chunksIdForBuild[i]);
                if (currentDistance < minDisance)
                {
                    minDisance = currentDistance;
                    chunkId = _chunksIdForBuild[i];
                }
            }
            return chunkId;
        }

        private void OnDestroy()
        {
            _playerController.OnUpdatePosition -= UpdateChunks;
        }
    }
}