using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float m_health = 100;
    public bool m_is_alive = true;

    private Animator m_animator;

    void Start ()
    {
        m_animator = gameObject.GetComponentInChildren<Animator> ();
    }

    void Die ()
    {
        if (m_is_alive)
        {
            CharacterAnimatorController.DoDeath ( m_animator );
            m_is_alive = false;
        }
    }

    public void TakeDamage (float damage_amount)
    {
        if (m_is_alive)
        {
            m_health -= damage_amount;

            if (m_health <= 0)
            {
                Die ();
            }
        }
    }
}
