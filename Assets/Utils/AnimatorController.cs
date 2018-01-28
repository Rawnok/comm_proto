using UnityEngine;

public class AnimatorController
{
    public const string MOVEMENT_KEY = "movement";
    public const string CLIMB_KEY = "climb";
    public const string SHOOT_KEY = "shoot";
    public const string IDLE_KEY = "idle";
    public const string STANDING_KEY = "is_standing";
    public const string STAB_TRIGGER = "stab_trigger";
    public const string SHOOT_TRIGGER = "shoot_trigger";
    public const string DEATH_TRIGGER = "death_trigger";


    public const float NORMAL_IDLE_VALUE = 0.0f;
    public const float PRONE_IDLE_VALUE = 1.0f;
    public const float PISTOL_SHOOTING_IDLE_VALUE = 2.0f;
    public const float FIRING_RIFLE_IDLE_VALUE = 3.0f;


    public const float WALK_VALUE = 1.0f;
    public const float RUN_VALUE = 2.0f;

    public const float CLIMB_VALUE = 1.0f;
    public const float CLIMB_END_VALUE = 2.0f;


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

    public static void DoClimb (Animator animator)
    {
        DoNomralIdle (animator);
        animator.SetFloat ( CLIMB_KEY, CLIMB_VALUE );
    }

    public static void DoClimbEnd ( Animator animator )
    {
        animator.SetFloat ( CLIMB_KEY, CLIMB_END_VALUE );
    }

}