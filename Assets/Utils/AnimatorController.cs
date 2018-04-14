using UnityEngine;

public class CharacterAnimatorController
{
    private const string MOVEMENT_KEY = "movement";
    private const string CLIMB_KEY = "climb";
    private const string SHOOT_KEY = "shoot";
    private const string IDLE_KEY = "idle";
    private const string STANDING_KEY = "is_standing";
    private const string STAB_TRIGGER = "stab_trigger";
    private const string SHOOT_TRIGGER = "shoot_trigger";
    private const string DEATH_TRIGGER = "death_trigger";


    private const float NORMAL_IDLE_VALUE = 0.0f;
    private const float PRONE_IDLE_VALUE = 1.0f;
    private const float PISTOL_SHOOTING_IDLE_VALUE = 2.0f;
    private const float FIRING_RIFLE_IDLE_VALUE = 3.0f;


    private const float WALK_VALUE = 1.0f;
    private const float RUN_VALUE = 2.0f;

    private const float CLIMB_VALUE = 1.0f;
    private const float CLIMB_END_VALUE = 2.0f;

    
    public static void DoNormalWalk ( Animator animator )
    {
        //Debug.Log ("Doing normal walk");
        animator.SetFloat ( MOVEMENT_KEY, WALK_VALUE );
        animator.SetFloat ( IDLE_KEY, 0 );
    }

    public static void DoNomralIdle ( Animator animator )
    {
        //Debug.Log ("Doing normal ide");
        animator.SetFloat ( IDLE_KEY, NORMAL_IDLE_VALUE );
        animator.SetFloat ( MOVEMENT_KEY, 0 );
        animator.SetFloat ( CLIMB_KEY, 0 );
    }

    public static void DoStab ( Animator animator )
    {
        DoNomralIdle ( animator );
        animator.SetTrigger ( STAB_TRIGGER );
    }

    public static void DoDeath ( Animator animator )
    {
        DoNomralIdle ( animator );
        animator.SetTrigger ( DEATH_TRIGGER );
    }

    public static void DoShoot (Animator animator)
    {
        DoNomralIdle (animator);
        animator.SetTrigger ( SHOOT_TRIGGER );
    }

    public static void DoClimb (Animator animator)
    {
        //Debug.Log ("climbing ");
        DoNomralIdle (animator);
        animator.SetFloat ( CLIMB_KEY, CLIMB_VALUE );
    }

    public static void DoClimbEnd ( Animator animator )
    {
        //Debug.Log ("climb ending");
        animator.SetFloat ( CLIMB_KEY, CLIMB_END_VALUE );
    }
    
}