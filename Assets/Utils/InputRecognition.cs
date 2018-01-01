using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputRecognition : MonoBehaviour
{
    public delegate void OnMouseOverTerrain (Vector3 mouse_pos);
    public event OnMouseOverTerrain on_mouse_over_terrain_observers;

    public delegate void OnMouseOverEnemy (Enemy enemy);
    public event OnMouseOverEnemy on_mouse_over_enemy_observers;

    public delegate void OnMouseOverCollectable (Collectables collectable_object);
    public event OnMouseOverCollectable on_mouse_over_collectable_observers;

    public delegate void OnMouseOverLadder (Ladder ladder);
    public event OnMouseOverLadder on_mouse_over_ladder_observers;

    public delegate void OnMouseOverPlayer (CharacterType player_type);
    public event OnMouseOverPlayer on_mouse_over_player_observers;
    

    public const int ENEMY_LAYER = 8;
    public const int WALKABLE_LAYER = 9;
    public const int LADDER_LAYER = 10;
    public const int COLLECTABLE_LAYER = 11;
    public const int PLAYER_LAYER = 12;

    private const int MAX_DISTANCE_RAYCAST = 100;

    private int last_saved_layer = -1;

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
            if ( CastOver ( PLAYER_LAYER, ray ) )
            {
                //Debug.Log ("Player layer found");
                return;
            }


            //is over enemy
            if ( CastOver ( ENEMY_LAYER, ray ) )
            {
                //Debug.Log ( "enemy layer found" );
                return;
            }


            if ( CastOver ( COLLECTABLE_LAYER, ray ) )
            {
                //Debug.Log ( "collectable layer found" );
                return;
            }


            // is over stiff object 
            if ( CastOver ( LADDER_LAYER, ray ) )
            {
                //Debug.Log ( "ladder layer found" );
                return;
            }
                

            if ( CastOver ( WALKABLE_LAYER, ray ) )
            {
                //Debug.Log ( "walkable layer found" );
                return;
            }

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

            /*
            if ( last_saved_layer == layerNum )
            {
                return true;
            }
            last_saved_layer = layerNum;
            */
            //Debug.Log ("current layer number " + layerNum);

            switch ( layerNum )
            {
                case PLAYER_LAYER:
                    {
                        Player player = hit.collider.gameObject.transform.root.GetComponent<Player> ();
                        if ( player != null )
                        {
                            on_mouse_over_player_observers ( player.m_char_type );
                        }
                        else
                        {
                            Debug.LogError ("Player layer found, but not player script");
                        }
                        return true;
                    }
                case ENEMY_LAYER:
                    {
                        Enemy enemy = hit.collider.gameObject.GetComponent<Enemy> ();
                        if ( enemy != null )
                        {
                            on_mouse_over_enemy_observers ( enemy );
                        }
                        else
                        {
                            Debug.LogError ( "enemy script not found on enemy layer" );
                        }
                        return true;
                    }
                case COLLECTABLE_LAYER:
                    {
                        Collectables collectable_script = hit.collider.gameObject.GetComponent<Collectables> ();
                        if ( collectable_script != null )
                        {
                            on_mouse_over_collectable_observers ( collectable_script );
                        }
                        else
                        {
                            Debug.LogError ( "Collectable script not found on collectable layer" );
                        }
                        return true;
                    }
                case LADDER_LAYER:
                    {
                        Ladder ladder_script = hit.collider.gameObject.GetComponent<Ladder> ();
                        if ( ladder_script != null )
                        {
                            on_mouse_over_ladder_observers ( ladder_script );
                        }
                        else
                        {
                            Debug.LogError ( "Ladder script not found on Ladder layer" );
                        }
                        return true;
                    }
                case WALKABLE_LAYER:
                    {
                        on_mouse_over_terrain_observers ( hit.point );
                        return true;
                    }
                default:
                    Debug.LogError ("unidentified layer : " + hit.collider.gameObject.layer);
                    return false;
            }        
        }

        return false;
    }
}
