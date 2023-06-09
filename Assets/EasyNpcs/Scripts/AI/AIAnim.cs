using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAnim : MonoBehaviour
{
    Animator anim;
    NavMeshAgent agent;

    AudioSource audioSource;
    public AudioClip clip;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponentInParent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        anim.SetFloat("Speed", agent.velocity.magnitude);

        if (agent.velocity.magnitude > 0)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
        else
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
    }
}
