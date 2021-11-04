using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SimpleFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 _offset;
    private void Update()
    {
        Vector3 pos = target.position;
        pos.x = 0;
        pos.y = 0;
        pos += _offset;
        transform.position = pos;
    }
}
