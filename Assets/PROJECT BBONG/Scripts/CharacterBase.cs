using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterBase : MonoBehaviour
{
    public bool IsAiming { get; set; }
    public bool IsGrounded => IsGrounded;

    public bool IsStrafe;
    public bool IsWalk
    {
        get => isWalk;
        set => isWalk = value;

    }
    public float moveSpeed;
    public float rotationSpeed;
    public float jumpForce;

    public float groundRadius = 0.1f;
    public float groundOffset = 0.1f;
    public LayerMask groundLayer;

    protected float maxJumpCount = 2;
    protected private int currentJumpCount = 0;
    protected private float verticalVelocity; //수직속도
    protected bool isGrounded;
    protected float jumpTimeout = 0.1f;
    protected float jumpTimeoutDelta = 0f;

    protected private bool isWalk = false;
    protected private bool isStrafe = false;
    protected private float speed = 0f;
    protected private float targetSpeed = 0f;
    protected private float targetSpeedBlend = 0f;

    protected private bool isPossibleMovement = false;
    protected private bool isPossibleAttack = false;
    protected bool isAttacking = false;
    protected Animator characterAnimator;

    public bool IsPossibleMovement
    {
        get => isPossibleMovement;
        set => isPossibleMovement = value;
    }
    public void Jump()
    {
        if (currentJumpCount >= maxJumpCount)
            return;
        verticalVelocity = jumpForce;
        jumpTimeoutDelta = jumpTimeout;
        currentJumpCount++;

        characterAnimator.SetTrigger("JumpTrigger");


    }


    
}
