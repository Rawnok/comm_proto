using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{
    [SerializeField]
    private Text character_text;

    [SerializeField]
    private Text ability_text;
    
    void Update ()
    {
        if (GameManager.instance.m_current_char_profile != null)
        {
            character_text.text = string.Format ( "Character : {0}", GameManager.instance.m_current_char_profile.c_type );
            ability_text.text = string.Format ( "Ability : {0}", GameManager.instance.m_current_char_profile.current_chosen_ability );
        }
    }
}
