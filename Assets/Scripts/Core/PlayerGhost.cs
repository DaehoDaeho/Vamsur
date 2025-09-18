using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhost : MonoBehaviour
{
    public PlayerDash dash;
    public SpriteRenderer sourceSprite;
    public float spawnIntervalSeconds = 0.05f;
    public float ghostLifetimeSeconds = 0.2f;

    private float lastSpawnTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(dash.GetIsDashing() == true)
        {
            if(Time.time - lastSpawnTime >= spawnIntervalSeconds)
            {
                SpawnGhost();
                lastSpawnTime = Time.time;
            }
        }
    }

    void SpawnGhost()
    {
        GameObject ghost = new GameObject("PlayerGhost");
        SpriteRenderer sr = ghost.AddComponent<SpriteRenderer>();
        sr.sprite = sourceSprite.sprite;
        sr.flipX = sourceSprite.flipX;
        sr.color = new Color(sourceSprite.color.r, sourceSprite.color.g, sourceSprite.color.b, sourceSprite.color.a);
        sr.sortingLayerID = sourceSprite.sortingLayerID;
        sr.sortingOrder = sourceSprite.sortingOrder - 1;

        ghost.transform.position = sourceSprite.transform.position;
        ghost.transform.rotation = sourceSprite.transform.rotation;
        ghost.transform.localScale = sourceSprite.transform.localScale;

        PlayerGhostDestroyer pgd = ghost.AddComponent<PlayerGhostDestroyer>();
        pgd.SetLifetime(ghostLifetimeSeconds);
    }
}
