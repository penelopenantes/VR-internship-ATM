using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEditor;

namespace AI_Package
{
    public class SenseTarget : MonoBehaviour
    {
        private void Awake()
        {
            stats = GetComponent<AI_Stats>();
            npcAI = GetComponent<NpcAI>();
            combatAI = GetComponent<CombatAI>();
            rig = GetComponentInChildren<MultiAimConstraint>();
            rigBuilder = GetComponent<RigBuilder>();
        }

        AI_Stats stats;
        NpcAI npcAI;
        CombatAI combatAI;
        MultiAimConstraint rig;
        RigBuilder rigBuilder;

        Transform _currentTarget;
        public Transform currentTarget
        {
            get { return _currentTarget; }
            set 
            { 
                _currentTarget = value;
                ChangeScripts(value);
            }
        }

        void ChangeScripts(Transform target)
        {
            if (_currentTarget != null)
            {
                npcAI.enabled = false;
                combatAI.enabled = true;
                combatAI.ChangeState(NpcState.Chase);
                
                RigToTarget(target);
            }
            else
            {
                combatAI.ChangeState(NpcState.Idle);
            }
        }

        bool buildRig = false;

        void RigToTarget(Transform target)
        {
            var data = rig.data.sourceObjects;
            if (data.Count > 0)
            {
                data[0] = new WeightedTransform(target.Find("Armature/Root/Spine_Lower/Spine_Middle/Spine_Upper/Neck/Head"), 1);
            }
            else
            {
                data.Add(new WeightedTransform(target.Find("Armature/Root/Spine_Lower/Spine_Middle/Spine_Upper/Neck/Head"), 1));
            }
            
            rig.data.sourceObjects = data;
            buildRig = true;
        }

        public void RemoveRig()
        {
            var data = rig.data.sourceObjects;
            data.Clear();
            rig.data.sourceObjects = data;

            buildRig = true;
        }

        private void Update()
        {
            if (buildRig == true)
            {
                rigBuilder.Build();
                buildRig = false;
            }

            UpdateBehavior();
        }

        void UpdateBehavior()
        {
            if (currentTarget == null)
            {
                Transform target = CheckForTargets();
                if (target != null)
                {
                    currentTarget = target;
                }
            }
            else
            {
                if (currentTarget.GetComponent<Stats>().isDead)
                {
                    currentTarget = null;
                }
            }
        }

        public Transform CheckForTargets()
        {
            List<Collider> possibleTargets = PossibleTargets();
            Collider nearestTarget = NearestTarget(possibleTargets);
            if (nearestTarget != null)
                return nearestTarget.transform;
            else
                return null;
        }

        List<Collider> PossibleTargets()
        {
            List<Collider> posssibleTargets = new List<Collider>();
            Collider[] cols = Physics.OverlapSphere(transform.position, stats.visionRange, stats.visionLayers);
            foreach (Collider col in cols)
            {
                if (Physics.Linecast(transform.position + new Vector3(0, 1), col.transform.position + new Vector3(0, 1), out RaycastHit hit, stats.visionLayers))
                {
                    if (CheckTag(col))
                    {
                        if (CheckAngle(col) < stats.visionAngle)
                            posssibleTargets.Add(col);
                    }
                }
            }

            return posssibleTargets;
        }

        bool CheckTag(Collider col)
        {
            for (int i = 0; i < stats.enemies.Count; i++)
            {
                if (col.gameObject.CompareTag(stats.enemies[i]))
                {
                    return true;
                }
            }

            return false;
        }

        float CheckAngle(Collider col)
        {
            Vector3 targetDir = col.transform.position - transform.position;
            return Vector3.Angle(targetDir, transform.forward);
        }

        Collider NearestTarget(List<Collider> possibleTargets)
        {
            Collider nearestTarget = null;
            if (possibleTargets.Count > 0)
            {
                nearestTarget = possibleTargets[0];
                for (int i = 1; i < possibleTargets.Count; i++)
                {
                    if (Vector3.Distance(possibleTargets[i].transform.position, transform.position)
                        < Vector3.Distance(nearestTarget.transform.position, transform.position))
                        nearestTarget = possibleTargets[i];
                }
            }

            return nearestTarget;
        }

        public bool Check_Target_Distance_And_Raycast()
        {
            RaycastHit hit;
            Physics.Raycast(transform.position + new Vector3(0, 1), 
                currentTarget.position - transform.position, out hit, stats.attackDistance - 0.5f);
            if (hit.transform == currentTarget)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected, typeof(Transform))]
        public static void DrawGizmo(Transform npc, GizmoType type)
        {
            Gizmos.color = Color.green;
            CapsuleCollider collider = npc.GetComponent<CapsuleCollider>();
            Vector3 npcPos = npc.transform.position;
            Gizmos.matrix = Matrix4x4.TRS(new Vector3(npcPos.x, collider == null ? npcPos.y : npcPos.y + collider.height * 0.9f, npcPos.z),
                npc.transform.rotation, npc.transform.lossyScale);

            if (Selection.Contains(npc.gameObject))
            {
                AI_Stats npcInfo = npc.GetComponent<AI_Stats>();
                if (npcInfo != null)
                {
                    Gizmos.DrawFrustum(Vector3.zero, npcInfo.visionAngle, npcInfo.visionRange, 0f, 2.5f);
                }
            }
        }
    }
}