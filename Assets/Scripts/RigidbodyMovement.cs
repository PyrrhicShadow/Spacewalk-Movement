using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class RigidbodyMovement : MonoBehaviour {
    [SerializeField] Transform playerCamera; 
    [SerializeField] Rigidbody playerBody; 
    // [SerializeField] GameManager gameManager; 
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
    [SerializeField] bool isGrounded = false; 
    [SerializeField] bool isDiving = false; 
    private float stepTime = 0.5f; 

    private bool leftRollPressed; 
    private bool rightRollPressed; 
    private bool crouched; 
    private bool ascending;  
    private bool descending; 
    private Vector3 normalScale; 

    private void Awake() {
        normalScale = transform.localScale; 
    }

    private void Start() {
        if (playerInput.currentActionMap == playerInput.actions.FindActionMap("Space")) {
            OnMicroGravity(); 
        }
        else {
            OnNormalGravity(); 
        }
    }

    private void FixedUpdate() {
        if (playerInput.currentActionMap == playerInput.actions.FindActionMap("Space")) {
            FreeMove();
            Fly(); 
            Roll(); 
        }
        else if (playerInput.currentActionMap == playerInput.actions.FindActionMap("Water")) {
            FreeMove(); 
            // Buoyancy(); 
            Dive(); 
        }
        else {
            FixedMove(); 
        }
    }

    private void LateUpdate() {
        if (playerInput.currentActionMap == playerInput.actions.FindActionMap("Space")) {
            FreeLook(); 
        }
        else if (playerInput.currentActionMap == playerInput.actions.FindActionMap("Water")) {
            WaterLook(); 
        }
        else {
            FixedLook(); 
        }
    }

    private void DeadZone() {
        if (playerBody.velocity.sqrMagnitude < minSpeed) {
            playerBody.velocity = Vector3.zero; 
        }
        if (playerBody.angularVelocity.sqrMagnitude < minSpeed) {
            playerBody.angularVelocity = Vector3.zero; 
        }
    }

    /** Normal gravity or Fixed movement **/

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

        private void Jump() {
        Vector3 jumpForces = Vector3.zero; 
        
        if (isGrounded) {
            jumpForces = Vector3.up * jumpForce; 
        }

        playerBody.AddForce(jumpForces, ForceMode.VelocityChange); 
    }

    public void SetGrounded(bool state) {
        isGrounded = state; 
    }

    private void Crouch() {
        if (crouched) {
            transform.localScale = normalScale; 
            crouched = false; 
        }
        else {
            transform.localScale = Vector3.one; 
            crouched = true; 
        }
    }

    /** Microgravity or Free movement **/

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

        playerCamera.eulerAngles = transform.eulerAngles; 
    }

    private void Fly() {
        if (ascending) {
            playerBody.AddRelativeForce(Vector3.up, ForceMode.VelocityChange); 
        }
        if (descending) {
            playerBody.AddRelativeForce(Vector3.down, ForceMode.VelocityChange); 
        }
    }

    private void Roll() {
        if (rightRollPressed && leftRollPressed) {
            playerBody.velocity = Vector3.zero; 
            playerBody.angularVelocity = Vector3.zero;  
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

    /** Swimming and Water movement **/ 
    private void WaterLook() {
        // turn
        transform.Rotate(Vector3.up * look.x * xSensitivity); 

        // look 
        lookRotation += (-look.y * ySensitivity); 
        lookRotation = Mathf.Clamp(lookRotation, -90, 90); 
        transform.eulerAngles = new Vector3(lookRotation, transform.eulerAngles.y, 0); 
    }

    private void Dive() {
        if (ascending) {
            playerBody.AddForce(Vector3.up, ForceMode.VelocityChange); 
        }
        if (descending) {
            playerBody.AddForce(Vector3.down, ForceMode.VelocityChange); 
        }
    }

    private void Buoyancy() {
        if (!ascending && !descending) {
            if (playerBody.velocity.y < 0) {
                playerBody.AddForce(-Physics.gravity, ForceMode.Acceleration); 
            }
        }
    }

    /** Buttons **/

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
        if (playerInput.currentActionMap == playerInput.actions.FindActionMap("Gravity")) {
            Jump(); 
        }
        ascending = context.ReadValueAsButton(); 

    }

    public void OnCrouch(InputAction.CallbackContext context) {
        if (playerInput.currentActionMap == playerInput.actions.FindActionMap("Gravity")) {
            Crouch(); 
        }
        descending = context.ReadValueAsButton(); 
    }

    public void OnDive(InputAction.CallbackContext context) {
        if (isDiving) {
            isDiving = false; 
        }
        else {
            isDiving = true; 
        }
    }

    /** Changing movement types **/ 

    public void OnNormalGravity() {
        Debug.Log("Normal gravity detected.");
        playerInput.currentActionMap = playerInput.actions.FindActionMap("Gravity"); 
        playerBody.useGravity = true; 
        playerBody.freezeRotation = true; 
        playerBody.velocity = Vector3.zero; 
        playerBody.angularVelocity = Vector3.zero; 
        // transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0); 
        playerInput.enabled = false; 
        Crouch(); 
        StartCoroutine(AlignToGravity(transform.rotation)); 
    }

    // Microgravity always leads to normal gravity
    public void OnMicroGravity() {
        Debug.Log("Micro gravity detected.");
        playerInput.currentActionMap = playerInput.actions.FindActionMap("Space"); 
        playerBody.useGravity = false; 
        playerBody.freezeRotation = false; 
        transform.eulerAngles = playerCamera.eulerAngles; 
        crouched = true; 
    }

    // smoothly(ish) transition from free look to fixed look
    private IEnumerator AlignToGravity(Quaternion currentRotation) {
        // Find target rotation
        Quaternion targetRotation = Quaternion.Euler(0, playerBody.transform.eulerAngles.y, 0); 

        float progress = 0f; 

        while (progress < 1f) {
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, progress);
            playerCamera.rotation = Quaternion.Slerp(playerCamera.rotation, Quaternion.Euler(0, playerBody.transform.eulerAngles.y, 0), progress); 
            progress += Time.deltaTime * stepTime; 

            if (progress <= 1f) {
                yield return new WaitForFixedUpdate(); 
            }
        }
        playerBody.AddRelativeForce(Vector3.forward, ForceMode.VelocityChange); 
        playerInput.enabled = true; 
    }

    public void OnWaterEnter(float waterDragMult) {
        Debug.Log("Splash"); 
        playerInput.currentActionMap = playerInput.actions.FindActionMap("Water"); 
        playerCamera.eulerAngles = new Vector3(0, playerBody.transform.eulerAngles.y, 0); 
        // playerBody.useGravity = false; 
        playerBody.drag *= waterDragMult; 
        playerBody.angularDrag *= waterDragMult; 
        crouched = true; 
        isDiving = false; 
    }

    // Water always leads to normal gravity
    public void OnWaterExit(float waterDragMult) {
        Debug.Log("Splish"); 
        playerInput.currentActionMap = playerInput.actions.FindActionMap("Gravity"); 
        transform.eulerAngles = new Vector3(0, playerBody.transform.eulerAngles.y, 0); 
        playerBody.velocity.Normalize(); 
        // playerBody.useGravity = true; 
        playerBody.drag /= waterDragMult; 
        playerBody.angularDrag /= waterDragMult; 
    }
}
