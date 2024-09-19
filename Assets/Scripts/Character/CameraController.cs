using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private GameObject target;
    private float mouseSensitivity = 500f;

    private void Awake()
    {
        
    }
    private void Update()
    {
        Rotate();
        MoveTo();
    }

    private void Rotate()
    {
        if (Input.GetMouseButton(1))
        {            
            float xRotateMove = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
            transform.RotateAround(target.transform.position, Vector3.up, xRotateMove);
            offset = transform.position - target.transform.position;
        }    
    }

    private void MoveTo()
    {
        transform.position = target.transform.position + offset;
    }
}
