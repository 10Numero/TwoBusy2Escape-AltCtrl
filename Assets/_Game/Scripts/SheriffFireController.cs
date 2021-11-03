using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheriffFireController : MonoBehaviour
{
    private void Start()
    {
        
    }

    //Appelle cette méthode comme tu veux, mais lorsque tu shoot la balle je veux que tu invoke cette event
    void _Shoot()
    {
        EventManager.instance.OnSheriffShoot.Invoke();
    }

    //Appelle cette méthode comme tu veux, mais si le joueur n'a pas dodge à temps et donc que la balle du sheriff l'a atteint je veux que tu invoke cette event
    void _Hit()
    {
        EventManager.instance.OnLostOneLife.Invoke();
    }
}
