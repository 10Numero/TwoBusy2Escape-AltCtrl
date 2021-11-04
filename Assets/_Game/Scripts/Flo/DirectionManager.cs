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
    }

    void Start()
    {
        EventManager.instance.OnSwitchLane += _SwitchDirection;
    }

    void _SwitchDirection(Vector3 direction)
    {
        foreach(GameObject go in _panneaux)
        {
            go.transform.localEulerAngles = direction;
        }
    }

    public void SwitchLeft()
    {
        foreach (GameObject go in _panneaux)
        {
            go.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
    }

    public void SwitchRight()
    {
        foreach (GameObject go in _panneaux)
        {
            go.transform.localEulerAngles = Vector3.zero;
        }
    }

    public void RegisterPanneau(GameObject panneau) => _panneaux.Add(panneau);
    public void UnegisterPanneau(GameObject panneau) => _panneaux.Remove(panneau);
}
