using UnityEngine;

namespace MyGame.Gameplay
{
    [CreateAssetMenu(menuName = "SO/AllBlocks")]
    public sealed class AllBlocks : ScriptableObject
    {
        [SerializeField] private Block[] _blocks;
        [SerializeField] private Vector2 _textureSize;

        public Block GetBlock(int id) => _blocks[id];
        public int BlockLenth => _blocks.Length;
        public Vector2 TextureSize => _textureSize;
    }

    [System.Serializable]
    public sealed class Block
    {
        public string name;
        public Vector2Int upTexturePosition;
        public Vector2Int sideTexturePosition;
        public Vector2Int downTexturePosition;
        public bool isTransparent;
        public bool isLight;
    }
}