using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] Vector3 gravityValue; 
    [SerializeField] bool _gravity; 
    [SerializeField] RigidbodyMovement playerBody; 

    public bool gravity { get { return _gravity; } private set { _gravity = value; } }

    private void Start() {
        if(gravity) {
            EnableGravity(); 
        }
        else {
            DisableGravity(); 
        }
    }

    [ContextMenu("Disable Gravity")]
    public void DisableGravity() {
        Physics.gravity = Vector3.zero; 
        gravity = false; 
        playerBody.OnGravityDisable(); 
    }

    [ContextMenu("Enable Gravity")]
    public void EnableGravity() {
        Physics.gravity = gravityValue; 
        gravity = true; 
        playerBody.OnGravityEnable(); 
    }
}
