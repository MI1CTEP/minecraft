using UnityEngine;

namespace MyGame.Gameplay.Terrain.MiniBlock
{
    public sealed class HalfBlockBuilder
    {
        private readonly MiniBlocksController _controller;

        public HalfBlockBuilder(MiniBlocksController controller)
        {
            _controller = controller;
        }

        public void Build(Vector3 pointPostition, Vector3 normalPosition, Vector3Int id)
        {
            Vector3 positionInBlock;
            if (normalPosition.x + normalPosition.y + normalPosition.z > 0)
                positionInBlock = (pointPostition - Vector3Int.FloorToInt(pointPostition + normalPosition * 0.01f));
            else
                positionInBlock = (pointPostition - Vector3Int.FloorToInt(pointPostition - normalPosition * 0.01f));

            if (normalPosition.x == -1) positionInBlock.x = 0.5f - positionInBlock.x;
            else if (normalPosition.y == -1) positionInBlock.y = 0.5f - positionInBlock.y;
            else if (normalPosition.z == -1) positionInBlock.z = 0.5f - positionInBlock.z;

            Vector3Int posInt = Vector3Int.RoundToInt(positionInBlock * 200);

            if (posInt.x == 0 && (posInt.y + posInt.z <= 200 || posInt.y < 150 && posInt.z < 150) ||
               posInt.y == 0 && (posInt.x + posInt.z <= 200 || posInt.x < 150 && posInt.z < 150) ||
               posInt.z == 0 && (posInt.x + posInt.y <= 200 || posInt.x < 150 && posInt.y < 150) ||
               posInt.x == 100 && posInt.y + posInt.z <= 200 && (posInt.y <= 50 || posInt.z <= 50) ||
               posInt.y == 100 && posInt.x + posInt.z <= 200 && (posInt.x <= 50 || posInt.z <= 50) ||
               posInt.z == 100 && posInt.x + posInt.y <= 200 && (posInt.x <= 50 || posInt.y <= 50))
                _controller.TryBuildMiniCube(id, new Vector3(0, 0, 0));
            else
                _controller.TryBuildMiniCube(id, new Vector3(0.5f, 0.5f, 0.5f));

            if (posInt.x == 100 && (posInt.y + posInt.z <= 200 || posInt.y < 150 && posInt.z < 150) ||
                posInt.y == 0 && (posInt.x > posInt.z || posInt.x >= 50 && posInt.z < 150) ||
                posInt.z == 0 && (posInt.x > posInt.y || posInt.x >= 50 && posInt.y < 150) ||
                posInt.x == 0 && posInt.y + posInt.z <= 200 && (posInt.y <= 50 || posInt.z <= 50) ||
                posInt.y == 100 && posInt.x > posInt.z && (posInt.x > 150 || posInt.z <= 50) ||
                posInt.z == 100 && posInt.x > posInt.y && (posInt.x > 150 || posInt.y <= 50))
                _controller.TryBuildMiniCube(id, new Vector3(0.5f, 0, 0));
            else
                _controller.TryBuildMiniCube(id, new Vector3(0, 0.5f, 0.5f));

            if (posInt.x == 0 && (posInt.y > posInt.z || posInt.y >= 50 && posInt.z < 150) ||
               posInt.y == 100 && (posInt.x + posInt.z <= 200 || posInt.x < 150 && posInt.z < 150) ||
               posInt.z == 0 && (posInt.y > posInt.x || posInt.x < 150 && posInt.y >= 50) ||
               posInt.x == 100 && posInt.y > posInt.z && (posInt.y > 150 || posInt.z <= 50) ||
               posInt.y == 0 && posInt.x + posInt.z <= 200 && (posInt.x <= 50 || posInt.z <= 50) ||
               posInt.z == 100 && posInt.y > posInt.x && (posInt.x <= 50 || posInt.y > 150))
                _controller.TryBuildMiniCube(id, new Vector3(0, 0.5f, 0));
            else
                _controller.TryBuildMiniCube(id, new Vector3(0.5f, 0, 0.5f));

            if (posInt.x == 0 && (posInt.z > posInt.y || posInt.y < 150 && posInt.z >= 50) ||
               posInt.y == 0 && (posInt.z > posInt.x || posInt.x < 150 && posInt.z >= 50) ||
               posInt.z == 100 && (posInt.x + posInt.y <= 200 || posInt.x < 150 && posInt.y < 150) ||
               posInt.x == 100 && posInt.z > posInt.y && (posInt.y <= 50 || posInt.z > 150) ||
               posInt.y == 100 && posInt.z > posInt.y && (posInt.x <= 50 || posInt.z > 150) ||
               posInt.z == 0 && posInt.x + posInt.y <= 200 && (posInt.x <= 50 || posInt.y <= 50))
                _controller.TryBuildMiniCube(id, new Vector3(0, 0, 0.5f));
            else
                _controller.TryBuildMiniCube(id, new Vector3(0.5f, 0.5f, 0));
        }
    }
}