using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    public Transform spawnPoint;
    public Animator animator;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.name.Equals("Jeremy")) {
            collision.gameObject.GetComponent<Jeremy>().Loot(spawnPoint);
            animator.Play("Looted");
        }
    }
}
