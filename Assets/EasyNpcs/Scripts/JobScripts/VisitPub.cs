using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI_Package;

public class VisitPub : Work
{
    public Transform lookDirection;
    Rotate rotate;
    Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        rotate = gameObject.AddComponent<Rotate>();
        rotate.RotateTo(lookDirection);

        animator.SetBool("Sit", true);
    }

    void OnDisable()
    {
        Destroy(rotate);
        animator.SetBool("Sit", false);
    }
}
