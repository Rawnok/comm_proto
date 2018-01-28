using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public CharacterProfile m_current_char_profile { get; set; }

    private Dictionary<CharacterType, CharacterProfile> m_all_profiles = new Dictionary<CharacterType, CharacterProfile> ();

    [SerializeField]
    private InputRecognition input;

    public List<CharacterType> m_selection_pool = new List<CharacterType> ();
    
    private void Awake ()
    {
        instance = this;
        LoadCharacterProfiles ();

        input.on_mouse_over_player_observers += OnMouseOverPlayer;
        input.on_character_select_observers += SelectCharacter;
        input.on_ability_select_observers += SelectAbility;

        ///TODO , change later
        SelectCharacter ( CharacterType.GREEN_BARET );
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
        
    }//start
    
    private void LoadCharacterProfiles ()
    {
        ///TODO, might have to change later

        m_all_profiles.Add ( CharacterType.GREEN_BARET, 
            new GreenBaret ( initial_health: 1000, speed_multiplier: 1.2f ) );

        m_all_profiles.Add ( CharacterType.SNIPER, 
            new Sniper ( initial_health: 700, speed_multiplier: 1f ) );

        m_all_profiles.Add ( CharacterType.SAPPER, 
            new Sapper ( initial_health: 850, speed_multiplier: 1f ) );
    }//loadcharacterprofiles

    public CharacterProfile GetCharacterProfile (CharacterType type)
    {
        foreach (CharacterProfile cp in m_all_profiles.Values)
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

    public void SelectAbility (AbilityType atype)
    {
        m_current_char_profile.SelectAbility ( atype );
    }

    public void SelectCharacter (CharacterType ctype)
    {
        ///TODO, handle multiple selection more accurately
        m_selection_pool.Clear (); /// TODO remove

        if (!m_selection_pool.Contains(ctype))
        {
            m_selection_pool.Add ( ctype );

            m_current_char_profile = m_all_profiles[ctype];
            SelectAbility ( AbilityType.DEFAULT );
        }
    }//select character

    public void DeselectAll ( )
    {
        m_selection_pool.Clear ();
    }//select character

}
