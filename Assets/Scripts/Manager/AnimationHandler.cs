using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void ResetAnimation()
    {
        animator.ResetTrigger("Pop");
        //transform.localScale = Vector3.one;
        //gameObject.SetActive(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
