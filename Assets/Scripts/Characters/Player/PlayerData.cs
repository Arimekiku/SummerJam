using System;
using UnityEngine;


[CreateAssetMenu(fileName = "Player", menuName = "Characters/Player", order = 1)]
public class PlayerData : ScriptableObject
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    [SerializeField] private int maxHealth;
    [SerializeField] private float hitInvulnerableTime;

    public Vector2 CursorPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    public float MoveSpeed => moveSpeed;
    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;
    public float DashCooldown => dashCooldown;
    public float HitInvulnerableTime => hitInvulnerableTime;
    public int MaxHealth => maxHealth;
    
    public static Vector2 GetInputVector()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (inputVector.magnitude > 1) 
            inputVector = inputVector.normalized;

        return inputVector;
    }
    
    public void IncreaseMaxHealth(int amount)
    {
        if (amount < 0)
            throw new Exception("Invalid HP amount");

        maxHealth += amount;
    }
}
