using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private EventManager _instance;
    public static EventManager instance { get { return instance; } }

    public UnityEvent OnWarningStart;
    public UnityEvent OnWarningStop;
    public UnityEvent OnSheriffShoot;
    public UnityEvent OnGameWin;
    public UnityEvent OnGameLoose;
    public UnityEvent OnLostOneLife;
    public UnityEvent OnDisplaysLengthChanged;

    void Awake()
    {
        _instance = this;

        OnSheriffShoot.AddListener(_SheriffShoot);
        OnLostOneLife.AddListener(_LostLife);
        OnGameLoose.AddListener(_Loose);
        OnGameWin.AddListener(_Win);
        OnWarningStart.AddListener(_WarningStart);
        OnWarningStop.AddListener(_WarningStop);
        OnDisplaysLengthChanged.AddListener(_DisplaysUpdate);
    }

    void _DisplaysUpdate()
    {

    }

    void _LostLife()
    {

    }

    void _WarningStart()
    {

    }

    void _WarningStop()
    {

    }

    void _SheriffShoot()
    {

    }

    void _Win()
    {

    }

    void _Loose()
    {

    }

}
