using UnityEngine;

namespace MyGame.Gameplay
{
    public sealed class Selected : MonoBehaviour
    {
        [SerializeField] private Material _material;
        [SerializeField] private Texture[] _textures;

        public void SetTexture(int id)
        {
            _material.mainTexture = _textures[id];
        }
    }
}