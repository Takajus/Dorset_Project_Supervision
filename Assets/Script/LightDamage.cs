using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDamage : MonoBehaviour
{
    [SerializeField] private Light _lightSource;
    [SerializeField] private GameObject _lightDetection;
    private PlayerMovement _playerMov;
    private float _playerLightDist;
    private Vector3 _lightPos, _playerLightDir;
    private Ray _ray;
    private RaycastHit _hit;
    [SerializeField] private LayerMask _wallMask;
    private bool _bFirstWarning, _bIslosingHealth;
    [SerializeField] private float _lifeTime, _damage;

    private void Awake()
    {
        _playerMov = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        _lightPos = _lightSource.transform.position;
        _bFirstWarning = true;
    }

    private void Update()
    {
        _playerLightDir = _lightSource.transform.forward.normalized;
        _playerLightDist = Vector3.Distance(_lightPos, _lightDetection.transform.position);

        if(!Physics.Raycast(_lightDetection.transform.position, -1f *_playerLightDir, _playerLightDist, _wallMask))
        {
            print("touch");

            if (_bFirstWarning)
            {
                _playerMov._bIsKnockBack = true;
                _bFirstWarning = false;
            }

            if (!_bIslosingHealth && _playerMov.life > 0)
            {
                StartCoroutine(LoseHealth());
            }

        }
    }

    private IEnumerator LoseHealth()
    {
        _bIslosingHealth = true;
        _playerMov.life -= _damage;
        print(_playerMov.life);

        yield return new WaitForSeconds(_lifeTime);

        _bIslosingHealth = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_lightDetection.transform.position, _lightPos);
    }
}
