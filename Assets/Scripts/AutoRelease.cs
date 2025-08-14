using System.Collections;
using UnityEngine;

/// <summary>
/// lifeTime 후 풀로 자동 반환.
/// - prefabKey: 어떤 풀에 돌려줄지(원본 프리팹)
/// </summary>
public class AutoRelease : MonoBehaviour
{
    private GameObject keyPrefab;
    private float life;

    public void Init(GameObject prefabKey, float lifeTime)
    {
        keyPrefab = prefabKey;
        life = Mathf.Max(0.05f, lifeTime);
        StopAllCoroutines();
        StartCoroutine(CoRelease());
    }

    private IEnumerator CoRelease()
    {
        yield return new WaitForSeconds(life);
        if (PoolManager.Instance != null && keyPrefab != null)
        {
            PoolManager.Instance.Release(keyPrefab, gameObject);
        }            
        else
        {
            Destroy(gameObject);
        }   
    }
}
