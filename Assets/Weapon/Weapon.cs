using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    MELEE = 0,
    RANGED,
}

[CreateAssetMenu(menuName ="Components/New Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] string m_name;
    [SerializeField] WeaponType m_w_type;
    [SerializeField] GameObject m_prefab; // model, prefab
    [SerializeField] Transform m_grip; // the position it is held on


    public string GetName ()
    {
        return m_name;
    }

    public WeaponType GetWeaponType ()
    {
        return m_w_type;
    }

    public GameObject GetWeaponPrefab ()
    {
        return m_prefab;
    }

    public Transform GetGripTransform ()
    {
        return m_grip;
    }
}
