using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrogravityZone : MonoBehaviour {
    private void OnTriggerStay(Collider other) {
        if (!other.CompareTag("Player")) {
            Rigidbody otherBody = other.gameObject.GetComponent<Rigidbody>(); 
            if (otherBody != null) {
                otherBody.useGravity = false; 
            }
            otherBody = null; 
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            other.gameObject.GetComponent<RigidbodyMovement>().OnGravityEnable(); 
        }
        else {
            Rigidbody otherBody = other.gameObject.GetComponent<Rigidbody>(); 
            if (otherBody != null) {
                otherBody.useGravity = true; 
            }
            otherBody = null; 
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            other.gameObject.GetComponent<RigidbodyMovement>().OnGravityDisable(); 
        }
        else {
            Rigidbody otherBody = other.gameObject.GetComponent<Rigidbody>(); 
            if (otherBody != null) {
                otherBody.useGravity = false; 
            }
            otherBody = null; 
        }
    }
}
