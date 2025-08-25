using UnityEngine;

public class EnemyGoldDropper : MonoBehaviour
{
    public GameObject coinPrefab; // 코인 프리팹
    public int minCount = 1;
    public int maxCount = 2;
    public int goldPerCoin = 1;
    public float spreadRadius = 0.4f;

    void OnDestroy()
    {
        if (Application.isPlaying == false)
        {
            return; // EnemyDropper / EnemyGoldDropper 둘 다에
        }

        if (coinPrefab == null)
        {
            return;
        }

        int count = Random.Range(minCount, maxCount + 1);
        int i = 0;
        while (i < count)
        {
            Vector2 offset = Random.insideUnitCircle * spreadRadius;
            Vector3 pos = transform.position + new Vector3(offset.x, offset.y, 0f);

            GameObject coin;
            if (PoolManager.Instance != null)
            {
                coin = PoolManager.Instance.Spawn(coinPrefab, pos, Quaternion.identity, null);
            }
            else
            {
                coin = Instantiate(coinPrefab, pos, Quaternion.identity);
            }

            GoldOrb orb = coin.GetComponent<GoldOrb>();
            if (orb != null)
            {
                orb.value = goldPerCoin;
            }

            i = i + 1;
        }
    }
}
