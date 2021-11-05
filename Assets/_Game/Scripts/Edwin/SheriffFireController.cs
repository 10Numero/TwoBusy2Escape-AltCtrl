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

    private float shootDelay = 0f;
    private float nextShotWait = 0f;

    private float stageTime = 0f; // pour ne pas calculer a chaque frame
    private bool stage1 = false, stage2 = false, stage3 = false;

    private bool hasShot = false;
    [HideInInspector] public bool hasDodged = false;

    public static SheriffFireController _instance;

    public AudioSource hitSound;

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

        nextShotWait = initialWait;    // pour que les joueures s'installe et sont pret
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
                {
                    EventManager.instance.OnLostOneLife.Invoke();
                    hitSound.Play();
                }
            }
            #endregion

            #region Shooting Stages
            if(shootDelay > 0f && shootDelay < stageTime)   // Stage 3 : proche
            {
                if(!stage3)
                {
                    stage3 = true;
                }
            }
            else if(shootDelay >= stageTime && shootDelay < shootDelay - stageTime)     // Stage 2 : milieu
            {
                if (!stage2)
                {


                    stage2 = true;
                }
            }
            else    // Stage 1 : loin
            {
                if (!stage1)
                {


                    stage1 = true;
                }
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
                stageTime = shootDelay / 3.0f;
                stage1 = stage2 = stage3 = false;
                EventManager.instance.OnSheriffShoot.Invoke();
            }
            #endregion
        }
    }
}
