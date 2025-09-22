using UnityEditor.Rendering;
using UnityEngine;

public class ProjectileStandard : MonoBehaviour
{
    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    private float lifeSeconds = 3.0f;

    [SerializeField]
    private float damage = 10.0f;

    [SerializeField]
    private int pierceCount = 0;

    private Vector2 moveDirection = Vector2.right;
    private float spawnTime = 0.0f;

    private void OnEnable()
    {
        spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = new Vector3(moveDirection.x, moveDirection.y, 0.0f) * speed * Time.deltaTime;
        transform.position += delta;

        if(Time.time - spawnTime >= lifeSeconds)
        {
            Destroy(gameObject);
        }
    }

    public void Setup(Vector2 directionUnit, float dmg, float speedUnitsPerSec, float lifeSec, int pierce)
    {
        moveDirection = directionUnit.normalized;
        damage = dmg;
        speed = speedUnitsPerSec;
        lifeSeconds = lifeSec;
        pierceCount = pierce;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy") == true)
        {
            Health enemy = collision.GetComponent<Health>();
            if(enemy != null)
            {
                enemy.TakeDamage((int)damage, enemy.transform.position);

                if(pierceCount > 0)
                {
                    --pierceCount;
                }
                else
                {
                    Destroy(gameObject);
                }

                return;
            }
        }
    }
}
