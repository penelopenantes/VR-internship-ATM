using UnityEngine;

namespace AI_Package
{
    public static class CheckState
    {
        public static bool Check_CharacterManager(GameObject npc)
        {
            if (npc.GetComponentInParent<AI_Stats>() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool Check_State(GameObject npc)
        {
            if (npc.GetComponent<NpcAI>().enabled)
            {
                return State_NotScared(npc.GetComponent<NpcAI>());
            }
            else
            {
                return State_NotScared(npc.GetComponent<CombatAI>());
            }
        }

        static bool State_NotScared(NpcAI npcAI)
        {
            if (npcAI.currentState == NpcState.Scared)
            {
                Debug.Log("The npc's current state blocks interaction");
                return false;
            }
            else
            {
                npcAI.enabled = false;
                return true;
            }
        }

        static bool State_NotScared(CombatAI enemyAI)
        {
            if (enemyAI.currentState == NpcState.Chase || enemyAI.currentState == NpcState.Attack)
            {
                Debug.Log("The npc's current state blocks interaction");
                return false;
            }
            else
            {
                enemyAI.enabled = false;
                return true;
            }
        }
    }
}
