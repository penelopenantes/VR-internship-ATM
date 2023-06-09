using System.Collections;
using UnityEngine;
using AI_Package;

public class LumberJack : Work
{
    public Transform lookPosition;

    Animator anim;
    Rotate rotate;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        rotate = gameObject.AddComponent<Rotate>();
        rotate.RotateTo(lookPosition);
        StartCoroutine(CutWood());
    }

    IEnumerator CutWood()
    {
        anim.SetTrigger("Chop");
        yield return new WaitForSeconds(1);

        StartCoroutine(CutWood());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        Destroy(rotate);
    }
}
