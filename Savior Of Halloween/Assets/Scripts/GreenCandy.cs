using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCandy : MonoBehaviour
{
    public ParticleSystem ps;
    public SpriteRenderer candyRenderer;
    public SpriteRenderer auraRenderer;
    private bool active = true;
    private bool refresh = false;
    private bool refreshOnLand = false;

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.name.Equals("Jeremy") && active) {
            ps.Play();
            candyRenderer.enabled = false;
            auraRenderer.enabled = false;
            active = false;
            refresh = false;
            collision.gameObject.GetComponent<Jeremy>().GCandy(this);
        }
    }

    public void RefreshAura() {
        refresh = true;
        auraRenderer.enabled = true;
        if (refreshOnLand) {
            Refresh();
            refreshOnLand = false;
        }
    }

    public void Refresh() {
        if (!active && refresh) {
            ps.Play();
            candyRenderer.enabled = true;
            active = true;
        } else if (!active) {
            refreshOnLand = true;
        }
    }
}   
