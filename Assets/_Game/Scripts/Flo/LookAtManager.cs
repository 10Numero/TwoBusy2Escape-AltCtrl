using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtManager : MonoBehaviour
{
    public static LookAtManager instance;
    public List<GameObject> _decors;
    public GameObject wagon;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (_decors != null)
        {
            foreach (GameObject go in _decors)
                go.transform.LookAt(wagon.transform);
        }
    }

    public void Register(GameObject decor) => _decors.Add(decor);
    public void Unregister(GameObject decor) => _decors.Remove(decor);
}
