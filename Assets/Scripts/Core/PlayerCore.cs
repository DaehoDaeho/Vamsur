using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    [SerializeField]private CharacterStatsSO stats;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    public float currentSpeedMagnitude
    {
        get;
        private set;
    }

    private float moveSpeed;
    private int currentHP;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        moveSpeed = stats.baseMoveSpeed;
        currentHP = stats.baseMaxHP;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentSpeedMagnitude = rb.velocity.magnitude;
    }

    private void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }
}
