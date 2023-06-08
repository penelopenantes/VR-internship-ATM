using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class RayViewerComplete : MonoBehaviour
{

    public float weaponRange = 50f;                       // Distance in Unity units over which the Debug.DrawRay will be drawn

    private Camera fpsCam;                                // Holds a reference to the first person camera


    void Start()
    {
        // Get and store a reference to our Camera by searching this GameObject and its parents
        Debug.Log(this.name);

        //First, Find the Parent Object which is either EnemyObject or EnemyObject(Clone)
        Transform parent = transform.parent;

        //Now, Find it's Enemy Object
        fpsCam = parent.GetComponent<Camera>();

        //fspCam = GameObject.Find("../VRCamera");
        //fpsCam = GetComponentInParent<Camera>();
    }


    void Update()
    {
        // Create a vector at the center of our camera's viewport
        Vector3 lineOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        // Draw a line in the Scene View  from the point lineOrigin in the direction of fpsCam.transform.forward * weaponRange, using the color green
        Debug.DrawRay(lineOrigin, fpsCam.transform.forward * weaponRange, Color.green);
    }
}
