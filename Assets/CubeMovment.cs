using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CubeMovment : MonoBehaviour {


    public Transform Destination;

    private NavMeshAgent agent;
    private bool isTraversingLink = false;
    private OffMeshLinkData currentOffMeshLinkData;
	// Use this for initialization
	void Start () {
        agent = this.GetComponent<NavMeshAgent> ();
        agent.autoTraverseOffMeshLink = false;
	}


    float totalTimeInMeshLink = 0;

	void Update () {
        //agent.destination = Destination.position;
        if ( agent.isOnOffMeshLink )
        {
            if (!isTraversingLink)
            {
                ///TODO animation
                //current link data
                currentOffMeshLinkData = agent.currentOffMeshLinkData;
                isTraversingLink = true;
                totalTimeInMeshLink = Time.time;
            }
            

            transform.position = new Vector3( currentOffMeshLinkData.startPos.x, 
                currentOffMeshLinkData.startPos.y + 0.5f, 
                currentOffMeshLinkData.startPos.z ) ;


            //Vector3 targetDir = currentOffMeshLinkData.startPos - transform.position;
            transform.LookAt ( currentOffMeshLinkData.startPos );

             //Quaternion.RotateTowards ( transform.rotation, targetDir, Time.deltaTime * 2 );

            //var tLerp = Time.deltaTime; // it will come from animtaion.normalizedTime
            //var newPos = Vector3.Lerp ( currentOffMeshLinkData.startPos, currentOffMeshLinkData.endPos, tLerp );
            //newPos.y += 2f * Mathf.Sin ( Mathf.PI * tLerp );
            //transform.position = newPos;

            if (Mathf.Abs( totalTimeInMeshLink - Time.time ) > 3f)
            {
                transform.position = new Vector3 ( currentOffMeshLinkData.endPos.x,
                    currentOffMeshLinkData.endPos.y + 0.5f,
                    currentOffMeshLinkData.endPos.z );
                transform.LookAt ( Destination.position );
                isTraversingLink = false;
                agent.CompleteOffMeshLink ();
            }
            
            //if (Vector3.Distance(transform.position, currentOffMeshLinkData.endPos) <=0.1f) // we are at the end
            //{
            //    Debug.Log ( Vector3.Distance ( transform.position, currentOffMeshLinkData.endPos ) );
            //}
        }
        else
        {
            agent.destination = Destination.position;
        }
    }
}
