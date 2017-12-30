using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputRecognition : MonoBehaviour
{
    public delegate void OnMouseOverTerrain (Vector3 mouse_pos);
    public event OnMouseOverTerrain on_mouse_over_terrain_observers;

    public delegate void OnMouseOverEnemy ();///TODO , might have to pass enemy script
    public event OnMouseOverEnemy on_mouse_over_enemy_observers;

    public delegate void OnMouseOverLadder ();
    public event OnMouseOverLadder on_mouse_over_ladder_observers;
    

    public const int ENEMY_LAYER = 8;
    public const int GROUND_LAYER = 9;
    public const int LADDER_LAYER = 10;

    private const int MAX_DISTANCE_RAYCAST = 100;

    void Update ()
    {
        if ( EventSystem.current.IsPointerOverGameObject () )
        {
            // mouse position is on ui element
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );

            //set priorities here

            //is over enemy
            if ( CastOver ( ENEMY_LAYER, ray ) )
                return;

            // is over stiff object 
            if ( CastOver ( LADDER_LAYER, ray ) )
                return;

            // is over terrain, if true return.
            if ( CastOver ( GROUND_LAYER, ray ) )
                return;
        }
    }

    private bool CastOver (int layerNum, Ray ray)
    {
        RaycastHit hit;
        int layermask = 1 << (int) layerNum;
        bool hasHit = Physics.Raycast ( ray, out hit, MAX_DISTANCE_RAYCAST, layermask );
        if ( hasHit )
        {
            //set cursor, call observers
            switch ( layerNum )
            {
                case ENEMY_LAYER:
                    ///TODO set cursor
                    on_mouse_over_enemy_observers ();
                    return true;
                case GROUND_LAYER:
                    ///TODO set cursor
                    on_mouse_over_terrain_observers ( hit.point );
                    return true;
                case LADDER_LAYER:
                    ///TODO set ladder cursor
                    on_mouse_over_ladder_observers ();
                    return true;
                default:
                    Debug.LogError ("Cursor found an un identified layer : " + hit.collider.gameObject.layer);
                    return false;
            }        
        }
        return false;
    }
}
