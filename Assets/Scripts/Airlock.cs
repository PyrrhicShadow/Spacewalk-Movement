using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airlock : MonoBehaviour {
    [SerializeField] GameManager gameManager; 
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if(gameManager.gravity) {
                gameManager.DisableGravity(); 
            }
            else {
                gameManager.EnableGravity(); 
            }
        }
    }
}
