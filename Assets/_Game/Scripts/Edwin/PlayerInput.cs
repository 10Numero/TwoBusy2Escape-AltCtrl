using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Joystick")]
    public KeyCode LevierVoieLeft = KeyCode.Z;
    [Header("Joystick Buttons")]
    public KeyCode LevierVoieRight = KeyCode.A;
    [Header("Button 1")]
    public KeyCode LevierLeftPlayerA = KeyCode.R;
    [Header("Button 2")]
    public KeyCode LevierRightPlayerA = KeyCode.F;
    [Header("Button 3")]
    public KeyCode LevierLeftPlayerB = KeyCode.T;
    [Header("Button 4")]
    public KeyCode LevierRightPlayerB = KeyCode.G;
    [Header("Button 5")]
    public KeyCode HandDodgeLeft = KeyCode.U;
    [Header("Button 7")]
    public KeyCode HandDodgeRight = KeyCode.I;
    [Header("Button 9")]
    public KeyCode KneeDodge = KeyCode.O;
    [Header("Button 11")]
    public KeyCode HeadDodge = KeyCode.P;



    void Start()
    {
        
    }

    void Update()
    {
        LevierLeft();
        LevierRight();

        LevierMain();
        
        DodgeHand();
        DodgeKnee();
        DodgeHead();
    }

    private void LevierLeft()
    {
        if (Input.GetKeyDown(LevierVoieLeft))
        {

        }
        else if (Input.GetKey(LevierVoieLeft))
        {

        }
        else if (Input.GetKeyUp(LevierVoieLeft))
        {

        }
    }

    private void LevierRight()
    {
        if (Input.GetKeyDown(LevierVoieRight))
        {

        }
        else if (Input.GetKey(LevierVoieRight))
        {

        }
        else if (Input.GetKeyUp(LevierVoieRight))
        {

        }
    }

    private void LevierMain()
    {
        if (Input.GetKeyDown(LevierLeftPlayerA))
        {

        }
        else if (Input.GetKey(LevierLeftPlayerA))
        {

        }
        else if (Input.GetKeyUp(LevierLeftPlayerA))
        {

        }

        if (Input.GetKeyDown(LevierRightPlayerA))
        {

        }
        else if (Input.GetKey(LevierRightPlayerA))
        {

        }
        else if (Input.GetKeyUp(LevierRightPlayerA))
        {

        }

        if (Input.GetKeyDown(LevierLeftPlayerB))
        {

        }
        else if (Input.GetKey(LevierLeftPlayerB))
        {

        }
        else if (Input.GetKeyUp(LevierLeftPlayerB))
        {

        }

        if (Input.GetKeyDown(LevierRightPlayerB))
        {

        }
        else if (Input.GetKey(LevierRightPlayerB))
        {

        }
        else if (Input.GetKeyUp(LevierRightPlayerB))
        {

        }
    }

    private void DodgeHand()
    {
        if (Input.GetKeyDown(HandDodgeLeft))
        {

        }
        else if (Input.GetKey(HandDodgeLeft))
        {

        }
        else if (Input.GetKeyUp(HandDodgeLeft))
        {

        }

        if (Input.GetKeyDown(HandDodgeRight))
        {

        }
        else if (Input.GetKey(HandDodgeRight))
        {

        }
        else if (Input.GetKeyUp(HandDodgeRight))
        {

        }
    }

    private void DodgeKnee()
    {
        if (Input.GetKeyDown(KneeDodge))
        {

        }
        else if (Input.GetKey(KneeDodge))
        {

        }
        else if (Input.GetKeyUp(KneeDodge))
        {

        }
    }

    private void DodgeHead()
    {
        if (Input.GetKeyDown(HeadDodge))
        {

        }
        else if (Input.GetKey(HeadDodge))
        {

        }
        else if (Input.GetKeyUp(HeadDodge))
        {

        }
    }
}
