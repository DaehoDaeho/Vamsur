using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemySpawner : MonoBehaviour
{
    public PrefabRegistry registry;
    public string[] ids;
    public float interval = 1.0f;
    public Transform[] spawnPos;

    float timer = 0.0f;
    int index = 0;

    // Update is called once per frame
    void Update()
    {
        timer = timer - Time.deltaTime;
        if(timer <= 0.0f)
        {
            string id = ids[index];
            int posIndex = Random.Range(0, spawnPos.Length);
            Vector3 pos = spawnPos[posIndex].position;
            registry.SpawnbyID(id, pos, Quaternion.identity, null);

            ++index;
            if(index >= ids.Length)
            {
                index = 0;
            }

            timer = interval;
        }
    }
}
