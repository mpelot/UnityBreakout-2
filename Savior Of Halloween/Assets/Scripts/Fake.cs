using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fake : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.name.Equals("Jeremy")) {
            Destroy(gameObject);
        }
    }
}
