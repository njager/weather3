using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{

    public GameObject myGameObject;
    // Start is called before the first frame update
    
    private Vector3 target = new Vector3(231.0f, 148.0f, -58.0f);

         void Update()
         {
            // Spin the object around the world origin at 20 degrees/second.
            transform.RotateAround(target, Vector3.forward, 30 * Time.deltaTime);
         }

    
}
