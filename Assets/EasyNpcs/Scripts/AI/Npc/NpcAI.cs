using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Package
{
    public class NpcAI : NpcBase
    {
        public float scaredRunningSpeed;
        public float runningDistance;
        public float runningTime;

        DayAndNightControl dayAndNightControl;
        Work workScript;
        Behaviour homeScript;
        public Transform home;
        public Transform work;
        public Transform pos { get; private set; }

        protected override void Start()
        {
            base.Start();
            DayAndNightCycle_Initialize();
            pos = work;
            workScript = GetComponent<Work>();
            homeScript = GetComponent<Home>();
            currentCoolTime = 0;
        }

        void DayAndNightCycle_Initialize()
        {
            dayAndNightControl = FindObjectOfType<DayAndNightControl>();
            if (dayAndNightControl != null)
            {
                dayAndNightControl.OnMorningHandler += GoToWork;
                dayAndNightControl.OnEveningHandler += GoToHome;
            }
            else
            {
                Debug.Log("Add in dayAndNight control to scene for use of npc's life cycle");
            }
        }

        float currentCoolTime = 0;

        void Update()
        {
            if (attacker != null)
            {
                if (currentState != NpcState.Scared)
                    ChangeState(NpcState.Scared);
            }
            else
            {
                Conversation_CoolTime();
            }
        }

        void Conversation_CoolTime()
        {
            currentCoolTime += Time.deltaTime;
            if (currentCoolTime >= converCoolTime)
            {
                TryConversation();
                currentCoolTime = 0;
            }
        }

        Transform attacker;

        public override void Recieve_Attacked_Broadcast(Transform target, Transform attacker)
        {
            this.attacker = attacker;
        }

        public NpcAI Sense_NearbyNpc(Vector3 position, float visionRange)
        {
            RaycastHit[] hits = Physics.SphereCastAll(position, visionRange, transform.forward, 0);
            List<NpcBase> npcs = ReturnNpcs_InRaycast(hits);
            NpcAI returnNpc = Return_RandomNpc(npcs);
            return returnNpc;
        }

        NpcAI Return_RandomNpc(List<NpcBase> npcs)
        {
            List<NpcAI> npcAIs = new List<NpcAI>();
            foreach (NpcBase npc in npcs)
            {
                if (npc is NpcAI)
                {
                    npcAIs.Add(npc as NpcAI);
                }
            }

            if (npcAIs.Count > 0)
                return npcAIs[UnityEngine.Random.Range(0, npcs.Count - 1)];
            else
                return null;
        }

        protected override void TurnOffBehaviour(NpcState prevState)
        {
            base.TurnOffBehaviour(prevState);
            switch (prevState)
            {
                case NpcState.GoingToWork:
                    Destroy(GetComponent<WaitTillReach_WorkHome>());
                    break;

                case NpcState.GoingHome:
                    Destroy(GetComponent<WaitTillReach_WorkHome>());
                    break;

                case NpcState.Working:
                    if (workScript != null)
                        workScript.WorkDisable();
                    break;

                case NpcState.AtHome:
                    if (homeScript != null)
                        homeScript.enabled = false;
                    break;

                case NpcState.Scared:
                    if (run_From_Danger != null)
                    {
                        Destroy(run_From_Danger);   
                    }
                    attacker = null;
                    break;
            }
        }

        protected override void OnIdle()
        {
            if (dayAndNightControl != null)
            {
                float time = dayAndNightControl.currentTime;
                OnTime_OfDay(time);
            }

            agent.speed = stats.walkSpeed;
        }

        void OnTime_OfDay(float time)
        {
            if (enabled)
            {
                if (time > .3f && time < .7f)
                {
                    GoToWork();
                }
                else
                {
                    GoToHome();
                }
            }
        }

        void GoToWork()
        {
            ChangeState(NpcState.GoingToWork);
        }

        protected override void OnGoingToWork()
        {
            Set_Cycle_Class().Start_GOTOWork();
        }

        void GoToHome()
        {
            ChangeState(NpcState.GoingHome);
        }

        protected override void OnGoingToHome()
        {
            Set_Cycle_Class().Start_GOTOHome();
        }

        WaitTillReach_WorkHome Set_Cycle_Class()
        {
            WaitTillReach_WorkHome lifeCycle = gameObject.AddComponent<WaitTillReach_WorkHome>();
            lifeCycle.Set(this);

            return lifeCycle;
        }

        protected override void OnWorking()
        {
            if (workScript != null)
                workScript.enabled = true;
        }

        protected override void OnAtHome()
        {
            if (homeScript != null)
                homeScript.enabled = true;
        }

        public override void Attacked(Transform attacker)
        {
            base.Attacked(attacker);
            this.attacker = attacker;
            
            if (resistOnAttack)
            {
                ChangeToCombat(attacker);
            }
            else
            {
                ChangeState(NpcState.Scared);
            }
        }

        Run_From_Danger run_From_Danger;

        protected override void OnScared()
        {
            run_From_Danger = gameObject.AddComponent<Run_From_Danger>();
            if (!GetComponent<AI_Stats>().isDead)
            {
                agent.speed = stats.runSpeed;
                StartCoroutine(run_From_Danger.Run(attacker));
            }
            else
            {
                run_From_Danger.attacker = attacker;
            }
        }

        [Range(0, 10)]
        public int converFactor = 0;
        public int converCoolTime = 30;

        void TryConversation()
        {
            NpcAI nearbyNpc = Sense_NearbyNpc(transform.position, stats.visionRange);
            if (nearbyNpc != null)
            {
                if (CheckConditions_ForTalk(nearbyNpc))
                {
                    RunConversation runConversation = gameObject.AddComponent<RunConversation>();
                    runConversation.Set(nearbyNpc, true);
                }
            }
        }

        bool CheckConditions_ForTalk(NpcAI npc)
        {
            if (npc.enabled == false)
                return false;
            if (currentState == NpcState.AtHome)
                return false;
            if (currentState == NpcState.Talking || npc.currentState == NpcState.Talking)
                return false;
            if (UnityEngine.Random.Range(0, 10) > converFactor)
                return false;
            if (GetInstanceID() < npc.GetInstanceID())
                return false;

            return true;
        }

        public void MoveTo(Transform point)
        {
            pos = point;
            ChangeState(NpcState.Idle);
        }

        public bool resistOnAttack = false;

        void ChangeToCombat(Transform attacker)
        {
            GetComponent<SenseTarget>().currentTarget = attacker;
        }
    }
}