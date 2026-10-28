using UnityEngine;
using UnityEngine.UI;
using MyGame.Gameplay.Terrain;

namespace MyGame.Gameplay
{
    public sealed class BlockDestroyer : MonoBehaviour
    {
        [SerializeField] private Transform _destroyView;
        [SerializeField] private Transform _selectedView;
        [SerializeField] private Material _destroyMaterial;

        private Vector2Int _oldChunkId = Vector2Int.one * int.MaxValue;
        private Vector3Int _oldBlockId = Vector3Int.one * int.MaxValue;
        private Vector3Int _currentBlockId = Vector3Int.zero;
        private float _destroyProgress;
        private int _setBlockId;

        private void Update()
        {
            _selectedView.position = new Vector3(0, -1000, 0);
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 5) && hit.transform.gameObject.TryGetComponent(out Chunk chunk))
            {
                Vector3 point = hit.point - chunk.transform.position;
                _currentBlockId.x = Mathf.RoundToInt(hit.point.x - chunk.transform.position.x - 0.5f - hit.normal.x / 2);
                _currentBlockId.y = Mathf.RoundToInt(hit.point.y - 0.5f - hit.normal.y / 2);
                _currentBlockId.z = Mathf.RoundToInt(hit.point.z - chunk.transform.position.z - 0.5f - hit.normal.z / 2);

                if (Input.GetMouseButtonDown(0))
                {
                    chunk.Click(point, hit.normal, true);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    chunk.Click(point, hit.normal, false);
                }

                _selectedView.position = _currentBlockId + chunk.transform.position + Vector3.one * 0.5f;
            }

        }

        public void UpdateDestroyView()
        {
            int step = (int)(_destroyProgress * 18);
            if(step == 0)
            {
                _destroyMaterial.mainTextureScale = new Vector2(0, 0);
                _destroyMaterial.mainTextureOffset = new Vector2(0, 0);
            }
            else if(step == 17)
            {
                return;
            }
            else
            {
                step--;
                _destroyMaterial.mainTextureScale = new Vector2(0.25f, 0.25f);
                _destroyMaterial.mainTextureOffset = new Vector2(0.25f * step, 0.25f * (step / 4));
            }
        }
    }
}