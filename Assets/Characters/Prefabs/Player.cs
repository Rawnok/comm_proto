using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InputRecognition input;

    [SerializeField]
    private float m_min_move_threshold = 0.15f;
    [SerializeField] [Range(0.85f, 1)]
    private float m_min_turn_threshold = 0.99f; // 1.0 = exactly looking at each other


    [SerializeField]
    private float m_move_speed = 5;

    [SerializeField]
    private float m_turn_speed = 10;
    

    private Vector3 m_current_destination;
    private bool m_is_moving;

    void Start ()
    {
        input.on_mouse_over_terrain_observers += OnMouseOverTerrain;
        input.on_mouse_over_enemy_observers += OnMouseOverEnemy;

        m_current_destination = transform.position;
    }//start

    private void Update ()
    {
        MoveToCurrentDestination ();
    }//update

    private void MoveToCurrentDestination ()
    {
        // first rotate towards destination, 
        if ( !IsLookingAt (m_current_destination) ) // not looking at destination
        {
            //Debug.Log ( "not looking" );
            Vector3 targetDirection = m_current_destination - transform.position;

            Vector3 direction = Vector3.RotateTowards ( transform.forward, targetDirection, Time.deltaTime * m_turn_speed, 0.0f );
            transform.rotation = Quaternion.LookRotation ( direction );
        }
        else
        {
            if ( Vector3.Distance ( transform.position, m_current_destination ) >= m_min_move_threshold )
            {
                transform.position = Vector3.MoveTowards ( transform.position, m_current_destination, Time.deltaTime * m_move_speed );
            }
        }
    }

    private bool IsLookingAt (Vector3 destination)
    {
        Vector3 direction = ( destination  - transform.position).normalized;
        float dotProduct = Vector3.Dot ( transform.forward , direction ) ;
        return dotProduct >= m_min_turn_threshold; 
    }

    private void OnMouseOverTerrain ( Vector3 destination )
    {
        if (Input.GetMouseButtonDown(0))
        {
            Move (destination);
        }
    }

    private void Move ( Vector3 destination )
    {
        m_current_destination = destination;
    }
    
    private void OnMouseOverEnemy ( )
    {
        if ( Input.GetMouseButtonDown ( 0 ) )
        {
            Debug.Log ( "Player should move towards enemy " );
            //transform.position = destination; /// TODO, animation to move
        }
    }
    
    private void OnDrawGizmos ()
    {
        Vector3 direction = ( transform.forward * 5 );
        Gizmos.DrawRay ( transform.position, direction );
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere ( m_current_destination, 0.5f );
    }
}
