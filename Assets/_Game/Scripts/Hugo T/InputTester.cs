using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InputUSB();
    }

    public void InputUSB()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Joystick 1");
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Joystick 2");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Left Lever Button P1");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Right Lever Button P1");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Left Lever Button P2");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Right Lever Button P2");
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Dodge Left Hand P2");
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Dodge Right Hand P2");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Dodge Knee P2");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Dodge Head P2");
        }
    }
}
