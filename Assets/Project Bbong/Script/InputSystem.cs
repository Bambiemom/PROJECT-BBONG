using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : SingletonBase<InputSystem>
{
    public Vector2 moveInput;
    public Vector2 look;
    public bool isStrafe;
    public bool isWalk;
    public bool isAim;
    public bool isFire;
    public bool isDash;
    public bool isCrouching;
    public bool isAttack;
    public bool isGuard;
    public delegate void OnJumpCallback();
    public OnJumpCallback onJumpCallback;
    public System.Action onAttack;
    public System.Action onInteract;
    public System.Action onFire;
    


    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isDash = true;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            onJumpCallback();
        }
        if (Input.GetKey(KeyCode.Q))
        {
            isAttack = true;
        }
        if (Input.GetKey(KeyCode.B))
        {
            onInteract?.Invoke();
        }
        if (Input.GetMouseButtonDown(0))
        {
            isFire = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isFire = false;
        }
        if (Input.GetMouseButton(1))
        {

        }
        if (Input.GetKey(KeyCode.F))
        {
            isGuard = true;
        }


    }
}
