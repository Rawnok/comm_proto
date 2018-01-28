using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float m_pan_speed;
    public float m_pan_thickness;

    public float down_limit;
    public float up_limit;
    public float right_limit;
    public float left_limit;

    void Update ()
    {
        Vector3 current_pos = transform.position;

        if (Input.mousePosition.y >= (Screen.height - m_pan_thickness) )
        {
            current_pos.x -= m_pan_speed * Time.deltaTime;
        }

        if (Input.mousePosition.y <= m_pan_thickness)
        {
            current_pos.x += m_pan_speed * Time.deltaTime;
        }

        if ( Input.mousePosition.x >= ( Screen.width - m_pan_thickness ) )
        {
            current_pos.z += m_pan_speed * Time.deltaTime;
        }

        if ( Input.mousePosition.x <= m_pan_thickness )
        {
            current_pos.z -= m_pan_speed * Time.deltaTime;
        }

        current_pos.x = Mathf.Clamp ( current_pos.x, up_limit, down_limit ); // x top is minus
        current_pos.z = Mathf.Clamp ( current_pos.z, left_limit, right_limit );

        transform.position = current_pos;
    }
}
