using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorType
{
    DEFAULT = 0,
    KNIFE,
    PISTOL,
    LADDER,
}


[System.Serializable]
public class CursorPack
{
    public CursorType type;
    public Texture2D image;
    public Vector2 offset;
}


public class GameCursor : MonoBehaviour
{
    [SerializeField]
    private CursorPack[] cursor_pack;
    
    
}
