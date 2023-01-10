using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {
    [SerializeField] RigidbodyMovement player; 
    [SerializeField] Collider col; 

    private void Awake() {
        if (col == null) {
            col = this.gameObject.GetComponent<Collider>(); 
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag(this.gameObject.tag)) {
            // Debug.Log("Grounded."); 
            player.SetGrounded(true); 
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!other.gameObject.CompareTag(this.gameObject.tag)) {
            // Debug.Log("Not grounded."); 
            player.SetGrounded(false); 
        }
    }

    private void OnTriggerStay(Collider other) {
        if (!other.gameObject.CompareTag(this.gameObject.tag)) {
            player.SetGrounded(true); 
        }
    }
}
