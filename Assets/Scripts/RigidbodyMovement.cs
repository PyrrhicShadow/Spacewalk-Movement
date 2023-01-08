using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class RigidbodyMovement : MonoBehaviour {
    [SerializeField] Transform playerCamera; 
    [SerializeField] Rigidbody playerBody; 
    [SerializeField] GameManager gameManager; 
    [SerializeField] PlayerInput playerInput; 
    [Space]
    [SerializeField] float acceleration; 
    [SerializeField] float xSensitivity; 
    [SerializeField] float ySensitivity; 
    [SerializeField] float jumpForce; 
    private bool gravity = true; 

    private Vector3 playerMovementInput; 
    private Vector2 playerLookInput; 
    private InputAction move; 

    private void Awake() {
        // move = playerInput.actionEvents.
    }

    private void FixedUpdate() {
        playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")); 
        playerLookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); 
        Move(); 
        Look(); 
    }

    public void Roll() {

    }

    public void Move() {
        Vector3 MoveVector = transform.TransformDirection(playerMovementInput) * acceleration;
        playerBody.velocity = new Vector3(MoveVector.x, playerBody.velocity.y, MoveVector.z);  
    }

    public void Look() {

    }

    public void OnGravityEnable() {
        playerInput.currentActionMap = playerInput.actions.FindActionMap("Gravity"); 
        playerBody.useGravity = true; 
    }

    public void OnGravityDisable() {
        playerInput.currentActionMap = playerInput.actions.FindActionMap("Space"); 
        playerBody.useGravity = false; 
    }
}
