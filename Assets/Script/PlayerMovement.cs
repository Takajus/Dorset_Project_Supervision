using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    #region Variable

    [Header("Player Movement")]

    private CharacterController _controller;
    [SerializeField] private GameObject _camera;

    [SerializeField] private float speed = 6f;
    public float gravity = -9.18f;
    [SerializeField] private float _jumpHeight = 3f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [SerializeField] private bool _bIsGrounded;
    [SerializeField] private float _groundDistance = 0.4f;
    private Vector3 _velocity;
    [SerializeField] private Transform _GroundCheckPos;
    [SerializeField] private LayerMask GroundMask;

    [Header("Player Action")]

    [SerializeField] private bool test = false;

    [Header("Camera")]

    [SerializeField] private bool _bCameraLock;
    [SerializeField] private Transform CameraScene;
    private float timer = 0;

    #endregion

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        MovementInput();

        if (!_bCameraLock)
        {
            _camera.transform.position = new Vector3(transform.position.x, _camera.transform.position.y, _camera.transform.position.z);
        }
        else if (_bCameraLock)
        {
            // Faire une transition fluide pour le placement de la camera
            //_camera.transform.position = CameraScene.position;
            //_camera.transform.rotation = CameraScene.rotation;
            if (_camera.transform.position != CameraScene.position)
            {
                timer += Time.deltaTime;
                float ration = timer / 3;

                //_camera.transform.position = Vector3.MoveTowards(_camera.transform.position, CameraScene.position, 25 * Time.deltaTime);
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, CameraScene.position, ration);
            }
            else if (_camera.transform.position == CameraScene.position)
            {
                _camera.transform.position = CameraScene.position;
            }

        }
    }

    private void MovementInput()
    {
        _bIsGrounded = Physics.CheckSphere(_GroundCheckPos.position, _groundDistance, GroundMask);

        if(_bIsGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            _controller.Move(direction * Time.deltaTime * speed);
        }

        if(Input.GetButtonDown("Jump") && _bIsGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * gravity);
        }

        _velocity.y += gravity * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "SceneTransition")
        {
            _bCameraLock = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SceneTransition")
        {
            _bCameraLock = false;
        }
    }
}
