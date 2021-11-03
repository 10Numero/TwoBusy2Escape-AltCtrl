using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtManager : MonoBehaviour
{
    public static LookAtManager instance;
    public List<GameObject> decors;
    public GameObject wagon;
    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        foreach(GameObject go in decors)
        {
            go.transform.LookAt(wagon.transform);
        }
    }

    public void Register(GameObject decor) => decors.Add(decor);
    public void Unregister(GameObject decor) => decors.Remove(decor);
}
