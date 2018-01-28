using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour {

    [SerializeField]
    private Transform m_bottom_point;

    [SerializeField]
    private Transform m_top_point;

    public Transform GetTopPoint ()
    {
        return m_top_point;
    }

    public Transform GetBottomPoint ()
    {
        return m_bottom_point;
    }
}
