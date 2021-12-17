using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushSystem : MonoBehaviour
{
    [SerializeField] private float _forceMagnitude;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rbObject = hit.collider.attachedRigidbody;
        
        if(rbObject != null)
        {
            if(rbObject.tag == "ObjectPush")
            {
                Vector3 forceDirection = hit.gameObject.transform.position - transform.position;//new Vector3(hit.gameObject.transform.position.x - transform.position.x, 0, 0);
                forceDirection.y = 0;
                forceDirection.z = 0;
                forceDirection.Normalize();

                rbObject.AddForceAtPosition(_forceMagnitude * forceDirection, transform.position, ForceMode.Impulse);
            }
        }
    }
}
