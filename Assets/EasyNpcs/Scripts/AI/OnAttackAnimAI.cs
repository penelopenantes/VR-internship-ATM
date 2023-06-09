using UnityEngine;
using UnityEngine.AI;
using AI_Package;

public class OnAttackAnimAI : StateMachineBehaviour
{
    AI_Stats stats;
    SenseTarget sense;
    Transform thisNpc;
    NavMeshAgent agent;

    public GameObject swordSound;
    public GameObject blockSound;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        stats = animator.GetComponent<AI_Stats>();
        thisNpc = animator.transform;
        sense = animator.GetComponent<SenseTarget>();

        agent = animator.GetComponent<NavMeshAgent>();
        agent.isStopped = true;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stats != null)
        {
            Attack(animator);
        }

        agent.isStopped = false;
    }

    void Attack(Animator animator)
    {
        if (stats.assignedWeapon == AI_Stats.Weapon.melee)
        {
            MeleeAttack(animator);
        }
        else
        {
            RangedAttack();
        }
    }

    void MeleeAttack(Animator animator)
    {
        if (Random.Range(0, 99) < 19)
        {
            animator.SetBool("Block", true);
        }
        else
        {
            ExecuteAttack();
        }
    }

    void ExecuteAttack()
    {
        Transform target = sense.currentTarget;
        if (AttackManager.AttackTarget(thisNpc, target))
        {
            Instantiate(swordSound);
        }
        else
        {
            if (target.GetComponent<Animator>() != null)
            {
                target.GetComponent<Animator>().SetTrigger("Impact");
                Instantiate(blockSound);
            }
        }
    }

    void RangedAttack()
    {
        Vector3 launchPos = thisNpc.transform.position + thisNpc.transform.forward * 1 + new Vector3(0, stats.launchHight, 0);
        Projectile projectile = Instantiate(stats.projectile, launchPos, thisNpc.transform.rotation);
        projectile.Fire(thisNpc, sense.currentTarget);
    }
}
