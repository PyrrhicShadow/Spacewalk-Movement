using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterZone : MonoBehaviour {
    [SerializeField] float waterDragMult; 
    private void OnTriggerStay(Collider other) {
        Rigidbody otherBody = other.attachedRigidbody; 
        if (otherBody != null) {
            otherBody.WakeUp(); 
            if (otherBody.velocity.y < 0) {
                otherBody.AddForce(-Physics.gravity, ForceMode.Acceleration); 
            }
            otherBody = null; 
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            other.gameObject.GetComponent<RigidbodyMovement>().OnWaterExit(waterDragMult); 
        }
        else {
            Rigidbody otherBody = other.attachedRigidbody; 
            if (otherBody != null) {
                // otherBody.useGravity = true; 
                otherBody.drag /= waterDragMult; 
                otherBody.angularDrag /= waterDragMult; 
            }
            otherBody = null; 
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            other.gameObject.GetComponent<RigidbodyMovement>().OnWaterEnter(waterDragMult); 
        }
        else {
            Rigidbody otherBody = other.attachedRigidbody; 
            if (otherBody != null) {
                // otherBody.useGravity = false; 
                otherBody.velocity = new Vector3(otherBody.velocity.x, 0, otherBody.velocity.z); 
                otherBody.drag *= waterDragMult; 
                otherBody.angularDrag *= waterDragMult; 
            }
            otherBody = null; 
        }
    }
}
