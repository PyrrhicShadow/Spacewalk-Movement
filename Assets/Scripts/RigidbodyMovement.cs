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
    [SerializeField] float speed; 
    [SerializeField] float xSensitivity; 
    [SerializeField] float ySensitivity; 
    [SerializeField] float jumpForce; 
    [SerializeField] float maxForce; 
    [SerializeField] float snappiness; 

    private Vector2 move; 
    private Vector2 look; 
    private Vector3 targetVelocityLerp; 
    private float lookRotation; 
    private bool isGrounded; 

    private bool leftRollPressed; 
    private bool rightRollPressed; 

    private void Awake() {

    }

    private void Start() {
        if (gameManager.gravity) {
            OnGravityEnable(); 
        }
        else {
            OnGravityDisable(); 
        }
    }

    private void FixedUpdate() {
        if (gameManager.gravity) {
            FixedMove(); 
        }
        else {
            FreeMove(); 
        }
    }

    private void LateUpdate() {
        if (gameManager.gravity) {
            FixedLook(); 
        }
        else {
            FreeLook(); 
        }
    }

    private void FreeMove() {
        // Find target velocity 
        Vector3 currentVelocity = playerBody.velocity; 
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y); 
        targetVelocity *= speed; 

        // Align direction 
        targetVelocity = transform.TransformDirection(targetVelocity); 
        // targetVelocityLerp = Vector3.Lerp(targetVelocityLerp, targetVelocity, snappiness); 

        // Calculate forces 
        Vector3 velocityChange = (targetVelocity - currentVelocity); 
        // Vector3 velocityChange = (targetVelocityLerp - currentVelocity); 
        // velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z); 

        // Limit force 
        Vector3.ClampMagnitude(velocityChange, maxForce); 

        playerBody.AddForce(velocityChange, ForceMode.Acceleration); 
    }

    private void FixedMove() {
        // Find target velocity 
        Vector3 currentVelocity = playerBody.velocity; 
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y); 
        targetVelocity *= speed; 

        // Align direction 
        targetVelocity = transform.TransformDirection(targetVelocity); 
        // targetVelocityLerp = Vector3.Lerp(targetVelocityLerp, targetVelocity, snappiness); 

        // Calculate forces 
        Vector3 velocityChange = (targetVelocity - currentVelocity); 
        // Vector3 velocityChange = (targetVelocityLerp - currentVelocity); 
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z); 

        // Limit force 
        Vector3.ClampMagnitude(velocityChange, maxForce); 

        playerBody.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void FixedLook() {
        // turn
        transform.Rotate(Vector3.up * look.x * xSensitivity); 

        // look 
        lookRotation += (-look.y * ySensitivity); 
        lookRotation = Mathf.Clamp(lookRotation, -90, 90); 
        playerCamera.transform.eulerAngles = new Vector3(lookRotation, playerCamera.transform.eulerAngles.y, playerCamera.transform.eulerAngles.z); 
    }

    private void FreeLook() {
        // turn
        // transform.Rotate(-look.y * ySensitivity, look.x * xSensitivity, 0); 
        Vector3 currentTorque = playerBody.angularVelocity; 
        Vector3 targetTorque = new Vector3(-look.y, look.x, 0); 

        Vector3 torqueChange = (targetTorque - currentTorque); 
        torqueChange = new Vector3(torqueChange.x, torqueChange.y, 0); 

        Vector3.ClampMagnitude(torqueChange, maxForce); 
        playerBody.AddRelativeTorque(torqueChange, ForceMode.Acceleration); 

        // look 
        playerCamera.transform.eulerAngles = transform.eulerAngles; 
    }

    private void Jump() {
        Vector3 jumpForces = Vector3.zero; 
        
        if (isGrounded) {
            jumpForces = Vector3.up * jumpForce; 
        }

        playerBody.AddForce(jumpForces, ForceMode.VelocityChange); 
    }

    private void Crouch() {

    }

    private void Ascend() {

    }

    private void Descend() {

    }

    private void Brake() {
        playerBody.velocity = Vector3.zero; 
        playerBody.angularVelocity = Vector3.zero; 
    }

    private void Roll(int direction) {
        playerBody.angularVelocity = Vector3.forward * direction; 
    }

    public void SetGrounded(bool state) {
        isGrounded = state; 
    }

    public void OnRollLeft(InputAction.CallbackContext context) {
        if(rightRollPressed) {
            Brake(); 
        }
        else {
            // rotate player z
            leftRollPressed = context.ReadValueAsButton(); 
            Roll(1); 
        }
    }

    public void OnRollRight(InputAction.CallbackContext context) {
        if (leftRollPressed) {
            Brake(); 
        }
        else {
            // rotate player z 
            rightRollPressed = context.ReadValueAsButton(); 
            Roll(-1); 
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        move = context.ReadValue<Vector2>(); 
    }

    public void OnLook(InputAction.CallbackContext context) {
        look = context.ReadValue<Vector2>(); 
    }

    public void OnJump(InputAction.CallbackContext context) {
        Jump(); 
    }

    public void OnCrouch(InputAction.CallbackContext context) {

    }

    public void OnGravityEnable() {
        playerInput.currentActionMap = playerInput.actions.FindActionMap("Gravity"); 
        playerBody.freezeRotation = true; 
        transform.Rotate(0, transform.eulerAngles.y, 0); 
        playerBody.WakeUp(); 
    }

    public void OnGravityDisable() {
        playerInput.currentActionMap = playerInput.actions.FindActionMap("Space"); 
        playerBody.freezeRotation = false; 
        transform.eulerAngles = playerCamera.eulerAngles; 
    }
}
