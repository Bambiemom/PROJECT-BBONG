using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public bool isStrafe;
    public bool isWalk;
    public bool isAim;
    public bool isFire;
    public bool isDash;
    public bool isCrouching;
    public delegate void OnJumpCallback();
    public OnJumpCallback onJumpCallback;

    

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
    }
}
