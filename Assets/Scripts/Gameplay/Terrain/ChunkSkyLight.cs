using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Gameplay.Terrain
{
    public sealed class ChunkSkyLight : MonoBehaviour
    {
        private readonly Chunk _chunk;
        private readonly Queue<Vector3Int> _lightsForSet;
        private readonly Queue<Vector3Int> _lightsForDelete;
        private readonly Vector3Int[] _neighbors;
        private Vector2Int _lampCurrentPosition;

        public ChunkSkyLight(Chunk chunk)
        {
            _chunk = chunk;
            _lightsForSet = new();
            _lightsForDelete = new();
            _neighbors = new Vector3Int[]
            {
                Vector3Int.left, Vector3Int.right, Vector3Int.down, Vector3Int.up, Vector3Int.back, Vector3Int.forward
            };
        }

        public void StartCountLight()
        {
            for (int x = 0; x < Settings.ChunkSize.x; x++)
            {
                for (int z = 0; z < Settings.ChunkSize.z; z++)
                {
                    for (int y = Settings.ChunkSize.y - 1; y >= 0; y--)
                    {
                        if (_chunk.BlockIds[x, y, z] == 0)
                            _chunk.BlockSkyLights[x, y, z] = 15;
                        else break;
                    }
                }
            }
            Vector3Int id = Vector3Int.zero;
            for (; id.x < Settings.ChunkSize.x; id.x++)
            {
                for (; id.z < Settings.ChunkSize.z; id.z++)
                {
                    for (; id.y < Settings.ChunkSize.y; id.y++)
                    {
                        if (_chunk.BlockSkyLights[id.x, id.y, id.z] == 15)
                            break;

                        if (_chunk.BlockIds[id.x, id.y, id.z] != 0)
                            continue;

                        if (GetLight(id.x + 1, id.y, id.z) == 15 ||
                            GetLight(id.x - 1, id.y, id.z) == 15 ||
                            GetLight(id.x, id.y, id.z + 1) == 15 ||
                            GetLight(id.x, id.y, id.z - 1) == 15)
                            SetLamp(id, 14);
                    }
                }
            }
            UpdateAllUvs3();
        }

        public void OnSetBlock(Vector3Int position)
        {
            SetLight(position, 0);
            for (int i = 0; i < _neighbors.Length; i++)
                TryDeleteLight(position + _neighbors[i]);
            position.y--;
            for (; position.y >= 0; position.y--)
            {
                if (_chunk.MiniBlocksValue[position.x, position.y, position.z] < 255)
                {
                    SetLight(position, 0);
                    for (int i = 0; i < _neighbors.Length; i++)
                        TryDeleteLight(position + _neighbors[i]);
                }
                else break;
            }

            while (_lightsForDelete.Count != 0)
            {
                position = _lightsForDelete.Dequeue();
                TryDeleteLight(position);
            }
            while (_lightsForSet.Count != 0)
            {
                position = _lightsForSet.Dequeue();
                TrySetLightForNeighbors(position);
            }
            UpdateAllUvs3();
        }

        public void OnDeleteBlock(Vector3Int position)
        {
            if(position.y == Settings.ChunkSize.y - 1 || _chunk.BlockSkyLights[position.x, position.y + 1, position.z] == 15)
            {
                for(; position.y >= 0; position.y--)
                {
                    if (_chunk.MiniBlocksValue[position.x, position.y, position.z] < 255)
                        SetLamp(position, 15);
                    else break;
                }
            }
            else
            {
                TrySetLight(position);
                while (_lightsForSet.Count != 0)
                {
                    position = _lightsForSet.Dequeue();
                    TrySetLightForNeighbors(position);
                }
            }
            UpdateAllUvs3();
        }

        private void SetLamp(Vector3Int position, byte force)
        {
            _lampCurrentPosition = new Vector2Int(position.x, position.z);
            SetLight(position, force);

            while (_lightsForSet.Count != 0)
            {
                position = _lightsForSet.Dequeue();
                TrySetLightForNeighbors(position);
            }
        }

        private void TryDeleteLight(Vector3Int position)
        {
            if (position.y < 0 || position.y >= Settings.ChunkSize.y)
                return;

            if (GetMiniBlocksValue(position.x, position.y, position.z) == 255)
                return;

            int currentLight = GetLight(position.x, position.y, position.z);

            if (currentLight == 0 || currentLight == 15)
                return;

            Vector3Int neighborPosition;
            int neighborLight;

            for (int i = 0; i < _neighbors.Length; i++)
            {
                neighborPosition = position + _neighbors[i];

                if (neighborPosition.y < 0 || neighborPosition.y >= Settings.ChunkSize.y)
                    continue;

                neighborLight = GetLight(neighborPosition.x, neighborPosition.y, neighborPosition.z);

                if (neighborLight <= 0)
                    continue;

                if (neighborLight < currentLight)
                    _lightsForDelete.Enqueue(neighborPosition);
                else
                    _lightsForSet.Enqueue(neighborPosition);
            }

            SetLight(position, 0);
        }

        private void TrySetLightForNeighbors(Vector3Int position)
        {
            for (int i = 0; i < _neighbors.Length; i++)
                TrySetLight(position + _neighbors[i]);
        }

        private void TrySetLight(Vector3Int position)
        {
            if (position.y < 0 || position.y >= Settings.ChunkSize.y)
                return;

            if (Mathf.Abs(position.x - _lampCurrentPosition.x) > 15 || Mathf.Abs(position.z - _lampCurrentPosition.y) > 15)
                return;

            if (GetMiniBlocksValue(position.x, position.y, position.z) == 255)
                return;

            byte currentLight = GetLight(position.x, position.y, position.z);
            byte maxLight = currentLight;
            byte light;

            for (int i = 0; i < _neighbors.Length; i++)
            {
                if (position.y == 0 || position.y == Settings.ChunkSize.y - 1)
                    continue;

                light = GetLight(position.x + _neighbors[i].x, position.y + _neighbors[i].y, position.z + _neighbors[i].z);
                if (light == 0)
                    continue;
                light -= 1;
                if (light > maxLight)
                    maxLight = light;
            }

            if (maxLight > currentLight)
                SetLight(position, maxLight);
        }

        private void SetLight(Vector3Int position, byte lightForce)
        {
            if (lightForce > 0)
                _lightsForSet.Enqueue(position);
            int x = position.x;
            int y = position.y;
            int z = position.z;
            _chunk.GetChunk(ref x, ref z).BlockSkyLights[x, y, z] = lightForce;
        }

        private byte GetLight(int x, int y, int z) => _chunk.GetChunk(ref x, ref z).BlockSkyLights[x, y, z];

        private byte GetMiniBlocksValue(int x, int y, int z) => _chunk.GetChunk(ref x, ref z).MiniBlocksValue[x, y, z];

        private void UpdateAllUvs3()
        {
            UpdateUvs3(_chunk);
            UpdateUvs3(_chunk.LeftBackChunk);
            UpdateUvs3(_chunk.LeftForwardChunk);
            UpdateUvs3(_chunk.LeftChunk);
            UpdateUvs3(_chunk.RightBackChunk);
            UpdateUvs3(_chunk.RightForwardChunk);
            UpdateUvs3(_chunk.RightChunk);
            UpdateUvs3(_chunk.BackChunk);
            UpdateUvs3(_chunk.ForwardChunk);
        }

        private void UpdateUvs3(Chunk chunk)
        {
            Vector3Int id;
            byte light;
            int idX;
            int idZ;
            for (int i = 0; i < chunk.ChunkMesh.PositionQuads.Count; i++)
            {
                id = chunk.ChunkMesh.PositionQuads[i];
                idX = id.x;
                idZ = id.z;
                light = chunk.GetChunk(ref idX, ref idZ).BlockSkyLights[idX, id.y, idZ];
                chunk.ChunkMesh.Uvs3[i * 4] = new Vector2(light / 16f, 0);
                chunk.ChunkMesh.Uvs3[i * 4 + 1] = new Vector2(light / 16f, 1);
                chunk.ChunkMesh.Uvs3[i * 4 + 2] = new Vector2((light + 1) / 16f, 1);
                chunk.ChunkMesh.Uvs3[i * 4 + 3] = new Vector2((light + 1) / 16f, 0);
            }
        }
    }
}