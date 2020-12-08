using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera mainCamera;

    public float rotateAngle;

    private void Start()
    {
        rotateAngle = 45;

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            RotateRight();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateLeft();
        }
    }


    public void RotateLeft()
    {
        transform.Rotate(Vector3.up, rotateAngle, Space.Self);
    }


    public void RotateRight()
    {
        transform.Rotate(Vector3.up, -rotateAngle, Space.Self);
    }
}
