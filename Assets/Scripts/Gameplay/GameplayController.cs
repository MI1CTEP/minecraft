using UnityEngine;
using MyGame.Gameplay.Terrain;

namespace MyGame.Gameplay
{
    public sealed class GameplayController : MonoBehaviour
    {
        [SerializeField] private TerrainController _terrainController;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private Material _chunkMaterial;
        [SerializeField] private Transform _sun;
        [SerializeField, Range(0, 24)] private float _time;

        private float _lastTime = -1;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;

            _terrainController.Init(_playerController);
            _playerController.Init();
        }

        private void Update()
        {
            _time += Time.deltaTime * 0.1f;
            if (_time > 24)
                _time -= 24;

            _sun.eulerAngles = new Vector3(_time * 15 - 90, 0, 0);

            if (_time >= 5 && _time <= 7)
                _chunkMaterial.SetFloat("_Opacity", (_time - 5) / 2);
            else if (_time >= 17 && _time <= 19)
                _chunkMaterial.SetFloat("_Opacity", 1 - (_time - 17) / 2);
            else if (_time > 7 && _time < 17)
                _chunkMaterial.SetFloat("_Opacity", 1);
            else
                _chunkMaterial.SetFloat("_Opacity", 0);
        }
    }
}