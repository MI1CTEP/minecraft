using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Gameplay
{
    public sealed class CurrentBlock : MonoBehaviour
    {
        [SerializeField] private AllBlocks _allBlocks;
        [SerializeField] private Text _text;
        [SerializeField] private Selected _selected;

        private int _setBlockId;
        private int _typeBlock;

        public static CurrentBlock Instance { get; set; }
        public int GetBlockId => _setBlockId + 1;
        public int TypeBlock => _typeBlock;

        private void Start()
        {
            Instance = this;
            UpdateText();
            _selected.SetTexture(_typeBlock);
        }

        private void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                ChangeSetBlock(1);
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                ChangeSetBlock(-1);
            else if (Input.GetMouseButtonDown(2))
                SwitchTypeBlock();
        }

        private void ChangeSetBlock(int pos)
        {
            _setBlockId += pos;
            if (_setBlockId < 0)
                _setBlockId = _allBlocks.BlockLenth - 1;
            else if (_setBlockId >= _allBlocks.BlockLenth)
                _setBlockId = 0;
            UpdateText();
        }

        private void SwitchTypeBlock()
        {
            _typeBlock++;
            if (_typeBlock > 3)
                _typeBlock = 0;
            _selected.SetTexture(_typeBlock);
            UpdateText();
        }

        private void UpdateText()
        {
            if (_typeBlock == 1)
                _text.text = _allBlocks.GetBlock(_setBlockId).name + " (Ступеньки)";
            else if (_typeBlock == 2)
                _text.text = _allBlocks.GetBlock(_setBlockId).name + " (Полублок)";
            else if(_typeBlock == 3)
                _text.text = _allBlocks.GetBlock(_setBlockId).name + " (Миниблок)";
            else
                _text.text = _allBlocks.GetBlock(_setBlockId).name + " (Блок)";
        }
    }
}