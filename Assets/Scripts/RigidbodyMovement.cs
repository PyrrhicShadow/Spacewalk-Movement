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
    [SerializeField] float minSpeed; 

    private Vector2 move; 
    private Vector2 look; 
    private Vector3 targetVelocityLerp; 
    private Vector3 targetRotationLerp; 
    private float lookRotation; 
    [SerializeField] bool isGrounded; 
    private float stepTime = 0.1f; 

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
        // if (playerBody.velocity.sqrMagnitude < minSpeed) {
        //     playerBody.velocity = Vector3.zero; 
        // }
        // if (playerBody.angularVelocity.sqrMagnitude < minSpeed) {
        //     playerBody.angularVelocity = Vector3.zero; 
        // }
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
        // Vector3.ClampMagnitude(velocityChange, maxForce); 

        playerBody.AddForce(velocityChange, ForceMode.Acceleration); 
    }

    private void FixedMove() {
        // Find target velocity 
        Vector3 currentVelocity = playerBody.velocity; 
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y); 
        targetVelocity *= speed; 

        // Align direction 
        targetVelocity = transform.TransformDirection(targetVelocity); 
        targetVelocityLerp = Vector3.Lerp(targetVelocityLerp, targetVelocity, snappiness); 

        // Calculate forces 
        // Vector3 velocityChange = (targetVelocity - currentVelocity); 
        Vector3 velocityChange = (targetVelocityLerp - currentVelocity); 
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
        // Find target torque
        Vector3 currentTorque = playerBody.angularVelocity; 
        Vector3 targetTorque = new Vector3(-look.y, look.x, 0); 

        // Calculate forces 
        Vector3 torqueChange = (targetTorque - currentTorque); 
        torqueChange = new Vector3(torqueChange.x, torqueChange.y, 0); 

        // Limit force
        Vector3.ClampMagnitude(torqueChange, maxForce); 
        playerBody.AddRelativeTorque(torqueChange, ForceMode.Acceleration); 

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
        // transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0); 

        playerBody.AddForce(Vector3.down, ForceMode.Impulse); 
        playerInput.enabled = false;
        playerBody.useGravity = false; 
        StartCoroutine(AlignToGravity()); 
    }

    public void OnGravityDisable() {
        playerInput.currentActionMap = playerInput.actions.FindActionMap("Space"); 
        playerBody.freezeRotation = false; 
        transform.eulerAngles = playerCamera.eulerAngles; 
    }

    private IEnumerator AlignToGravity() {
        Debug.Log("Local gravity detected.");
        // Find target rotation
        Vector3 currentRotation = playerBody.rotation.eulerAngles; 
        Vector3 targetRotation = new Vector3(0, transform.eulerAngles.y, 0); 
        while (currentRotation != targetRotation) {
            // Update current rotation 
            currentRotation = playerBody.rotation.eulerAngles; 

            // Calculate this step
            targetRotationLerp = Vector3.Lerp(targetRotationLerp, targetRotation, snappiness); 

            // Calculate forces 
            Vector3 rotationChange = (targetRotationLerp - currentRotation); 
            rotationChange = new Vector3(rotationChange.x, 0, rotationChange.z); 

            // Dampen forces 
            Vector3.Normalize(rotationChange); 

            // Apply change
            playerBody.AddRelativeTorque(rotationChange, ForceMode.Acceleration);
            yield return new WaitForSeconds(stepTime); 
        }
        playerBody.useGravity = true; 
        playerInput.enabled = true; 
    }
}
