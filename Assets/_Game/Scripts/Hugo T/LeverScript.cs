using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{

    [SerializeField] private float timer;
    [SerializeField] private float firstTime;
    public float delta;
    public float cycleTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PushLeverCycle();
    }

    public void PushLeverCycle()
    {
        
        if (Input.GetKey(KeyCode.A))
        {
            timer += Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            firstTime = timer;
        }

        if (Input.GetKey(KeyCode.E))
        {
            timer += Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            delta = timer - firstTime;
            cycleTime = timer;
            timer = 0;
        }
    }
}
