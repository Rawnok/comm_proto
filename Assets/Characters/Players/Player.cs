using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private InputRecognition input;

    [SerializeField]
    private float m_min_move_threshold = 0.15f;

    [SerializeField]
    [Range ( 0.3f, 2.0f )]
    private float m_min_melee_attack_threshold = 1.0f;

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
        // get current abilities
        m_current_destination = transform.position;
    }//start

    private void Update ()
    {
        if ( m_current_ladder != null )
        {
            m_current_mode = PlayerMode.MELEE_ACTION;
            MoveToCurrentDestination ();
            return;
        }

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
            float chosen_threshold = ( m_current_mode != PlayerMode.MELEE_ACTION ) ? m_min_move_threshold : m_min_melee_attack_threshold;

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
                    AnimatorController.DoNomralIdle ( m_animator );
                }
            }
        }
    }//MoveToCurrentDestination

    private void MoveTowardsDestination ()
    {
        //Debug.Log ( "moving towards destination" );
        m_is_busy = false;
        transform.position = Vector3.MoveTowards ( transform.position, m_current_destination, Time.deltaTime * m_move_speed );
        AnimatorController.DoNormalWalk ( m_animator );
    }

    private bool IsLookingAt ( Vector3 destination )
    {
        if (Vector3.Distance(destination, transform.position) <= 0.1f) 
        {
            // too small to judge direction
            return true;
        }

        Vector3 direction = ( destination - transform.position ).normalized;
        float dotProduct = Vector3.Dot ( transform.forward, direction );
        return dotProduct >= m_min_turn_threshold;

    }

    private void ModifyDestination ( Vector3 destination )
    {
        m_current_destination = destination;
    }

    private void DoMeleeAction ()
    {
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
                default:
                    Debug.LogError ( "un recognized melee action detected" );
                    break;
            }
            StartCoroutine ( WaitForEndOfAction () );
        }
        else
        {
            AnimatorController.DoClimb ( m_animator );
            StartCoroutine ( WaitForEndOfAction () );

            ///TODO start coroutine to end the current action
        }
    }

    private IEnumerator WaitForEndOfAction ()
    {
        if ( m_current_ladder != null )
        {
            while ( m_current_ladder && transform.position.y <  m_current_ladder.GetTopPoint().position.y) /// TODO examine bottom point for end
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
                yield return new WaitForSeconds ( 0.2f );
            }
            AnimatorController.DoClimbEnd (m_animator);
            EndCurrentMeleeAction ();
        }
        else
        {
            yield return new WaitForSeconds ( 1 );
        }
    }

    private void EndCurrentMeleeAction ()
    {
        m_current_target = null; /// TODO consider making it a method
        m_current_ladder = null;
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
        AnimatorController.DoStab ( m_animator );
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
            if ( enemy.m_is_on_same_height )
            {
                m_current_target = enemy;
                m_current_destination = enemy.gameObject.transform.position;
            }
            else
            {
                /// TODO , let player know that enemy is not at same height
                Debug.LogError ( Messages.NOT_IN_SAME_HEIGHT );
                //on_communicaiton_event_observers ( Messages.NOT_IN_SAME_HEIGHT );
            }
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
