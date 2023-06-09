using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySound : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DestroySoundObject());
    }

    IEnumerator DestroySoundObject()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
