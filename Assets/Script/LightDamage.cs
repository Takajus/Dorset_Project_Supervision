using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDamage : MonoBehaviour
{
    [SerializeField] private Light _lightSource;
    [SerializeField] private GameObject _player;
    private float _playerLightDist;
    private Vector3 _lightPos, _playerLightDir;
    private Ray _ray;
    private RaycastHit _hit;
    [SerializeField] private LayerMask _wallMask;

    private void Start()
    {
        _lightPos = _lightSource.transform.position;
    }

    private void Update()
    {
        _playerLightDir = _lightSource.transform.forward.normalized;
        _playerLightDist = Vector3.Distance(_lightPos, _player.transform.position);

        if(!Physics.Raycast(_player.transform.position, -1f *_playerLightDir, _playerLightDist, _wallMask))
        {
            print("touch");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_player.transform.position, _lightPos);
    }
}
