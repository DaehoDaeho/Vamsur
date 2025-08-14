using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class KnockbackReceiver : MonoBehaviour
{
    public float damping = 12.0f;   // ∞®ºË ¡§µµ.(≈¨ºˆ∑œ ª°∏Æ ∏ÿ√„)
    public float maxSpeed = 12.0f;  // √÷¥Î º”µµ.

    private Rigidbody2D rb;
    private Vector2 externalVelocity;   // ≥ÀπÈ º”µµ.

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 force)
    {
        externalVelocity = Vector2.ClampMagnitude(externalVelocity + force, maxSpeed);
    }

    private void FixedUpdate()
    {
        rb.velocity += externalVelocity;
        externalVelocity = Vector2.MoveTowards(externalVelocity, Vector2.zero, damping * Time.fixedDeltaTime);
    }
}
