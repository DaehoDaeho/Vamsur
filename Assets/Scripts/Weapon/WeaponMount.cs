using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMount : MonoBehaviour
{
    public WeaponDefinitionSO weapon;
    public Transform mountPoint;

    private GameObject SpawnedVisual;

    private void OnEnable()
    {
        ApplyWeapon(weapon);
    }

    public void ApplyWeapon(WeaponDefinitionSO def)
    {
        weapon = def;
        if(mountPoint == null)
        {
            return;
        }

        if(SpawnedVisual != null)
        {
            Destroy(SpawnedVisual);
        }

        if(weapon != null && weapon.visualPrefab != null)
        {
            SpawnedVisual = Instantiate(weapon.visualPrefab, mountPoint);
            SpawnedVisual.transform.localPosition = Vector3.zero;
            SpawnedVisual.transform.localRotation = Quaternion.identity;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
