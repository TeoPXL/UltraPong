using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 5;
    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
