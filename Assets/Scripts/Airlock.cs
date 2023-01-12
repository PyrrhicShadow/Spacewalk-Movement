using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airlock : MonoBehaviour {
    [SerializeField] GameManager gameManager; 
    private bool interactable; 
    private float timer = 0; 
    private float interactDelay = 2f; 

    private void Update() {
        if (!interactable) {
            if (timer < interactDelay) {
                timer += Time.deltaTime; 
            }
            else {
                interactable = true; 
                timer = 0; 
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (interactable && other.CompareTag("Player")) {
            if(gameManager.gravity) {
                gameManager.DisableGravity(); 
            }
            else {
                gameManager.EnableGravity(); 
            }
            interactable = false; 
        }
    }
}
