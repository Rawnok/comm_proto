using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorType
{
    DEFAULT = 0,
    KNIFE,
    HAND,
    PISTOL,
    LADDER,
}


[System.Serializable]
public class CursorPack
{
    public CursorType type;
    public Texture2D image;
    public Vector2 offset = new Vector2(64,64); // default
}


public class GameCursor : MonoBehaviour
{
    public static GameCursor instance;

    [SerializeField]
    private InputRecognition input;
    
    [SerializeField]
    private CursorPack[] cursor_pack;

    private void Awake ()
    {
        instance = this;
    }
    
    private void Start ()
    {
        input.on_mouse_over_collectable_observers += OnMouseOverCollectables;
        input.on_mouse_over_enemy_observers += OnMouseOverEnemy;
        input.on_mouse_over_ladder_observers += OnMouseOverLadder;
        input.on_mouse_over_terrain_observers += OnMouseOverTerrain;
        input.on_character_select_observers += OnCharacterSelect;
    }

    private void OnCharacterSelect ( CharacterType ctype )
    {
        CursorPack default_cursor = GetCursorOfType ( CursorType.DEFAULT );
        Cursor.SetCursor ( default_cursor.image, default_cursor.offset, CursorMode.Auto );
    }

    private void OnMouseOverCollectables (Collectables collectable)
    {
        // based on chosen character profile we have to deciede which cursor to show
        CharacterProfile current_profile = GameManager.instance.m_current_char_profile;
        if ( current_profile.collectables.Contains ( collectable.m_current_type ) )
        {
            // character is allowed to pick it up
            CursorPack hand_cursor = GetCursorOfType (CursorType.HAND);
            if ( hand_cursor != null )
            {
                Cursor.SetCursor ( hand_cursor.image, hand_cursor.offset, CursorMode.Auto );
            }
        }
    }

    private CursorPack GetCursorOfType (CursorType cursor_type)
    {
        foreach ( CursorPack cp in cursor_pack )
        {
            if ( cp.type == cursor_type )
                return cp;
        }
        return null;
    }

    private void OnMouseOverTerrain ( Vector3 mouse_pos )
    {
        // base on current chosen ability, we will decide cursor
        AbilityType current_ability_type = GameManager.instance.m_current_char_profile.current_chosen_ability;

        switch ( current_ability_type )
        {
            case AbilityType.UNSET:
            case AbilityType.DEFAULT:
                CursorPack default_cursor = GetCursorOfType ( CursorType.DEFAULT );
                if ( default_cursor != null )
                {
                    Cursor.SetCursor ( default_cursor.image, default_cursor.offset, CursorMode.Auto );
                }
                break;
            case AbilityType.KNIFE:
                CursorPack knife_cursor = GetCursorOfType ( CursorType.KNIFE );
                Cursor.SetCursor ( knife_cursor.image, knife_cursor.offset, CursorMode.Auto );
                break;
            case AbilityType.HAND_PICK:
                CursorPack hand_cursor = GetCursorOfType ( CursorType.HAND );
                Cursor.SetCursor ( hand_cursor.image, hand_cursor.offset, CursorMode.Auto );
                break;
            case AbilityType.THROW:
                break;
            case AbilityType.PISTOL:
                break;
            case AbilityType.PUNCH:
                break;
            case AbilityType.SNIPING:
                break;
            case AbilityType.TRAP:
                break;
            case AbilityType.MEDIC:
                break;
            case AbilityType.RADIO:
                break;
            default:
                break;
        }
    }

    private void OnMouseOverLadder (Ladder ladder)
    {
        // choose ladder cursor
        CursorPack ladder_cursor = GetCursorOfType ( CursorType.LADDER );
        if ( ladder_cursor != null )
        {
            Cursor.SetCursor ( ladder_cursor.image, ladder_cursor.offset, CursorMode.Auto );
        }
    }

    private void OnMouseOverEnemy (Enemy enemy)
    {
        ///TODO 
        // based on chosen ability && enemy char, cursor will change
    }
}
