using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShooter : MonoBehaviour
{
    public float TimerA;
    public float TimerB;
    private float _Timer;
    public float TimetoDodge;
    
    public bool Sheltered = false;

    public static BulletShooter _instance;
    void Awake()
    {
        if (_instance == null) _instance = this; 
        else Destroy(this.gameObject);
    }

    public void Start()
    {
        StartCoroutine(Shooting());
    }

    
    IEnumerator Shooting()
    {
        _Timer = Random.Range(TimerA, TimerB);
        yield return new WaitForSeconds(_Timer);
        
        StartCoroutine(Dodge());
    }

    IEnumerator Dodge()
    {
        //Le temps avant que le dodge time se finisse
        yield return new WaitForSeconds(TimetoDodge);
       
        Shooted();
        StartCoroutine(Shooting());
    }

   


    public void Sheltering()
    {
        StopCoroutine(Dodge());
        StartCoroutine(Shooting());
    }

    public void Shooted()
    {
        StartCoroutine(Shooting());
    }


}
