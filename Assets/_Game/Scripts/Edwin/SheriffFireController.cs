using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheriffFireController : MonoBehaviour
{
    private const int MIN_TIME = 1;
    private const int MAX_TIME = 20;

    [Header("Shoot Delay")]
    [SerializeField, Range(MIN_TIME, MAX_TIME)] private int minimumShootDelay = 4;
    [SerializeField, Range(MIN_TIME, MAX_TIME)] private int maximumShootDelay = 10;

    [Header("Next Shot Wait")]
    [SerializeField, Range(MIN_TIME, MAX_TIME)] private int initialWait = 10;
    [SerializeField, Range(MIN_TIME, MAX_TIME)] private int minimumNextWait = 2;
    [SerializeField, Range(MIN_TIME, MAX_TIME)] private int maximumNextWait = 6;

    private float shootDelay;
    private float nextShotWait;

    private bool hasShot = false;
    [HideInInspector] public bool hasDodged = false;

    public static SheriffFireController _instance;
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this.gameObject);
    }

    void Start()
    {
        #region Check Inputs
        if (minimumShootDelay > maximumShootDelay)   // pour eviter de faire le custom editor et de faire des slider dynamiques
        {
            minimumShootDelay = MIN_TIME;
            maximumShootDelay = MAX_TIME;
            Debug.LogWarning("Shoot Delay Bounds has been reset due to invalid inputs");
        }
        if(minimumNextWait > maximumNextWait)
        {
            minimumNextWait = MIN_TIME;
            maximumNextWait = MAX_TIME;
            Debug.LogWarning("Next Shoot Wait Bounds has been reset due to invalid inputs");
        }
        #endregion

        shootDelay = 0;
        nextShotWait = initialWait;    // pour que les joueures s'installe et sont pret
        nextShotWait = 1;    // pour que les joueures s'installe et sont pret
    }

    void Update()
    {
        if(hasShot)
        {
            #region Shooting
            shootDelay -= Time.deltaTime;
            if(shootDelay < 0)  // go wait
            {
                hasShot = false;
                nextShotWait = Random.Range(minimumNextWait, maximumNextWait + 1);
                if (!hasDodged)
                    EventManager.instance.OnLostOneLife.Invoke();
                Debug.Log("Dodged=" + hasDodged + " Wait " + nextShotWait);
            }
            #endregion
        }
        else
        {
            #region Waiting
            nextShotWait -= Time.deltaTime;
            if(nextShotWait < 0)    // go shoot
            {
                hasShot = true;
                shootDelay = Random.Range(minimumShootDelay, maximumShootDelay + 1);
                EventManager.instance.OnSheriffShoot.Invoke();
                Debug.Log("Shoot " + shootDelay);
            }
            #endregion
        }
    }
}
