using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerMode
    {
        IDLE = 0,
        MOVE,
        ATTACK
    }
    public PlayerMode m_current_mode;

    [SerializeField]
    private InputRecognition input;

    [SerializeField]
    private float m_min_move_threshold = 0.15f;

    [SerializeField] [Range ( 1.2f, 2.0f )]
    private float m_min_melee_attack_threshold = 1.0f;

    [SerializeField] [Range(0.85f, 1)]
    private float m_min_turn_threshold = 0.99f; // 1.0 = exactly looking at each other

    [SerializeField]
    private float m_move_speed = 5;

    [SerializeField]
    private float m_turn_speed = 10;
    
    private Vector3 m_current_destination;

    public CharacterType m_char_type;

    void Start ()
    {

        input.on_mouse_over_terrain_observers += OnMouseOverTerrain;
        input.on_mouse_over_ladder_observers += OnMouseOverLadder;
        input.on_mouse_over_collectable_observers += OnMouseOverCollectables;
        input.on_mouse_over_enemy_observers += OnMouseOverEnemy;
        
        ///TODO, have to tweek turn / speed values according to profile
        CharacterProfile my_profile = GameManager.instance.GetCharacterProfile (m_char_type);
        //modify speed and other values from cp
        if ( my_profile != null )
        {
            m_move_speed = m_move_speed * my_profile.speed_multiplier;
        }
        else
        {
            Debug.LogFormat ("No character profile found for {0} ", m_char_type );
        }

        m_current_destination = transform.position;
    }//start
    
    private void Update ()
    {
        MoveToCurrentDestination ();
    }//update

    private void MoveToCurrentDestination ()
    {
        // first look towards destination, 
        // then move to destination
        if ( !IsLookingAt (m_current_destination) )
        {
            Vector3 targetDirection = m_current_destination - transform.position;
            Vector3 direction = Vector3.RotateTowards ( transform.forward, targetDirection, Time.deltaTime * m_turn_speed, 0.0f );
            transform.rotation = Quaternion.LookRotation ( direction );
        }
        else
        {
            if ( m_current_mode == PlayerMode.MOVE )
            {
                if ( Vector3.Distance ( transform.position, m_current_destination ) >= m_min_move_threshold )
                {
                    transform.position = Vector3.MoveTowards ( transform.position, m_current_destination, Time.deltaTime * m_move_speed );
                }
                else // reached to current destination, change mode to stationary
                {
                    ///TODO might need to change when we have collect command
                    m_current_mode = PlayerMode.IDLE;
                }

            }
            else if (m_current_mode == PlayerMode.ATTACK)
            {
                if ( Vector3.Distance ( transform.position, m_current_destination ) >= m_min_melee_attack_threshold )
                {
                    transform.position = Vector3.MoveTowards ( transform.position, m_current_destination, Time.deltaTime * m_move_speed );
                }
                else // we are close enough for a melee atttack
                {
                    DoMeleeAttack ();
                    // melee attack done,go back to stationary position
                    m_current_mode = PlayerMode.IDLE;
                }
            }
        }
    }//MoveToCurrentDestination

    private bool IsLookingAt ( Vector3 destination )
    {
        Vector3 direction = ( destination - transform.position ).normalized;
        float dotProduct = Vector3.Dot ( transform.forward, direction );
        return dotProduct >= m_min_turn_threshold;
    }

    private void Move ( Vector3 destination )
    {
        m_current_mode = PlayerMode.MOVE;
        m_current_destination = destination;
    }

    private void DoMeleeAttack ()
    {
        Debug.Log ("Player should now attack enemy : melee attack");
    }

    private bool IsOnSelectionPool ()
    {
        return ( GameManager.instance.m_selection_pool.Contains ( m_char_type ) );
    }

    #region input subscribers

    public void OnMouseOverLadder ( Ladder ladder )
    {
        // move to ladder point, climb up
        if (!IsOnSelectionPool())
        {
            return;
        }
    }

    public void OnMouseOverCollectables ( Collectables collectable_object )
    {
        //examine if the player can pick up current object, 
        if ( !IsOnSelectionPool () )
        {
            return;
        }
    }

    public void OnMouseOverTerrain ( Vector3 destination )
    {
        if ( !IsOnSelectionPool () )
        {
            return;
        }

        if ( Input.GetMouseButtonDown ( 0 ) )
        {
            Move ( destination );
        }
    }

    public void OnMouseOverEnemy ( Enemy enemy )
    {
        if ( !IsOnSelectionPool () )
        {
            return;
        }

        if ( Input.GetMouseButtonDown ( 0 ) )
        {
            m_current_mode = PlayerMode.ATTACK;
            m_current_destination = enemy.gameObject.transform.position;
        }
    }
    #endregion
    
    #region GIZMOS
    private void OnDrawGizmos ()
    {
        Vector3 direction = ( transform.forward * 5 );
        Gizmos.DrawRay ( transform.position, direction );
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere ( m_current_destination, 0.5f );
    } 
    #endregion
}
