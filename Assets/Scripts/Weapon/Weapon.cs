using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onHit;
    public void OnHit()
    {
        onHit.Invoke();
    }
}
