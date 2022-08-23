using UnityEngine;

namespace DimensionLoop.Level3
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _horizontalSpeed = 2f, _verticalSpeed = 2f;
        
        private float _xAngle, _yAngle, _xRotation, _yRotation;
        
        private Camera _cam;
        private CharacterController _player;

        private void Start()
        {
            _xAngle = 0;
            _yAngle = 0;
            Cursor.lockState = CursorLockMode.Locked;
            transform.rotation = Quaternion.Euler(_yAngle, _xAngle, 0);
            _cam = GetComponentInChildren<Camera>();
            _player = GetComponentInParent<CharacterController>();
        }

        private void Update()
        {
            var mouseX = Input.GetAxis("Mouse X") * _horizontalSpeed;
            var mouseY = Input.GetAxis("Mouse Y") * _verticalSpeed;

            _yRotation += mouseX;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90, 90);

            _cam.transform.eulerAngles = new Vector3(_xRotation, _yRotation, 0.0f);
            transform.parent.rotation = Quaternion.Euler(0.0f, _yRotation, 0.0f);

            if (Input.GetButton("Jump") && _xRotation < -75)
                _player.cc.Move(Vector3.up * 0.75f);
        }
    }
}
