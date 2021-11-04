using BansheeGz.BGSpline.Curve;
using BansheeGz.BGSpline.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Main Lever and Speed Settings")]
    [SerializeField] private bool constantSpeed = false;
    [SerializeField] private float pushTimeout = 2f;
    [SerializeField] private float averageSpeed = 0.5f;
    [SerializeField] private float maxSpeed = 2.5f;
    [SerializeField] private float friction = 0.625f;
    private float speed = 0f;
    private bool isPushingLever = false, isPushedA = false;
    private float timer = 0f, prevTimer = 0f, delta = 0f, period = 0f;

    [Header("Joystick")]
    [SerializeField] private KeyCode LeverLaneLeft = KeyCode.Z;
    [Header("Joystick Buttons")]
    [SerializeField] private KeyCode LeverLaneRight = KeyCode.A;

    [Header("Button 1")]
    [SerializeField] private KeyCode LeverLeftPlayerA = KeyCode.R;
    [Header("Button 2")]
    [SerializeField] private KeyCode LeverRightPlayerA = KeyCode.F;
    [Header("Button 3")]
    [SerializeField] private KeyCode LeverDownPlayerA = KeyCode.E;

    [Header("Button 4")]
    [SerializeField] private KeyCode LeverLeftPlayerB = KeyCode.T;
    [Header("Button 5")]
    [SerializeField] private KeyCode LeverRightPlayerB = KeyCode.G;
    [Header("Button 6")]
    [SerializeField] private KeyCode LeverDownPlayerB = KeyCode.Y;

    [Header("Button 7")]
    [SerializeField] private KeyCode HandDodgeLeft = KeyCode.U;
    [Header("Button 8")]
    [SerializeField] private KeyCode HandDodgeRight = KeyCode.I;

    [Header("Button 9")]
    [SerializeField] private KeyCode KneeDodge = KeyCode.O;

    [Header("Button 10")]
    [SerializeField] private KeyCode HeadDodge = KeyCode.P;

    public static PlayerInput _instance;
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this.gameObject);
    }

    void Update()
    {
        LeverMain();

        LeverLeftLane();
        LeverRightLane();

        Dodge();
    }

    #region ChangeLane
    private void LeverLeftLane()
    {
        if (Input.GetKeyDown(LeverLaneLeft))
            LevelGenerator._instance.SwitchLane(true);
    }

    private void LeverRightLane()
    {
        if (Input.GetKeyDown(LeverLaneRight))
            LevelGenerator._instance.SwitchLane(false);
    }
    #endregion

    #region LeverMain
    private void LeverMain()
    {
        if (!constantSpeed)
        {
            if (Input.GetKey(LeverLeftPlayerA) && Input.GetKey(LeverRightPlayerA) && Input.GetKey(LeverLeftPlayerB) && Input.GetKey(LeverRightPlayerB)) // les mains des 2 joueurs sont sur le levier
            {
                if (Input.GetKeyDown(LeverDownPlayerA))    // le levier et baissé
                {
                    Debug.Log(LeverDownPlayerA);
                    if (timer != 0 && isPushedA)
                        resetSpeed();

                    setSpeed();
                    isPushedA = true;
                }
                else if (Input.GetKeyDown(LeverDownPlayerB))
                {
                    Debug.Log(LeverDownPlayerB);
                    if (timer != 0 && !isPushedA)
                        resetSpeed();

                    setSpeed();
                    isPushedA = false;
                }

                if (isPushingLever)
                {
                    timer += Time.deltaTime;
                    if (timer > pushTimeout)
                        resetSpeed();
                }
            }
            else if (isPushingLever)
                resetSpeed();

            if (speed > 0f)
                speed -= friction * Time.deltaTime;
            else if (speed < 0f)
                speed = 0f;
        }
        else
            speed = averageSpeed;
        LevelGenerator._instance.path.GetComponent<BGCcTrs>().Speed = speed;

    }

    private void setSpeed()
    {
        if(prevTimer != 0f)
        {
            period = timer + prevTimer;
            delta = Mathf.Abs(timer - prevTimer);

            speed += averageSpeed * (1f + period / pushTimeout) * (1f - delta / period);

            if(speed > maxSpeed)
                speed = maxSpeed;

            prevTimer = 0f;
        }
        else
            prevTimer = timer;

        isPushingLever = true;
        timer = 0f;
    }

    private void resetSpeed()
    {
        delta = 0f;
        period = 0f;
        timer = 0f;
        prevTimer = 0f;
        isPushingLever = false;
    }
    #endregion

    #region Dodge
    private void Dodge()
    {
        if (DodgeHand() && DodgeKnee() && DodgeHead())
        {
            if (!SheriffFireController._instance.hasDodged)
                SheriffFireController._instance.hasDodged = true;
        }
        else if (SheriffFireController._instance.hasDodged)
            SheriffFireController._instance.hasDodged = false;
    }

    private bool DodgeHand()
    {
        return Input.GetKey(HandDodgeLeft) && Input.GetKey(HandDodgeRight);
    }

    private bool DodgeKnee()
    {
        return Input.GetKey(KneeDodge);
    }

    private bool DodgeHead()
    {
        return Input.GetKey(HeadDodge);
    }
    #endregion
}
