using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTest : MonoBehaviour
{
    public Lightning lightning;
    public BulletShooter bulletShooter;
    public FireRing fireRing;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L) == true)
        {
            lightning.StrikeAt(transform.position);
        }

        if (Input.GetKeyDown(KeyCode.K) == true)
        {
            bulletShooter.Fire(20);
        }

        if (Input.GetKeyDown(KeyCode.P) == true)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            fireRing.Play(pos);
        }
    }
}
