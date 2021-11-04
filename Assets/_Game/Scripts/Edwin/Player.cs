using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Obstacle")
        {
            Debug.Log("HIT");
            EventManager.instance.OnLostOneLife.Invoke();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))    // le levier et baissé
        {
            Debug.Log(KeyCode.E);
        }
    }
}
