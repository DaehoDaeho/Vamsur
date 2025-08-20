using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    [SerializeField]private CharacterStatsSO stats;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float baseMoveSpeed;    // 기본 속도.
    private float movespeedMultiplier;  // 합성 배율.(1 = 변화 없음)
    public float currentSpeedMagnitude
    {
        get;
        private set;
    }

    private int currentHP;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHP = stats.baseMaxHP;
        baseMoveSpeed = stats ? stats.baseMoveSpeed : 5.0f;
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
        float effective = baseMoveSpeed * movespeedMultiplier;
        rb.velocity = moveInput * effective;
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void AddMoveSpeedMultiplier(float addPercent)
    {
        float m = 1.0f + Mathf.Max(-0.9f, addPercent / 100.0f);
        movespeedMultiplier *= m;
    }
}
