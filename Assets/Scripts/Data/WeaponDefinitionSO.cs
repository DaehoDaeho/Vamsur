using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Data/Weapon Definition", fileName = "WeaponDefinition")]
public class WeaponDefinitionSO : ScriptableObject
{
    public string weaponName = "Sword";
    public int baseDamage = 10;
    public float attackInterval = 0.5f;
    public GameObject visualPrefab;
}
