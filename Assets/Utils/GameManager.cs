using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public InputRecognition input;

    public CharacterType m_current_chosen_character
    {
        get; set;
    }
    public AbilityType m_current_chosen_ability
    {
        get; set;
    }

    public CharacterProfile m_current_char_profile
    {
        get;set;
    }

    private void Awake ()
    {
        instance = this;

        LoadCharacterProfiles ();

    }//awake

    private void LoadCharacterProfiles ()
    {
        ///TODO, might have to change later
        m_current_char_profile = new GreenBaret ( initial_health: 1000, speed_multiplier: 2 );

    }//loadcharacterprofiles

    private void Start ()
    {
        m_current_chosen_character = m_current_char_profile.c_type;
        m_current_chosen_ability = AbilityType.DEFAULT; 
    }//start
}
