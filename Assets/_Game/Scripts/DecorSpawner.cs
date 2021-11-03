using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] decors;
    public float spawnProb;

    private void Awake()
    {
        foreach(Transform t in spawnPoints)
        {
            
        }
    }
}
