using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Test code for Edge collisions
public class TestTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col) {
        if(col.CompareTag("Border")) {
            Debug.Log("border");
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if(col.CompareTag("Border")) {
            Debug.Log("BORDER");
        }
    }
}
