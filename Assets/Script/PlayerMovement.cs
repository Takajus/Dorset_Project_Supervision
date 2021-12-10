using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    #region Variable

    [Header("Player Movement")]

    private CharacterController _controller;
    [Tooltip("Mettre la Camera avec la bonne position de d�part")]
    [SerializeField] private GameObject _camera;
    [Tooltip("Vitesse de d�placement debout")]
    [SerializeField] private float speed = 6f;
    [Tooltip("Vitesse de d�placement accroupi")]
    [SerializeField] private float crouchSpeed = 3f;
    private float currentSpeed;
    [Tooltip("TOUCHE ENCORE MOINS !!")]
    [SerializeField] private float gravity = -9.18f;
    [Tooltip("PAS TOUCHE !!")]
    [SerializeField] private float _knockBackForce = 5f;
    [HideInInspector] public bool _bIsKnockBack, _bKnockOntTime;
    [Tooltip("Force du saut")]
    [SerializeField] private float _jumpHeight = 3f;
    [Tooltip("PAS TOUCHE !!")]
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private bool _bIsGrounded;
    [SerializeField] private float _groundDistance = 0.4f;
    private Vector3 _velocity;
    [Tooltip("PAS TOUCHE !!")]
    [SerializeField] private Transform _GroundCheckPos;
    [Tooltip("PAS TOUCHE !!")]
    [SerializeField] private LayerMask GroundMask;

    private bool _bIsCrouch;

    [Header("Player Action")]

    //[SerializeField] private bool test = false;
    [Tooltip("PAS TOUCHE !!")]
    [SerializeField] private Animator _anim;
    [HideInInspector] public float currentLife;
    [Tooltip("MODIFICATION OK ^^")]
    [SerializeField] private float _life;

    [Header("Camera")]

    [Tooltip("Mettre le GameObject avec la bonne Position et Rotation pour la camera d'�nigme")]
    [SerializeField] private Transform _CameraScene;
    private Vector3 _OriginalCamPos;
    private float _timer = 0;
    private bool _bCameraLock;

    #endregion

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _OriginalCamPos = new Vector3(transform.position.x, _camera.transform.position.y, _camera.transform.position.z);
        currentLife = _life;
        currentSpeed = speed;
        _bIsKnockBack = false;
        _bKnockOntTime = true;
    }

    void Update()
    {
        _bIsGrounded = Physics.CheckSphere(_GroundCheckPos.position, _groundDistance, GroundMask);
        _anim.SetBool("bGrounded", _bIsGrounded);

        if (_bIsGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += gravity * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);

        if (!_bIsKnockBack)
        {
            MovementInput();
        }
        else if (_bIsKnockBack)
        {
            StartCoroutine(KnockBack());
        }
        

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
        //_bIsGrounded = Physics.CheckSphere(_GroundCheckPos.position, _groundDistance, GroundMask);

        /*_anim.SetBool("bGrounded", _bIsGrounded);

        if (_bIsGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }*/

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

        /*_velocity.y += gravity * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);*/

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

    public IEnumerator KnockBack()
    {
        if (_bKnockOntTime)
        {
            //_bKnockOntTime = false;
            Vector3 direction = (transform.forward - transform.position).normalized;
            direction.y = 1f;

            _controller.Move(direction * Time.deltaTime * _knockBackForce);

            //_rb.AddForce(direction * _knockBackForce, ForceMode.Impulse);

            yield return new WaitForSeconds(0.2f);

            if (_bIsGrounded)
            {
                _bIsKnockBack = false;
                _bKnockOntTime = false;
            }
                
        }
        
        
    }
}
