using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyInputController : MonoBehaviour { 
    void Update() 
    {
        ButtonTest();
    } 
    private void ButtonTest() 
    {
        string msg = null;
        if (Input.GetButtonDown("Fire1"))
        {
            msg = "Fire1 down";
        }
        if (Input.GetButtonUp("Fire1"))
        {
            msg = "Fire1 up";
        }
        if (msg != null)
        {
            Debug.Log("Input: " + msg);
        } else
        {
            Debug.Log("ayayayaye");
        }
    } 
}