using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace AI_Package
{
    [RequireComponent(typeof(SenseTarget))]
    public class CombatAI : NpcBase
    {
        public Transform guardPost;
        public List<Transform> patrolSpots;
        int spotNum = 0;

        protected override void Awake()
        {
            base.Awake();
            sense = GetComponent<SenseTarget>();
        }

        protected override void OnIdle()
        {
            agent.speed = stats.walkSpeed;
            ChangeState(NpcState.Patrol);
        }

        protected override void OnPatrol()
        {
            if (guardPost == null)
            {
                PatrolSpots();
            }
            else
            {
                agent.SetDestination(guardPost.position);
            }
        }

        void PatrolSpots()
        {
            if (spotNum >= patrolSpots.Count)
            {
                spotNum = 0;
            }

            agent.SetDestination(patrolSpots[spotNum].position);
            spotNum++;

            StartCoroutine(GoTo_PatrolSpot());
        }

        public UnityEvent onAttack_Event;

        protected override void OnAttack()
        {
            onAttack_Event.Invoke();
            Rotate rotate = gameObject.AddComponent<Rotate>();
            rotate.RotateTo(sense.currentTarget);
        }

        void Update()
        {
            State_On_Update();
        }

        void State_On_Update()
        {
            switch (currentState)
            {
                case NpcState.Chase:
                    Update_OnChase();
                    break;

                case NpcState.Attack:
                    Update_OnAttack();
                    break;

                default:
                    break;
            }
        }

        public float patrolTime = 3;

        IEnumerator GoTo_PatrolSpot()
        {
            Vector3 pos = agent.destination;
            
            yield return new WaitUntil(() => Vector3.Distance(agent.destination, transform.position) <= agent.stoppingDistance);
            anim.SetTrigger("LookAround");

            yield return new WaitForSeconds(patrolTime);
            ChangeState(NpcState.Idle);
        }

        SenseTarget sense;

        void Update_OnChase()
        {
            agent.speed = stats.runSpeed;
            if (sense.currentTarget != null)
            {
                Attack_OrChase();
            }
        }

        void Attack_OrChase()
        {
            if (sense.Check_Target_Distance_And_Raycast())
            {
                ChangeState(NpcState.Attack);
            }
            else
            {
                SetDestination_For_Chase();
            }
        }

        void SetDestination_For_Chase()
        {
            if (agent.enabled)
            {
                if (agent.destination != sense.currentTarget.position)
                {
                    agent.SetDestination(sense.currentTarget.position);
                }
            }
        }

        void Update_OnAttack()
        {
            if (agent.enabled)
            {
                agent.SetDestination(transform.position);
                Chase_OnCondition();
            }
        }

        void Chase_OnCondition()
        {
            if (sense.Check_Target_Distance_And_Raycast())
            {
                Trigger_Attack_Anim();
            }
            else
            {
                ChangeState(NpcState.Chase);
            }
        }

        void Trigger_Attack_Anim()
        {
            anim.SetTrigger("Attack");
            anim.SetInteger("AttackInt", UnityEngine.Random.Range(0, 2));
        }

        public override void Attacked(Transform attacker)
        {
            base.Attacked(attacker);
            sense.currentTarget = attacker.transform;
        }

        public override void Recieve_Attacked_Broadcast(Transform target, Transform attacker)
        {
            foreach (string tag in stats.protects)
            {
                if (target.tag == tag)
                    sense.currentTarget = attacker.transform;
            }
        }
    }
}
