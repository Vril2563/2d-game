using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace VFX
{
    public class CameraShake : MonoBehaviour
    {
        [FormerlySerializedAs("ShakeDuration")]
        [Header("Camera Shake")]
        [SerializeField] private float shakeDuration = 0.5f;
        [SerializeField] private float shakeMagnitude = 0.1f;

        public static CameraShake instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private void ShakeCamera()
        {
            StartCoroutine(Shake(shakeDuration, shakeMagnitude));
        }
        
        public void ShakeCamera(float duration, float magnitude)
        {
            StartCoroutine(Shake(duration, magnitude));
        }
        
        IEnumerator Shake(float duration, float magnitude)
        {
            Vector3 originalPos = transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                // Generate Perlin noise values for x and y
                float x = Mathf.PerlinNoise(Time.time * 10f, 0f) * 2f - 1f;
                float y = Mathf.PerlinNoise(0f, Time.time * 10f) * 2f - 1f;

                // Scale the noise values by the magnitude
                x *= magnitude;
                y *= magnitude;

                // Apply the noise to the camera's position
                transform.localPosition = new Vector3(x, y, originalPos.z);

                // Update elapsed time
                elapsed += Time.deltaTime;

                // Wait for the next frame
                yield return null;
            }

            // Reset the camera's position to its original position
            transform.localPosition = originalPos;
        }
    }
}
