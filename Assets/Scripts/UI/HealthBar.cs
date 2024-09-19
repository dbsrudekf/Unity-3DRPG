using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Health healthComponent = null;
    [SerializeField]
    private RectTransform foreGround = null;
    [SerializeField]
    private Canvas rootCanvas = null;

    private void Update()
    {
        if (Mathf.Approximately(healthComponent.GetFraction(), 0) || Mathf.Approximately(healthComponent.GetFraction(), 1))
        {
            rootCanvas.enabled = false;
            return;
        }

        rootCanvas.enabled = true;

        foreGround.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);

    }
}
