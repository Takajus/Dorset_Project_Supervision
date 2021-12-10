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
    [SerializeField] private float crouchSpeed;
    private float currentSpeed;
    public float gravity = -9.18f;
    [SerializeField] private float _jumpHeight = 3f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [SerializeField] private bool _bIsGrounded;
    [SerializeField] private float _groundDistance = 0.4f;
    private Vector3 _velocity;
    [SerializeField] private Transform _GroundCheckPos;
    [SerializeField] private LayerMask GroundMask;

    private bool _bIsCrouch;

    [Header("Player Action")]

    //[SerializeField] private bool test = false;
    [SerializeField] private Animator _anim;

    [Header("Camera")]

    [SerializeField] private bool _bCameraLock;
    [SerializeField] private Transform _CameraScene;
    private Vector3 _OriginalCamPos;
    private float _timer = 0;

    #endregion

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _OriginalCamPos = new Vector3(transform.position.x, _camera.transform.position.y, _camera.transform.position.z);
        currentSpeed = speed;
    }

    void Update()
    {
        MovementInput();

        _OriginalCamPos.x = transform.position.x;

        #region Camera Transition

        if (!_bCameraLock)
        {

            if (_camera.transform.position.y != _OriginalCamPos.y && _camera.transform.position.z != _OriginalCamPos.z)
            {
                _timer += Time.deltaTime;
                float ration = _timer / 3;
                //_camera.transform.position = Vector3.MoveTowards(_camera.transform.position, CameraScene.position, 25 * Time.deltaTime);
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, _OriginalCamPos, ration);
            }
            else/* if (_camera.transform.position.y == _OriginalCamPos.y)*/
            {
                _camera.transform.position = new Vector3(_OriginalCamPos.x, _OriginalCamPos.y, _OriginalCamPos.z);
            }
            //_camera.transform.position = new Vector3(transform.position.x, _OriginalCamPos.y, _OriginalCamPos.z);
        }
        else if (_bCameraLock)
        {
            // Faire une transition fluide pour le placement de la camera
            //_camera.transform.position = CameraScene.position;
            //_camera.transform.rotation = CameraScene.rotation;
            if (_camera.transform.position != _CameraScene.position)
            {
                _timer += Time.deltaTime;
                float ration = _timer / 3;

                //_camera.transform.position = Vector3.MoveTowards(_camera.transform.position, CameraScene.position, 25 * Time.deltaTime);
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, _CameraScene.position, ration);
            }
            else/* if (_camera.transform.position == _CameraScene.position)*/
            {
                _camera.transform.position = _CameraScene.position;
            }

        }

        #endregion


    }

    private void MovementInput()
    {
        _bIsGrounded = Physics.CheckSphere(_GroundCheckPos.position, _groundDistance, GroundMask);

        _anim.SetBool("bGrounded", _bIsGrounded);

        if (_bIsGrounded && _velocity.y < 0)
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

            _controller.Move(direction * Time.deltaTime * currentSpeed);
        }

        if(Input.GetButtonDown("Jump") && _bIsGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * gravity);
            if (_bIsCrouch)
            {
                _bIsCrouch = false;
                _controller.height = 2f;
                _controller.center = new Vector3(0, 1f, 0);
                currentSpeed = speed;
                _anim.SetTrigger("CrouchEnd");

            }
        }

        _velocity.y += gravity * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);

        _anim.SetFloat("velocity", direction.magnitude);

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (_bIsCrouch)
            {
                _bIsCrouch = false;
                _controller.height = 2f;
                _controller.center = new Vector3(0, 1f, 0);
                currentSpeed = speed;
                _anim.SetTrigger("CrouchEnd");

            }
            else if (!_bIsCrouch)
            {
                _bIsCrouch = true;
                _controller.height = 1.4f;
                _controller.center = new Vector3(0, 0.7f, 0);
                currentSpeed = crouchSpeed;
                _anim.SetTrigger("CrouchStart");
            }
        }

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
