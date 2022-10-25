using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCandy : MonoBehaviour
{
    public ParticleSystem ps;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.name.Equals("Jeremy")) {
            collision.gameObject.GetComponent<Jeremy>().GreenCandy();
            ps = Instantiate(ps, transform.position, Quaternion.identity);
            ps.GetComponent<Burst>().Play();
            Destroy(gameObject);
        }
    }
}   
