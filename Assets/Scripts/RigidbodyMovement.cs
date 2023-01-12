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
    private float lookRotation; 
    [SerializeField] bool isGrounded; 
    private float stepTime = 0.5f; 

    private bool leftRollPressed; 
    private bool rightRollPressed; 
    private bool crouched; 
    private bool ascending;  

    private Vector3 normalScale; 

    private void Awake() {
        normalScale = transform.localScale; 
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
            Roll(); 
        }
        // DeadZone(); 
    }

    private void LateUpdate() {
        if (gameManager.gravity) {
            FixedLook(); 
            Crouch(); 
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

        if (ascending) {
            Ascend(); 
        }
        if (crouched) {
            Descend(); 
        }
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
        if (crouched) {
            transform.localScale = Vector3.one; 
        }
        else {
            transform.localScale = normalScale; 
        }
    }

    private void Ascend() {
        playerBody.AddRelativeForce(Vector3.up * jumpForce, ForceMode.Acceleration); 
    }

    private void Descend() {
        playerBody.AddRelativeForce(Vector3.down * jumpForce, ForceMode.Acceleration); 
    }

    private void Roll() {
        if (rightRollPressed && leftRollPressed) {
            playerBody.velocity = Vector3.zero; 
            playerBody.AddRelativeTorque(playerBody.angularVelocity, ForceMode.Acceleration); 
        }
        else {
            int direction = 0; 
            if (rightRollPressed) {
                direction = 1; 
            }
            else if (leftRollPressed) {
                direction = -1; 
            }
            playerBody.AddRelativeTorque(Vector3.forward * direction, ForceMode.Acceleration); 
        }
    }

    public void SetGrounded(bool state) {
        isGrounded = state; 
    }

    public void OnRollLeft(InputAction.CallbackContext context) {
            leftRollPressed = context.ReadValueAsButton(); 
    }

    public void OnRollRight(InputAction.CallbackContext context) {
            rightRollPressed = context.ReadValueAsButton(); 
    }

    public void OnMove(InputAction.CallbackContext context) {
        move = context.ReadValue<Vector2>(); 
    }

    public void OnLook(InputAction.CallbackContext context) {
        look = context.ReadValue<Vector2>(); 
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (gameManager.gravity) {
            Jump(); 
        }
        else {
            ascending = context.ReadValueAsButton(); 
        }
    }

    public void OnCrouch(InputAction.CallbackContext context) {
        crouched = context.ReadValueAsButton(); 
    }

    public void OnGravityEnable() {
        Debug.Log("Normal gravity detected.");
        playerInput.currentActionMap = playerInput.actions.FindActionMap("Gravity"); 
        playerBody.freezeRotation = true; 
        playerBody.velocity = Vector3.zero; 
        playerBody.angularVelocity = Vector3.zero; 
        // transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0); 
        playerBody.useGravity = false; 
        playerInput.enabled = false; 
        StartCoroutine(AlignToGravity(transform.rotation)); 
    }

    public void OnGravityDisable() {
        Debug.Log("Micro gravity detected.");
        playerInput.currentActionMap = playerInput.actions.FindActionMap("Space"); 
        playerBody.freezeRotation = false; 
        transform.eulerAngles = playerCamera.eulerAngles; 
    }

    private IEnumerator AlignToGravity(Quaternion currentRotation) {
        // Find target rotation
        Quaternion targetRotation = Quaternion.Euler(0, playerBody.transform.eulerAngles.y, 0); 

        float progress = 0f; 

        while (progress < 1f) {
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, progress);
            playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, Quaternion.Euler(0, playerBody.transform.eulerAngles.y, 0), progress); 
            progress += Time.deltaTime * stepTime; 

            if (progress <= 1f) {
                yield return new WaitForFixedUpdate(); 
            }
        }
        playerBody.AddRelativeForce(Vector3.forward, ForceMode.VelocityChange); 
        playerBody.useGravity = true; 
        playerInput.enabled = true; 
    }

    private void DeadZone() {
        if (playerBody.velocity.sqrMagnitude < minSpeed) {
            playerBody.velocity = Vector3.zero; 
        }
        if (playerBody.angularVelocity.sqrMagnitude < minSpeed) {
            playerBody.angularVelocity = Vector3.zero; 
        }
    }
}
