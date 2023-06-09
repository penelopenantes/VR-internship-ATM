using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI_Package
{
    public class Ragdoll : MonoBehaviour
    {
        Rigidbody[] rigs;
        SkinnedMeshRenderer[] skins;

        void Start()
        {
            skins = GetComponentsInChildren<SkinnedMeshRenderer>();
            rigs = GetComponentsInChildren<Rigidbody>();
            Turn_ChildRigs(true);
            GetComponent<CapsuleCollider>().enabled = true;
        }

        void Turn_ChildRigs(bool on)
        {
            foreach (Rigidbody rigidbody in rigs)
            {
                Rigidbody mainRigidBody = GetComponent<Rigidbody>();
                if (rigidbody != mainRigidBody)
                {
                    rigidbody.isKinematic = on;
                }
            }
        }

        public void ActivateRagdoll()
        {
            DestroyRotate();
            UpdateWhenOffScreen_For_Each_Rig();

            GetComponentInChildren<Animator>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponent<Rigidbody>());
            Turn_ChildRigs(false);
        }

        void UpdateWhenOffScreen_For_Each_Rig()
        {
            foreach (SkinnedMeshRenderer skinned in skins)
            {
                skinned.updateWhenOffscreen = true;
            }
        }

        void DestroyRotate()
        {
            Rotate rotate = GetComponent<Rotate>();
            if (rotate != null)
            {
                Destroy(rotate);
            }
        }
    }
}