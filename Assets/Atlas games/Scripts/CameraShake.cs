using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public static CameraShake instance { get; private set; }

    private void Awake() {
        instance = this;
    }

    // Internal timer to keep track of shake duration
    private float shakeTimeRemaining;
    // Magnitude of the shake effect
    private float shakeMagnitude;

    // Update is called once per frame
    void Update() {
        if (shakeTimeRemaining > 0)
        {
            // Add random shake offset directly to the global position
            Vector3 shakeOffset = (Vector3)Random.insideUnitCircle * shakeMagnitude;
            transform.position += shakeOffset;
            shakeTimeRemaining -= Time.deltaTime;
        }

    }

    // Method to start the camera shake with specified duration and magnitude
    public void StartShake(float duration, float magnitude) {
        shakeTimeRemaining = duration;
        shakeMagnitude = magnitude;
    }

}
