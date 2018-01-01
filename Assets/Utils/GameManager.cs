using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public CharacterProfile m_current_char_profile { get; set; }

    private List<CharacterProfile> m_all_profiles = new List<CharacterProfile>(); // used by player.cs to initiate values base on profiles

    [SerializeField]
    private InputRecognition input;

    public List<CharacterType> m_selection_pool = new List<CharacterType> ();
    
    private void Awake ()
    {
        instance = this;

        LoadCharacterProfiles ();
    }//awake

    private void Update ()
    {
        if (Input.GetMouseButtonDown(1))
        {
            DeselectAll ();
        }
    }


    private void Start ()
    {
        input.on_mouse_over_player_observers += OnMouseOverPlayer;
        
        ///TODO , change later
        SelectCharacter ( CharacterType.GREEN_BARET );
        m_current_char_profile.SelectAbility ( AbilityType.DEFAULT );
    }//start
    
    private void LoadCharacterProfiles ()
    {
        ///TODO, might have to change later
        m_current_char_profile = new GreenBaret ( initial_health: 1000, speed_multiplier: 2 );
        m_all_profiles.Add ( m_current_char_profile );
    }//loadcharacterprofiles

    public CharacterProfile GetCharacterProfile (CharacterType type)
    {
        foreach (CharacterProfile cp in m_all_profiles)
        {
            if (cp.c_type == type)
            {
                return cp;
            }
        }
        return null;
    }

    public void OnMouseOverPlayer (CharacterType ctype)
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectCharacter ( ctype );
        }
    }

    public void SelectCharacter (CharacterType ctype)
    {
        ///TODO, handle multiple selection more accurately
        m_selection_pool.Clear (); /// TODO remove

        if (!m_selection_pool.Contains(ctype))
        {
            m_selection_pool.Add ( ctype );
        }
    }//select character

    public void DeselectAll ( )
    {
        m_selection_pool.Clear ();
    }//select character

}
