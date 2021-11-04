using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public UnityEvent OnWarningStart;
    public UnityEvent OnWarningStop;
    public UnityEvent OnSheriffShoot;
    public UnityEvent OnGameWin;
    public UnityEvent OnGameLoose;
    public UnityEvent OnLostOneLife;
    public UnityEvent<Vector3> OnSwitchLane;

    public UnityAction<bool> DidPlayerDodged;
    public UnityAction<int> OnDisplaysLengthChanged;

    public static EventManager instance;
    void Awake()
    {
        instance = this;

        OnSheriffShoot.AddListener(_SheriffShoot);
        OnLostOneLife.AddListener(_LostLife);
        OnGameLoose.AddListener(_Loose);
        OnGameWin.AddListener(_Win);
        OnWarningStart.AddListener(_WarningStart);
        OnWarningStop.AddListener(_WarningStop);
        OnDisplaysLengthChanged += _DisplaysUpdate;
        DidPlayerDodged += _DidPlayerDodged;
        OnSwitchLane.AddListener(_SwitchLane);
    }

    private void Start()
    {
        CanvasManager.instance.UpdateActualUiComponents(Display.displays.Length);
    }

    void _DisplaysUpdate(int monitorsCount)
    {
        Debug.Log("Monitor count changed : " + monitorsCount);
    }

    void _LostLife()
    {
        //Debug.Log("Life One life");
    }

    void _WarningStart()
    {
        //Debug.Log("Warning start");
    }


    void _WarningStop()
    {
        //Debug.Log("Warning Stop");
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

    void _DidPlayerDodged(bool dodged)
    {

    }

    void _SwitchLane(Vector3 dir)
    {
        OnWarningStop.Invoke();
    }
}
