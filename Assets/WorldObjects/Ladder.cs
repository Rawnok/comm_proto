using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour {

    [SerializeField]
    private Transform m_bottom_point;

    [SerializeField]
    private Transform m_top_point;

    [SerializeField]
    private Transform m_bottom_land_point;

    [SerializeField]
    private Transform m_top_land_point;

    public Transform GetTopPoint ()
    {
        return m_top_point;
    }

    public Transform GetBottomPoint ()
    {
        return m_bottom_point;
    }

    public Transform GetLandingSpace (bool isTop = true)
    {
        return isTop ? m_top_land_point : m_bottom_land_point;
    }
}
