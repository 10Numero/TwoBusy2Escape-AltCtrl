using BansheeGz.BGSpline.Curve;
using BansheeGz.BGSpline.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Main Lever and Speed Settings")]
    [SerializeField] private float pushTimeout = 2f;
    [SerializeField] private float averageSpeed = 0.5f;
    [SerializeField] private float maxSpeed = 2.5f;
    [SerializeField] private float friction = 0.625f;
    private float speed = 0f;
    private bool isPushingLever = false, isPushedA = false;
    private float timer = 0f, prevTimer = 0f, delta = 0f, period = 0f;

    [Header("Joystick")]
    public KeyCode LeverVoieLeft = KeyCode.Z;

    [Header("Joystick Buttons")]
    public KeyCode LeverVoieRight = KeyCode.A;

    [Header("Button 1")]
    public KeyCode LeverLeftPlayerA = KeyCode.R;
    [Header("Button 2")]
    public KeyCode LeverRightPlayerA = KeyCode.F;
    [Header("Button 3")]
    public KeyCode LeverDownPlayerA = KeyCode.E;

    [Header("Button 4")]
    public KeyCode LeverLeftPlayerB = KeyCode.T;
    [Header("Button 5")]
    public KeyCode LeverRightPlayerB = KeyCode.G;
    [Header("Button 6")]
    public KeyCode LeverDownPlayerB = KeyCode.Y;

    [Header("Button 7")]
    public KeyCode HandDodgeLeft = KeyCode.U;
    [Header("Button 8")]
    public KeyCode HandDodgeRight = KeyCode.I;

    [Header("Button 9")]
    public KeyCode KneeDodge = KeyCode.O;

    [Header("Button 10")]
    public KeyCode HeadDodge = KeyCode.P;


    void Update()
    {
        LeverMain();

        LeverLaneLeft();
        LeverLaneRight();

        if (DodgeHand() && DodgeKnee() && DodgeHead())
        {
            if(!BulletShooter._instance.Sheltered)
                BulletShooter._instance.Sheltered = true;
        }
        else if (BulletShooter._instance.Sheltered)
            BulletShooter._instance.Sheltered = false;
    }

    #region ChangeLane
    private void LeverLaneLeft()
    {
        if (Input.GetKeyDown(LeverVoieLeft))
            LevelGenerator._instance.SwitchLane(true);
    }

    private void LeverLaneRight()
    {
        if (Input.GetKeyDown(LeverVoieRight))
            LevelGenerator._instance.SwitchLane(false);
    }
    #endregion

    #region LeverMain
    private void LeverMain()
    {
        if (Input.GetKey(LeverLeftPlayerA) && Input.GetKey(LeverRightPlayerA) && Input.GetKey(LeverLeftPlayerB) && Input.GetKey(LeverRightPlayerB)) // les mains des 2 joueurs sont sur le levier
        {
            if(Input.GetKeyDown(LeverDownPlayerA))    // le levier et baissé
            {
                if(timer != 0 && isPushedA)
                    resetSpeed();

                setSpeed();
                isPushedA = true;
            }
            else if(Input.GetKeyDown(LeverDownPlayerB))
            {
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
        else if(isPushingLever)
            resetSpeed();

        if (speed > 0f)
            speed -= friction * Time.deltaTime;
        else if (speed < 0f)
            speed = 0f;
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
