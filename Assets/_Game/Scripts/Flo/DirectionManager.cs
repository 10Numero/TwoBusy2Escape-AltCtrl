using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionManager : MonoBehaviour
{
    public static DirectionManager instance;

    private List<GameObject> _panneaux = new List<GameObject>();
    void Awake()
    {
        instance = this;

        EventManager.instance.OnSwitchLane.AddListener(_SwitchDirection);
    }

    void _SwitchDirection(Vector3 direction)
    {
        foreach(GameObject go in _panneaux)
        {
            go.transform.localEulerAngles = direction;
        }
    }

    public void RegisterPanneau(GameObject panneau) => _panneaux.Add(panneau);
    public void UnegisterPanneau(GameObject panneau) => _panneaux.Remove(panneau);
}
