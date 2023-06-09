using UnityEngine;

namespace AI_Package
{
    public static class AttackManager
    {
        public static bool AttackTarget(Transform attacker, Transform target)
        {
            if (NotBlocked(target.GetComponentInChildren<Animator>()) &&
                Vector3.Distance(attacker.transform.position, target.transform.position) <= attacker.GetComponent<Stats>().attackDistance)
            {
                SendAttack(attacker, target);
                return true;
            }

            return false;
        }

        static void SendAttack(Transform attacker, Transform target)
        {
            NpcBase targetNpc = EnabledComponent.NpcBase(target.GetComponents<NpcBase>());
            if (targetNpc != null)
                targetNpc.Attacked(attacker);

            DealDamage(attacker.GetComponent<Stats>(), target.GetComponent<Stats>());
        }

        public static void DealDamage(Stats attacker, Stats defender)
        {
            float baseDamage = attacker.damage;

            if (defender != null)
                baseDamage -= defender.armour;

            if (baseDamage < 0)
                baseDamage = 0;

            defender.currentHealth = defender.currentHealth - attacker.damage;
        }

        static bool NotBlocked(Animator anim)
        {
            if (anim.GetBool("Block"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}