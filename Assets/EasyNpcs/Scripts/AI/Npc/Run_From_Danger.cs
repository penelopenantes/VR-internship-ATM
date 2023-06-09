using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using AI_Package;

public class Run_From_Danger : MonoBehaviour
{
    NavMeshAgent agent;
    NpcAI npc;
    public Transform attacker;
    private float runTimeLeft = 10;

    private void Update()
    {
        runTimeLeft -= Time.deltaTime;
    }

    public IEnumerator Run(Transform attacker)
    {
        this.attacker = attacker;
        Initalize();

        runTimeLeft = 10;
        while (runTimeLeft > 0)
        {
            CalculatePath();
            yield return new WaitUntil(() => Vector3.Distance(agent.destination, transform.position) <= npc.runningDistance / 1.2);
        }
        
        npc.ChangeState(NpcState.Idle);
        Destroy(this);
    }

    void Initalize()
    {
        agent = GetComponent<NavMeshAgent>();
        npc = GetComponent<NpcAI>();

        runTimeLeft = npc.runningTime;
        agent.ResetPath();
    }

    void CalculatePath()
    {
        NavMeshPath path = new NavMeshPath();

        double[] angleXY = CalculateDirectionOfPoint();
        double angleX = angleXY[0];
        double angleY = angleXY[1];

        RotateAround_Point_ForValidPosition(angleX, angleY);
    }

    void RotateAround_Point_ForValidPosition(double angleX, double angleY)
    {
        int index = 0;
        const int limit = 13;

        do
        {
            angleX += index * Math.Pow(-1.0f, index) * Math.PI / 6.0f;
            angleY -= index * Math.Pow(-1.0f, index) * Math.PI / 6.0f;
            Vector3 goal = PointToGo(angleX, angleY);
            if (SetDestination_ToPoint(goal))
            {
                return;
            }

            index++;
        } while (index < limit);

        agent.destination = transform.position;
    }

    bool SetDestination_ToPoint(Vector3 goal)
    {
        bool samplePosition = NavMesh.SamplePosition(goal, out NavMeshHit hit, npc.runningDistance / 5, agent.areaMask);
        if (samplePosition)
        {
            agent.SetDestination(hit.position);
            return true;
        }
        else
        {
            return false;
        }
    }

    Vector3 PointToGo(Double angleX, double angleY)
    {
        Vector2 direction = new Vector2((float)Math.Cos(angleX), (float)Math.Sin(angleY));
        return new Vector3(transform.position.x - direction.x * npc.runningDistance, transform.position.y, transform.position.z - direction.y * npc.runningDistance);
    }

    double[] CalculateDirectionOfPoint()
    {
        Vector3 attackerToNpc = attacker.transform.position - transform.position;
        float twoDimension_magnitude = new Vector2(attackerToNpc.x, attackerToNpc.z).magnitude;
        double angleX = Math.Acos(attackerToNpc.x / twoDimension_magnitude);
        double angleY = Math.Asin(attackerToNpc.z / twoDimension_magnitude);
        double[] anglesXY = {angleX, angleY};

        return anglesXY;
    }
}
