using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmbrellaHolderSys : MonoBehaviour
{
    [SerializeField] private GameObject _umbrellaHolder, _umbrella;
    [SerializeField] private float _umbrellaXPos;
    private Vector3 _umbrellaPos;

    private void Start()
    {
        _umbrellaPos = _umbrellaHolder.transform.position;
    }

    private void Update()
    {
        _umbrellaHolder.transform.position = transform.position;
    }
}
