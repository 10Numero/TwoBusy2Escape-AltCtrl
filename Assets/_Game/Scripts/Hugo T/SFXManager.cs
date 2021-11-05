using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioSource hitSound;
    public AudioSource leverSound;
    public AudioSource handcarSound;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.instance.DidPlayerDodged += LostLifeSound;
        EventManager.instance.OnSwitchLane += LeverSound;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LostLifeSound(bool dodged)
    {
        if(!dodged)
            hitSound.Play();
    }

    public void LeverSound(Vector3 dir)
    {
        leverSound.Play();
    }
}
