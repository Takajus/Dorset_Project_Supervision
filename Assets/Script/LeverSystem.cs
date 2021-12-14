using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverSystem : MonoBehaviour
{
    [SerializeField] private float _minimumHeldDuration;
    private bool _bCanTurn, _bKeyHeld, _bOneTime;
    private float _spacePressedTime;

    private void OnTriggerEnter(Collider player)
    {
        if(player.tag == "Player")
        {
            _bCanTurn = true;
            _bOneTime = false;
            print("Trigger Enter");
        }
    }

    private void OnTriggerExit(Collider player)
    {
        if (player.tag == "Player")
        {
            _bCanTurn = false;
            _bOneTime = true;
            print("Trigger Exit");
        }
    }

    private void Update()
    {
        if (_bCanTurn)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Use has pressed the Space key. We don't know if they'll release or hold it, so keep track of when they started holding it.
                _spacePressedTime = Time.timeSinceLevelLoad;
                _bKeyHeld = false;
            }
            else if (Input.GetKeyUp(KeyCode.E)) {
                if (!_bKeyHeld)
                {
                    // Player has released the space key without holding it.
                    // TODO: Perform the action for when Space is pressed.
                }
                _bKeyHeld = false;

            }

            if (Input.GetKey(KeyCode.E)) {
                if (Time.timeSinceLevelLoad - _spacePressedTime > _minimumHeldDuration)
                {
                    // Player has held the Space key for .25 seconds. Consider it "held"
                    _bKeyHeld = true;
                    print("holding complete");
                }
            }
        }
        
    }
}
