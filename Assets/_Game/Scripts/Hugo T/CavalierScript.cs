﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CavalierScript : MonoBehaviour
{
    public float rotationRange = -90f;

    public Transform[] cavaliers;
    //public SpriteRenderer[] viewfinders;
    public SpriteRenderer viewfinder;

    public Color[] viewfinderColors;

    public float animationSpeed;
    public float timeShoot;

    private int actualCavalier;

    public float timeColorChange1 = 2f;
    public float timeColorChange2 = 3.1f;

    public Animator animator;


    void Start()
    {
        rotationRange = -90f;
        EventManager.instance.OnSheriffShoot.AddListener(CavalierShooting);

        viewfinder.gameObject.SetActive(false);
        viewfinder.color = viewfinderColors[0];
        
    }

    void Update()
    {
        // Temporary
        if (Input.GetKeyDown(KeyCode.N))
        {
            CavalierShooting();
        }
    }

    public void CavalierShooting()
    {
        actualCavalier = Random.Range(0, cavaliers.Length);
        cavaliers[actualCavalier].DOLocalRotate(Vector3.zero, animationSpeed); 
        
        StartCoroutine(StartViewfinderAnim(actualCavalier));

        StartCoroutine(ReturnCavalier());
    }

    IEnumerator ReturnCavalier()
    {
        yield return new WaitForSeconds(timeShoot + animationSpeed);
        
        cavaliers[actualCavalier].DOLocalRotate(new Vector3(rotationRange, 0, 0), animationSpeed);

        viewfinder.gameObject.SetActive(false);
        animator.SetInteger("animationIterator", -1);

    }

    IEnumerator StartViewfinderAnim(int animationIt)
    {
        yield return new WaitForSeconds(animationSpeed);

        viewfinder.gameObject.SetActive(true);
        animator.SetInteger("animationIterator", animationIt);

        StartCoroutine(ColorChange(timeColorChange1, 1));
        StartCoroutine(ColorChange(timeColorChange2, 2));

    }

    IEnumerator ColorChange(float timeColorChange, int color)
    {
        yield return new WaitForSeconds(timeColorChange);
        viewfinder.color = viewfinderColors[color];

    }
}