using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyDropper : MonoBehaviour
{
    public GameObject xpOrbPrefab; // XP 오브 프리팹
    public int minCount = 1;       // 최소 개수
    public int maxCount = 3;       // 최대 개수
    public int xpPerOrb = 3;       // 오브 하나당 XP
    public float spreadRadius = 0.5f; // 드랍 퍼짐 반경

    //void OnDestroy()
    public void CreateOrb()
    {
        // 씬이 종료될 때도 OnDestroy가 불려서, 에디터 Stop 시 드랍될 수 있음.
        // 그런 상황을 피하고 싶으면 조건을 하나 더 걸어도 됨.
        if (xpOrbPrefab == null)
        {
            return;
        }

        int count = Random.Range(minCount, maxCount + 1);
        int i = 0;
        while (i < count)
        {
            Vector2 offset = Random.insideUnitCircle * spreadRadius;
            Vector3 pos = transform.position + new Vector3(offset.x, offset.y, 0f);

            GameObject orb;

            // 풀 매니저가 있으면 사용(선택)
            if (PoolManager.Instance != null)
            {
                orb = PoolManager.Instance.Spawn(xpOrbPrefab, pos, Quaternion.identity, null);
                XpOrb xpOrb = orb.GetComponent<XpOrb>();
                if (xpOrb != null)
                {
                    xpOrb.SetKeyPrefab(xpOrbPrefab);
                }
            }
            else
            {
                orb = Instantiate(xpOrbPrefab, pos, Quaternion.identity);
            }

            // 오브에 XP 값 세팅
            XpOrb xo = orb.GetComponent<XpOrb>();
            if (xo != null)
            {
                xo.xpValue = xpPerOrb;
            }

            i = i + 1;
        }
    }
}
