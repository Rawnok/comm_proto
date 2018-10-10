using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ACTION_TYPE
{
    DEFAULT_OR_NONE = 0,
    CLIMB,
}

public class Player : MonoBehaviour
{
    public bool disable_custom_animation = false;

    [SerializeField]
    private InputRecognition input;

    [SerializeField]
    private float m_min_move_threshold = 0.15f;

    [SerializeField]
    [Range ( 0.3f, 15.0f )]
    private float m_current_attack_range = 1.0f;

    [SerializeField]
    [Range ( 0.85f, 1 )]
    private float m_min_turn_threshold = 0.99f; // 1.0 = exactly looking at each other

    [SerializeField]
    private float m_move_speed = 5;

    [SerializeField]
    private float m_turn_speed = 10;

    [SerializeField]
    private CharacterProfile m_profile;

    [SerializeField]
    private Enemy m_current_target = null;

    [SerializeField]
    private Ladder m_current_ladder = null;

    [SerializeField]
    private NavMeshAgent m_nav_mesh;

    // pure private variables
    private Vector3 m_current_destination;
    private Animator m_animator;
    
    private bool m_is_busy = false;
    
    //public variables
    public CharacterType m_char_type; // used by input recognition to allow other observers of  the access to current chosen type
    public AbilityType m_chosen_ability
    {
        get
        {
            if(m_profile != null)
                return m_profile.current_chosen_ability;
            return AbilityType.DEFAULT;
        }
    }
    public enum PlayerMode
    {
        IDLE = 0,
        MELEE_ACTION,
        RANGE_ACTION,
    }

    public PlayerMode m_current_mode; /// TODO, consider making it private

    void Start ()
    {
        input.on_mouse_over_terrain_observers += OnMouseOverTerrain;
        input.on_mouse_over_ladder_observers += OnMouseOverLadder;
        input.on_mouse_over_collectable_observers += OnMouseOverCollectables;
        input.on_mouse_over_enemy_observers += OnMouseOverEnemy;

        ///TODO, have to tweek turn / speed values according to profile
        m_profile = GameManager.instance.GetCharacterProfile ( m_char_type );
        //modify speed and other values from cp
        if ( m_profile != null )
        {
            m_move_speed = m_move_speed * m_profile.speed_multiplier;
        }
        else
        {
            Debug.LogFormat ( "No character profile found for {0} ", m_char_type );
        }

        m_animator = gameObject.GetComponentInChildren<Animator> ();


        if (m_animator == null)
        {
            Debug.LogError ("no animator found on the component");
        }


        m_nav_mesh = GetComponentInChildren<NavMeshAgent> ();

        // get current abilities
        m_current_destination = transform.position;
        //AnimatorController.DoClimb ( m_animator ); ///TODO testing
        m_nav_mesh.SetDestination ( m_current_destination );
    }//start

    private void Update ()
    {
        //transform.Translate ( Vector3.up * Time.deltaTime, Space.World );

        //if ( m_current_ladder != null )
        //{
        //    m_current_mode = PlayerMode.MELEE_ACTION;
        //    MoveToCurrentDestination ();
        //    return;
        //}

        switch ( m_chosen_ability )
        {
            case AbilityType.UNSET:
            case AbilityType.DEFAULT:
                m_current_mode = PlayerMode.IDLE;
                MoveToCurrentDestination ();
                break;
            case AbilityType.TRAP:
            case AbilityType.MEDIC:
            case AbilityType.RADIO:
            case AbilityType.HAND_PICK:
                m_current_mode = PlayerMode.MELEE_ACTION;
                MoveToCurrentDestination ();
                break;
            case AbilityType.KNIFE:
            case AbilityType.PUNCH:
                m_current_mode = (m_current_target) ? PlayerMode.MELEE_ACTION : PlayerMode.IDLE;
                MoveToCurrentDestination ();
                break;
            case AbilityType.THROW:
                break;
            case AbilityType.PISTOL:
                break;
            case AbilityType.SNIPING:
                ///TODO testing
                m_current_mode = ( m_current_target ) ? PlayerMode.MELEE_ACTION : PlayerMode.IDLE;
                MoveToCurrentDestination ();
                break;
        }
    }//update

    private void MoveToCurrentDestination ()
    {
        

        // first look towards destination, 
        // then move to destination
        if ( !IsLookingAt ( m_current_destination ) )
        {
            //Debug.Log ("looking at target");
            Vector3 targetDirection = m_current_destination - transform.position;
            Vector3 direction = Vector3.RotateTowards ( transform.forward, targetDirection, Time.deltaTime * m_turn_speed, 0.0f );
            transform.rotation = Quaternion.LookRotation ( direction );
        }
        else
        {
            float chosen_threshold = ( m_current_mode != PlayerMode.MELEE_ACTION ) ? m_min_move_threshold : m_current_attack_range;

            if ( Vector3.Distance ( transform.position, m_current_destination ) >= chosen_threshold )
            {
                MoveTowardsDestination ();
            }
            else
            {
                if ( m_current_mode == PlayerMode.MELEE_ACTION )
                {
                    //Debug.Log ( "melee action on" ); 
                    if ( !m_is_busy ) // currently not busy in a melee action
                        DoMeleeAction ();
                }
                else
                {
                    //Debug.Log ("idle mode, playing idle animation");
                    m_is_busy = false;
                    if ( !disable_custom_animation )
                    {
                        CharacterAnimatorController.DoNomralIdle ( m_animator );
                    }
                }
            }
        }
    }//MoveToCurrentDestination
    

    private void MoveTowardsDestination ()
    {
        //Debug.Log ( "moving towards destination" );
        m_is_busy = false;
        m_nav_mesh.SetDestination ( m_current_destination );
        //transform.position = Vector3.MoveTowards ( transform.position, m_current_destination, Time.deltaTime * m_move_speed );
        if (!disable_custom_animation)
        {
            CharacterAnimatorController.DoNormalWalk ( m_animator );
        } 
    }//movetowardsdestination

    private bool IsLookingAt ( Vector3 destination ) 
    {

        destination.y = -1; //no matter what height we are in, move to default height

        if (Vector3.Distance(destination, transform.position) <= 0.1f) 
        {
            // too small to judge direction
            return true;
        }

        Vector3 direction = ( destination - transform.position ).normalized;
        float dotProduct = Vector3.Dot ( transform.forward, direction );
        return dotProduct >= m_min_turn_threshold;
    }//islookingat

    private void ModifyDestination ( Vector3 destination )
    {
        m_current_destination = destination;
    }//modifydestination

    private void DoMeleeAction ()
    {
        Debug.Log ("do melee action");
        m_is_busy = true;

        if ( m_current_ladder == null )
        {
            switch ( m_chosen_ability )
            {
                case AbilityType.KNIFE:
                    StabCurrentTarget ();
                    break;
                case AbilityType.PUNCH:
                    break;
                case AbilityType.TRAP:
                    break;
                case AbilityType.MEDIC:
                    break;
                case AbilityType.RADIO:
                    break;
                case AbilityType.HAND_PICK:
                    break;
                case AbilityType.PISTOL:
                    //ShootCurrentTarget ();
                    break;
                case AbilityType.SNIPING: ///TODO, move this to ranged action
                    ShootCurrentTarget ();
                    break;
                default:
                    Debug.LogError ( "un recognized melee action detected" );
                    break;
            }
            StartCoroutine ( WaitForEndOfAction () );
        }
        else
        {

            /// TODO, figure out bottom or top
            transform.position = m_current_ladder.GetBottomPoint ().position; 
            transform.rotation = m_current_ladder.GetBottomPoint ().rotation;

            if (!disable_custom_animation)
                CharacterAnimatorController.DoClimb ( m_animator );

            StartCoroutine ( WaitForEndOfAction (ACTION_TYPE.CLIMB) );

            ///TODO start coroutine to end the current action
        }
    }

    private void ShootCurrentTarget ()
    {
        Debug.Log ("shotting target");
        if (m_current_target != null)
        {
            m_current_target.TakeDamage ( m_current_target.m_health ); ///TODO just for testing
        }

        if (!disable_custom_animation)
            CharacterAnimatorController.DoShoot ( m_animator );
    }

    private IEnumerator WaitForEndOfAction (ACTION_TYPE action_type = ACTION_TYPE.DEFAULT_OR_NONE)
    {
        switch ( action_type )
        {
            case ACTION_TYPE.CLIMB:
                /// TODO examine bottom point for end
                while ( m_current_ladder && transform.position.y < m_current_ladder.GetTopPoint ().position.y ) 
                {
                    transform.Translate ( Vector3.up * Time.deltaTime );
                    m_current_destination = transform.position;
                    yield return null;
                }

                if(!disable_custom_animation)
                    CharacterAnimatorController.DoClimbEnd ( m_animator );
                // 0.3 = offset
                yield return new WaitForSeconds ( m_animator.GetCurrentAnimatorStateInfo (0).length - 0.15f);

                if(!disable_custom_animation)
                    CharacterAnimatorController.DoNomralIdle (m_animator);


                m_current_destination = m_current_ladder.GetLandingSpace ( true ).position;
                transform.position = m_current_ladder.GetLandingSpace ( true ).position;
                //EndCurrentMeleeAction ( ACTION_TYPE.CLIMB );
                break;
            case ACTION_TYPE.DEFAULT_OR_NONE:
            default:
                
                break;
        }


        EndCurrentMeleeAction ();
    }

    private void EndCurrentMeleeAction ()
    {
        Debug.Log ("ending cucrrent melee action");
        m_current_ladder = null;
        m_current_target = null; /// TODO consider making it a method
        ModifyDestination ( transform.position );

        m_profile.SelectAbility ( AbilityType.DEFAULT );
        m_is_busy = false; /// TODO, use this variable to prevent multiple action
    }

    private void StabCurrentTarget ()
    {
        if ( m_current_target != null )
        {
            m_current_target.TakeDamage ( m_current_target.m_health );
        }

        if ( !disable_custom_animation )
        {
            CharacterAnimatorController.DoStab ( m_animator );
        }
        
    }

    private bool IsOnSelectionPool ()
    {
        return ( GameManager.instance.m_selection_pool.Contains ( m_char_type ) );
    }

    #region input subscribers

    public void OnMouseOverLadder ( Ladder ladder )
    {
        if ( !IsOnSelectionPool () )
        {
            return;
        }

        ///TODO, examine height to determine bottom or top point as destination
        if (Input.GetMouseButtonDown(0) && !m_is_busy)
        {
            m_current_ladder = ladder;
            ModifyDestination ( ladder.GetBottomPoint ().position );
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

        if ( Input.GetMouseButtonDown ( 0 ) && !m_is_busy )
        {
            m_current_target = null;
            m_current_ladder = null;
            ModifyDestination ( destination );
        }
    }

    public void OnMouseOverEnemy ( Enemy enemy )
    {
        if ( !IsOnSelectionPool () )
        {
            return;
        }

        if ( Input.GetMouseButtonDown ( 0 ) && !m_is_busy )
        {
            ///TODO , get if enemy is reachable or not
            m_current_target = enemy;

            //not always
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
