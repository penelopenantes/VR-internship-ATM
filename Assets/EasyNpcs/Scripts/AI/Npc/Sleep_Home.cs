using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI_Package
{
    public class Sleep_Home : Home
    {
        public Transform bed;
        public Transform start;
        public Transform rotation;
        bool move;

        public float bedHeight = 0;
        public float rotateSec = 0;

        NavMeshAgent agent;
        Rotate rotate;

        private void OnEnable()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.SetDestination(start.position);
            StartCoroutine(LieOnBed());
            move = false;
        }

        private void Update()
        {
            if (move == true)
            {
                transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, transform.position.z),
                    new Vector3(bed.position.x, bed.position.y + bedHeight, bed.position.z), Time.deltaTime * 1/2);
            }
        }

        IEnumerator LieOnBed()
        {
            yield return new WaitUntil(() => Vector3.Distance(transform.position, start.position) < 0.5f);
            agent.enabled = false;
            rotate = gameObject.AddComponent<Rotate>();
            rotate.RotateTo(rotation);
            GetComponent<Animator>().SetBool("LyingDown", true);

            move = true;

            yield return new WaitForSeconds(rotateSec);
            Destroy(rotate);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            agent.enabled = true;
            GetComponent<Animator>().SetBool("LyingDown", false);
            if (rotation != null)
                Destroy(GetComponent<Rotate>());
        }
    }
}