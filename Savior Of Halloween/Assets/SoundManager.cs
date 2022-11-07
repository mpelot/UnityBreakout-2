using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance;
    [SerializeField] private AudioSource musicSource, effectsSource;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        musicSource.Play();
    }

    public void PlaySound(AudioClip clip) {
        effectsSource.PlayOneShot(clip);
    }
}