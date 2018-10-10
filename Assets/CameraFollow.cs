using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public GameObject playerObject;

    public float followHeight;
    public float followDistance;
    
    void Update ()
    {
        float targetHeight = playerObject.transform.position.y + followHeight;

        float currentHeight = Mathf.Lerp (transform.position.y , targetHeight, 0.9f * Time.deltaTime);

        float targetDistance = playerObject.transform.position.z - followDistance;
        Vector3 targetPos = new Vector3 (0, currentHeight, targetDistance);
        
        transform.position = targetPos;
        //look at will controll the rotation
        transform.LookAt ( playerObject.transform );

    }
}
