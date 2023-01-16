using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSurface : MonoBehaviour {
    [SerializeField] float bobInterval; 
    private float timer = 0; 
    private bool bob; 

    private void Update() {
        if (!bob) {
            if (timer < bobInterval) {
                timer += Time.deltaTime; 
            }
            else {
                bob = true; 
                timer = 0; 
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (bob) {
            Rigidbody otherBody = other.attachedRigidbody; 
            otherBody.velocity = Vector3.up; 
            bob = false; 
        }
    }
}
