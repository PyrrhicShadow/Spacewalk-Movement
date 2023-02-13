using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PyrrhicSilva.Movement; 

public class GameManager : MonoBehaviour {
    [SerializeField] Vector3 gravityValue; 
    [SerializeField] bool _gravity; 
    [SerializeField] RigidbodyMovement playerBody; 
    [SerializeField] float gravityChangeInterval; 
    private float timer; 

    public bool gravity { get { return _gravity; } private set { _gravity = value; } }

    private void Start() {
        if(gravity) {
            EnableGravity(); 
        }
        else {
            DisableGravity(); 
        }
    }

    private void Update() {
        // GravityBouncing(); 
    }

    [ContextMenu("Disable Gravity")]
    public void DisableGravity() {
        Physics.gravity = Vector3.zero; 
        gravity = false; 
        playerBody.OnMicroGravity(); 
    }

    [ContextMenu("Enable Gravity")]
    public void EnableGravity() {
        Physics.gravity = gravityValue; 
        gravity = true; 
        playerBody.OnNormalGravity(); 
    }

    private void GravityBouncing() {
        if (timer < gravityChangeInterval) {
            timer += Time.deltaTime; 
        }
        else {
            if (gravity) {
                DisableGravity(); 
            }
            else {
                EnableGravity(); 
            }
            timer = 0; 
        }
    }
}
