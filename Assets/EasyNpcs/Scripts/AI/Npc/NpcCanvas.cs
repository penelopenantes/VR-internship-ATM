using UnityEngine.Events;
using UnityEngine;
using TMPro;
using AI_Package;


public class NpcCanvas : MonoBehaviour
{
    TMP_Text text;

    Canvas canvas;
    public Camera playerCam;

    private void Awake()
    {
        if(text == null)
            text = GetComponentInChildren<TMP_Text>();
        if(canvas == null)
            canvas = GetComponent<Canvas>();
        if (playerCam == null)
            playerCam = Camera.main;
     
    }

    private void Start()
    {
        var parent = GetComponentInParent<NpcBase>();
        if (parent == null)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        canvas.transform.LookAt(playerCam.transform.position);
    }
}
