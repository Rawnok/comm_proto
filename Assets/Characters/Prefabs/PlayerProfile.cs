using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    UNSET = -1,
    GREEN_BARET,
    SNIPER,
    MARINE,
    SAPPER
}

public enum CollectableType
{
    UNSET = -1,
    CIGARETTE,
    BARREL,
    TRAP,
    BOAT,
    BODY
}

public enum AbilityType
{
    UNSET =-1,
    DEFAULT,
    THROW,
    PISTOL,
    KNIFE,
    PUNCH,
    SNIPING,
    TRAP,
    MEDIC,
    RADIO,
    HAND_PICK
}


public class CharacterProfile
{
    private const float DEFAULT_SPEED_MULTIPLIER = 10.0f;

    //common in all
    public CharacterType c_type;
    public float health = -1;
    public float speed_multiplier = DEFAULT_SPEED_MULTIPLIER;
    public float throw_radius = 15;

    public List<CollectableType> collectables = new List<CollectableType> ();
    public List<AbilityType> abilities = new List<AbilityType> ();
    public AbilityType current_chosen_ability = AbilityType.DEFAULT;
    
    //pickup criteria is different from player to player
    public virtual void PickUp (CollectableType ctype)
    {
        Debug.Log ("pick is called from base class");
    }

    public virtual void Throw ()
    {
        Debug.Log ("player should throw current chosen item (ability) to a world position.");
    }

    public virtual void SelectAbility (AbilityType aType)
    {
        if (abilities.Contains(aType))
        {
            current_chosen_ability = aType;
        }
    }

    public virtual void DeselectAbility ()
    {
        current_chosen_ability = AbilityType.DEFAULT;
    }
    
    public CharacterProfile (CharacterType ctype, float initial_health, float speed_multiplier)
    {
        this.c_type = ctype;
        this.health = initial_health;
        this.speed_multiplier = speed_multiplier;

        // default collectables
        collectables.Clear ();
        collectables.Add ( CollectableType.CIGARETTE );

        // default abilities
        abilities.Clear ();
        abilities.Add ( AbilityType.HAND_PICK );
        abilities.Add ( AbilityType.THROW );
    }
}

public class GreenBaret : CharacterProfile
{
    public GreenBaret ( float initial_health, float speed_multiplier ) :
        base ( CharacterType.GREEN_BARET, initial_health, speed_multiplier )
    {
        //add green baret characteristics here
        // 1. collectables
        collectables.Add ( CollectableType.BARREL );
        collectables.Add ( CollectableType.BODY );

        // 2. abilities
        abilities.Add ( AbilityType.PUNCH );
        abilities.Add ( AbilityType.KNIFE );
        abilities.Add ( AbilityType.RADIO );
    }

    public override void PickUp (CollectableType ctype)
    {
        Debug.Log ( "Green baret requested to pickup " + ctype );
        //first check  to see if character can pickup ctype
    }

}
