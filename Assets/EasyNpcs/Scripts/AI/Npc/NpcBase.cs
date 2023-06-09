using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

namespace AI_Package
{
    public class NpcBase : MonoBehaviour
    {
        [HideInInspector]
        public NavMeshAgent agent;
        protected Animator anim;

        [HideInInspector]
        public TMP_Text textMesh;

        public NpcState currentState { get; protected set; }

        protected AI_Stats stats;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            stats = GetComponent<AI_Stats>();
        }

        protected virtual void Start()
        {
            anim = GetComponentInChildren<Animator>();
            textMesh = GetComponentInChildren<TMP_Text>();
        }

        public virtual bool ChangeState(NpcState newState)
        {
            if (enabled)
            {
                if (IsStageChangeAble(newState))
                {
                    StopAllCoroutines();

                    NpcState oldState = currentState;
                    currentState = newState;
                    OnStateChanged(oldState, newState);

                    return true;
                }
            }

            return false;
        }

        bool IsStageChangeAble(NpcState newState)
        {
            if (newState != NpcState.Idle)
            {
                if (currentState == newState && newState == NpcState.Scared)
                    return false;

                return IsStateAdvantage_BiggerThan_Previous(newState);
            }

            return true;
        }

        bool IsStateAdvantage_BiggerThan_Previous(NpcState newState)
        {
            int currentStateAdvantage = StateAdvantageNumber(currentState);
            int newStateAdvantage = StateAdvantageNumber(newState);
            if (newStateAdvantage < currentStateAdvantage)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        int StateAdvantageNumber(NpcState state)
        {
            switch (state)
            {
                case NpcState.Idle:
                    return 0;

                case NpcState.GoingToWork:
                    return 1;

                case NpcState.Working:
                    return 1;

                case NpcState.GoingHome:
                    return 1;

                case NpcState.AtHome:
                    return 1;

                case NpcState.Talking:
                    return 2;

                case NpcState.Scared:
                    return 4;

                case NpcState.Patrol:
                    return 1;

                case NpcState.Chase:
                    return 1;

                case NpcState.Attack:
                    return 1;
            }

            return -1;
        }

        protected virtual void OnStateChanged(NpcState prevState, NpcState newState)
        {
            TurnOffBehaviour(prevState);
            switch (newState)
            {
                case NpcState.Idle:
                    OnIdle();
                    break;

                case NpcState.GoingToWork:
                    OnGoingToWork();
                    break;

                case NpcState.Working:
                    OnWorking();
                    break;

                case NpcState.GoingHome:
                    OnGoingToHome();
                    break;

                case NpcState.AtHome:
                    OnAtHome();
                    break;

                case NpcState.Scared:
                    OnScared();
                    break;

                case NpcState.Patrol:
                    OnPatrol();
                    break;

                case NpcState.Attack:
                    OnAttack();
                    break;
            }
        }

        protected virtual void TurnOffBehaviour(NpcState prevState)
        {
            switch (prevState)
            {
                case NpcState.Talking:
                    GetComponentInChildren<TextMeshPro>().text = null;
                    Destroy(GetComponent<RunConversation>());
                    break;

                case NpcState.Attack:
                    Destroy(GetComponent<Rotate>());
                    break;
            }
        }

        protected virtual void OnIdle() { }

        protected virtual void OnGoingToWork() { }

        protected virtual void OnGoingToHome() { }

        protected virtual void OnWorking() { }

        protected virtual void OnAtHome() { }

        protected virtual void OnScared() { }

        protected virtual void OnPatrol() { }

        protected virtual void OnAttack() { }

        public virtual void Attacked(Transform attacker)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, stats.visionRange, transform.forward, 0);
            List<NpcBase> npcs = ReturnNpcs_InRaycast(hits);
            foreach (NpcBase npc in npcs)
            {
                npc.Recieve_Attacked_Broadcast(transform, attacker);
            }
        }

        protected List<NpcBase> ReturnNpcs_InRaycast(RaycastHit[] hits)
        {
            List<NpcBase> npcs = new List<NpcBase>();
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.GetComponent<NpcBase>())
                {
                    NpcBase npc = hit.transform.GetComponent<NpcBase>();
                    if (npc != this)
                    {
                        npcs.Add(npc);
                    }
                }
            }

            return npcs;
        }

        public virtual void Recieve_Attacked_Broadcast(Transform target, Transform attacker) { }

        private void OnEnable()
        {
            ChangeState(NpcState.Idle);
        }

        public void Disable()
        {
            enabled = false;
        }

        public void OnDisable()
        {
            TurnOffBehaviour(currentState);
            StopAllCoroutines();
            anim.SetFloat("Speed", 0);
        }

        public void BroadCastDeath()
        {
            enabled = false;
        }
    }
}