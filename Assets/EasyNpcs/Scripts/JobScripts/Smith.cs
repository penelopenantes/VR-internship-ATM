using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using AI_Package;

public class Smith : Work
{
    public Transform workShop;
    public Transform workShop1;
    public Transform anvil;
    public Transform smelter;

    Transform currentWorkShop;
    NavMeshAgent agent;

    float time = 0;

    Rotate rotate;

    private void Start()
    {
        time = 0;
        currentWorkShop = workShop;
        agent = GetComponent<NavMeshAgent>();
        rotate = gameObject.AddComponent<Rotate>();

        StartCoroutine(Hammer());
        rotate.RotateTo(anvil);
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time >= 30)
        {
            if (currentWorkShop != workShop)
            {
                StopAllCoroutines();

                Set_CurrentWorkShop(workShop);
                NewRotate();
                StartCoroutine(Hammer());
                rotate.RotateTo(anvil);
            }
            else
            {
                StopAllCoroutines();

                Set_CurrentWorkShop(workShop1);
                NewRotate();
                StartCoroutine(Crouch());
                rotate.RotateTo(smelter);
            }

            time = 0;
        }
    }

    void Set_CurrentWorkShop(Transform _transform)
    {
        currentWorkShop = _transform;
        agent.SetDestination(_transform.position);
    }

    void NewRotate()
    {
        Destroy(rotate);
        rotate = gameObject.AddComponent<Rotate>();
    }

    IEnumerator Hammer()
    {
        Animator anim = GetComponentInChildren<Animator>();
        yield return new WaitForSeconds(2);

        anim.SetTrigger("Smith");
        StartCoroutine(Hammer());
    }

    IEnumerator Crouch()
    {
        Animator anim = GetComponentInChildren<Animator>();
        yield return new WaitForSeconds(2);

        anim.SetTrigger("Pull");
        StartCoroutine(Crouch());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        Destroy(rotate);
    }
}