using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtManager : MonoBehaviour
{
    public static LookAtManager instance;
    public List<GameObject> decors;
    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        foreach(GameObject go in decors)
        {
            //Appliquer lookat
        }
    }

    public void Register(GameObject decor) => decors.Add(decor);
    public void Unregister(GameObject decor) => decors.Remove(decor);
}
