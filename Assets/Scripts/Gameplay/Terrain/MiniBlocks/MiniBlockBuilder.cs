using UnityEngine;
using UnityEngine.Events;

namespace MyGame.Gameplay.Terrain.MiniBlock
{
    public sealed class MiniBlockBuilder
    {
        private readonly MiniBlocksController _controller;

        public MiniBlockBuilder(MiniBlocksController controller)
        {
            _controller = controller;
        }

        public void Build(Vector3 pointPostition, Vector3 normalPosition, Vector3Int id)
        {
            Vector3 positionInBlock;
            if (normalPosition.x + normalPosition.y + normalPosition.z > 0)
            {
                positionInBlock = (pointPostition - Vector3Int.FloorToInt(pointPostition + normalPosition * 0.01f)) * 2;
                positionInBlock = Vector3Int.FloorToInt(positionInBlock + normalPosition * 0.01f);
            }
            else
            {
                positionInBlock = (pointPostition - Vector3Int.FloorToInt(pointPostition - normalPosition * 0.01f)) * 2;
                positionInBlock = Vector3Int.FloorToInt(positionInBlock - normalPosition * 0.01f);
            }
            positionInBlock /= 2;

            if (normalPosition.x == -1) positionInBlock.x = 0.5f - positionInBlock.x;
            else if (normalPosition.y == -1) positionInBlock.y = 0.5f - positionInBlock.y;
            else if (normalPosition.z == -1) positionInBlock.z = 0.5f - positionInBlock.z;

            _controller.TryBuildMiniCube(id, positionInBlock);
        }
    }
}