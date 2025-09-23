using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingAndSplitTest : MonoBehaviour
{
    public GameObject homingPrefab;
    public GameObject splitPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H) == true)
        {
            GameObject go = Instantiate(homingPrefab, transform.position, Quaternion.identity);
            HomingBullet hb = go.GetComponent<HomingBullet>();
            hb.Fire(transform.position, Vector2.right);
        }

        if (Input.GetKeyDown(KeyCode.J) == true)
        {
            GameObject go = Instantiate(splitPrefab, transform.position, Quaternion.identity);
            SplitBullet sb = go.GetComponent<SplitBullet>();
            sb.Fire(transform.position, Vector2.right);
        }
    }
}
