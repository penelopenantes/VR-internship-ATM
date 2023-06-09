using System;
using UnityEngine;

namespace AI_Package
{
    public class Projectile : MonoBehaviour
    {
        private Transform caster;
        private Transform target;

        Rigidbody rigidBody;

        [Range(20.0f, 75.0f)] public float LaunchAngle;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        public void Fire(Transform caster, Transform getTarget)
        {
            this.caster = caster;
            target = getTarget;

            Fly();
        }

        void Update()
        {
            transform.rotation = Quaternion.LookRotation(rigidBody.velocity);
        }

        private void Fly()
        {
            Vector3 projectileXZPos = new Vector3(transform.position.x, 1.7f, transform.position.z);
            Vector3 targetXZPos = new Vector3(target.transform.position.x, 1.7f, target.transform.position.z);
            targetXZPos += -0.5f * (projectileXZPos - targetXZPos).normalized;

            CalculateProjectory(projectileXZPos, targetXZPos);
        }

        void CalculateProjectory(Vector3 projectileXZPos, Vector3 targetXZPos)
        {
            // rotate the object to face the target
            transform.LookAt(targetXZPos);

            // shorthands for the formula
            float R = Vector3.Distance(projectileXZPos, targetXZPos);
            float G = Physics.gravity.y;
            float tanAlpha = Mathf.Tan(LaunchAngle * Mathf.Deg2Rad);
            float H = target.transform.position.y - transform.position.y;

            // calculate the local space components of the velocity 
            // required to land the projectile on the target object 
            float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
            float Vy = tanAlpha * Vz;

            // create the velocity vector in local space and get it in global space
            Vector3 localVelocity = new Vector3(0f, Vy, Vz);
            Vector3 globalVelocity = transform.TransformDirection(localVelocity);

            // launch the object by setting its initial velocity and flipping its state
            rigidBody.velocity = globalVelocity;
        }

        public GameObject bloodGush;

        private void OnCollisionEnter(Collision other)
        {
            AttackManager.AttackTarget(caster, target);
            Quaternion rotation = target.transform.rotation * Quaternion.Euler(UnityEngine.Random.Range(-90, 90), UnityEngine.Random.Range(-90, 90), 0);
            Destroy(gameObject);
        }
    }
}