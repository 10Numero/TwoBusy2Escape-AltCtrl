using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public AudioSource obstacleSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacle")
        {
            Destroy(other.transform.parent.gameObject);
            EventManager.instance.OnLostOneLife.Invoke();
            obstacleSound.Play();
        }
    }
}
