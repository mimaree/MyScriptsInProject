using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    public static Vector2 JoyStickTouchMove; // To tu jest tylko narazie
    public static GameObject target;
    public static Transform CameraLockTransformPosition;
    [Range(0, 1)]
    public static float analogStrength;
    public static float MainHorizontal()
    {
        float r = 0.0f;
        r += -Input.GetAxis("k_MainHorizontal");
        r += -Input.GetAxis("J_MainHorizontal");
        r += -JoyStickTouchMove.x;
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static float MainVertical()
    {
        float r = 0.0f;
        r += -Input.GetAxis("K_MainVertical");
        r += Input.GetAxis("J_MainVertical");
        r += -JoyStickTouchMove.y;
        return Mathf.Clamp(r, -1.0f, 1.0f);   
    }

    public static Vector3 MainJoystick()
    {
        return new Vector3(MainHorizontal(), MainVertical(), 0 );
    }

    public static float Get_Right_Stick_Horizontal()
    {
        float r = 0.0f;
        r += Input.GetAxis("Right_Stick_Horizontal");
        r += Input.GetAxis("Mouse X");
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static float Get_Right_Stick_Vertical()
    {
        float r = 0.0f;
        r += Input.GetAxis("Right_Stick_Vertical");
        r += Input.GetAxis("Mouse Y");
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static Vector3 Get_Right_Stck_Vector3()
    {
        return new Vector3(Get_Right_Stick_Horizontal(), 0, Get_Right_Stick_Vertical());
    }

    public static bool AButton()
   {
        return Input.GetButton("A_Button");
   }

    public static bool BButton()
    {
        return Input.GetButton("B_Button");
    }

    public static bool XButton()
    {
        return Input.GetButton("X_Button");
    }

    public static bool YButton()
    {
        return Input.GetButton("Y_Button");
    }

    public static bool JumpButton()
    {
        return Input.GetButton("SpaceK") || Input.GetButton("A_Button");
    }

    public static bool RagdollButtom()
    {
        return YButton() || Input.GetKey("y");
    }
}
