using UnityEngine;
using System.Collections;

public class Ice : MonoBehaviour
{
    private Vector3 originalPos;
    private bool isFalling = false;

    public float fallDistance = 10f;
    public float fallSpeed = 3f;
    public float respawnSpeed = 1f;
    public float minWait = 2f;          
    public float maxWait = 5f;          

    void Start()
    {
        originalPos = transform.localPosition;
        StartCoroutine(FallRoutine());
    }

    IEnumerator FallRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minWait, maxWait);
            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(FallDown());

            yield return StartCoroutine(MoveBack());
        }
    }

    IEnumerator FallDown()
    {
        isFalling = true;
        Vector3 targetPos = originalPos + Vector3.down * fallDistance;

        while (Vector3.Distance(transform.localPosition, targetPos) > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, Time.deltaTime * fallSpeed);
            yield return null;
        }
        isFalling = false;
    }

    IEnumerator MoveBack()
    {
        Vector3 startPos = transform.localPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * respawnSpeed;
            transform.localPosition = Vector3.Lerp(startPos, originalPos, t);
            yield return null;
        }
    }
}
