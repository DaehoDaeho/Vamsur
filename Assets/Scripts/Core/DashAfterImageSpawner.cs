using UnityEngine;

public class DashAfterImageSpawner : MonoBehaviour
{
    [SerializeField]
    private DashAbility dash;

    [SerializeField]
    private SpriteRenderer sourceSprite;

    [SerializeField]
    private float spawnIntervalSeconds = 0.05f;

    [SerializeField]
    private float ghostLifetimeSeconds = 0.25f;

    private float lastSpawnTime = 0.0f;

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
        GameObject ghost = new GameObject("AfterImage");
        SpriteRenderer r = ghost.AddComponent<SpriteRenderer>();
        r.sprite = sourceSprite.sprite;
        r.flipX = sourceSprite.flipX;
        r.color = new Color(sourceSprite.color.r, sourceSprite.color.g, sourceSprite.color.b, sourceSprite.color.a);
        r.sortingLayerID = sourceSprite.sortingLayerID;
        r.sortingOrder = sourceSprite.sortingOrder - 1;

        ghost.transform.position = sourceSprite.transform.position;
        ghost.transform.rotation = sourceSprite.transform.rotation;
        ghost.transform.localScale = sourceSprite.transform.localScale;

        FadeSpriteAndDestroy f = ghost.AddComponent<FadeSpriteAndDestroy>();
        f.SetLifeTime(ghostLifetimeSeconds);
    }
}
