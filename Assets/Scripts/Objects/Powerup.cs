using System;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public Action OnUpgrade;

    void OnTriggerEnter2D(Collider2D collision)
    {
        OnUpgrade?.Invoke();
        Destroy(gameObject);
    }
}
