using System.Collections;
using UnityEngine;
using AI_Package;

public class WaitTillReach_WorkHome : MonoBehaviour
{
    NpcAI npcAI;

    public void Set(NpcAI npc)
    {
        npcAI = npc;
    }

    public void Start_GOTOWork()
    {
        StartCoroutine(GoToWorkCoroutine());
    }
 
    IEnumerator GoToWorkCoroutine()
    {
        Debug.Log(npcAI.name + npcAI.agent.name + npcAI.pos.position);
        npcAI.agent.SetDestination(npcAI.pos.position);
        Debug.Log("lsgo");
        yield return new WaitUntil(() => Vector3.Distance(transform.position, npcAI.pos.position) <= 0.5f);

        npcAI.ChangeState(NpcState.Working);
    }

    public void Start_GOTOHome()
    {
        StartCoroutine(GoHomeCoroutine());
    }

    IEnumerator GoHomeCoroutine()
    {
        npcAI.agent.SetDestination(npcAI.home.position);
        yield return new WaitUntil(() => npcAI.agent.remainingDistance <= 0.1f && !npcAI.agent.pathPending);

        npcAI.ChangeState(NpcState.AtHome);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
