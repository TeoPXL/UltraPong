using UnityEngine;
using System.Collections;

public class SimpleCameraShake : MonoBehaviour
{
    [Range(0f, 10f)]
    public float strength = 1f; // New public property to scale shake

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude * strength;
            float offsetY = Random.Range(-1f, 1f) * magnitude * strength;

            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}