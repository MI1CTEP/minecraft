using UnityEngine;
using UnityEngine.Events;

namespace MyGame.Gameplay
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _sensitivity = 500;
        [SerializeField] private float _walkSpeed = 10f;
        [SerializeField] private float _runSpeed = 20f;
        [SerializeField] private float _jumpForce = 8f;
        [SerializeField] private float _gravity = -9.81f;

        private CharacterController _characterController;
        private Vector2Int _idPosition = Vector2Int.zero;
        private float _rotationX;
        private float _rotationY;
        private float _verticalVelocity;
        private bool _isInited;
        private bool _isGrounded;
        private bool _isNewIdPosition;
        private bool _isStop;

        public UnityAction<Vector2Int> OnUpdatePosition { get; set; }

        public void Init()
        {
            transform.position = new Vector3(0, 100, 0);
            _characterController = GetComponent<CharacterController>();
            _isInited = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
                _isStop = !_isStop;

            if (!_isInited || _isStop) 
                return;

            _rotationX -= Input.GetAxisRaw("Mouse Y") * _sensitivity * Time.deltaTime;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);
            _cameraTransform.localEulerAngles = _rotationX * Vector3.right;

            _rotationY += Input.GetAxisRaw("Mouse X") * _sensitivity * Time.deltaTime;
            transform.eulerAngles = _rotationY * Vector3.up;

            Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
            move = transform.TransformDirection(move);
            if (Input.GetKey(KeyCode.LeftShift))
                move *= _runSpeed;
            else
                move *= _walkSpeed;

            if (_characterController.isGrounded)
            {
                _isGrounded = true;
            }
            else
            {
                _verticalVelocity -= _gravity * Time.deltaTime;
                _isGrounded = false;
            }

            if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
                _verticalVelocity = _jumpForce;

            move.y = _verticalVelocity;

            _characterController.Move(move * Time.deltaTime);

            UpdateIdPosition();
        }

        private void UpdateIdPosition()
        {
            if(Mathf.Abs(transform.position.x - _idPosition.x * Settings.ChunkSize.x) > Settings.ChunkSize.x)
            {
                _idPosition.x = Mathf.RoundToInt(transform.position.x / Settings.ChunkSize.x);
                _isNewIdPosition = true;
            }
            if (Mathf.Abs(transform.position.z - _idPosition.y * Settings.ChunkSize.z) > Settings.ChunkSize.z)
            {
                _idPosition.y = Mathf.RoundToInt(transform.position.z / Settings.ChunkSize.z);
                _isNewIdPosition = true;
            }
            if (_isNewIdPosition)
            {
                OnUpdatePosition?.Invoke(_idPosition);
                _isNewIdPosition = false;
            }
        }
    }
}