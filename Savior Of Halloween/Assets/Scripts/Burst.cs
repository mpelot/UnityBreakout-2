using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burst : MonoBehaviour
{

    public void Play() {
        StartCoroutine(StartPlay());
    }
    IEnumerator StartPlay() {
        var ps = GetComponent<ParticleSystem>();
        ps.Play();
        yield return new WaitForSeconds(.6f);
        Destroy(gameObject);
    }
}
