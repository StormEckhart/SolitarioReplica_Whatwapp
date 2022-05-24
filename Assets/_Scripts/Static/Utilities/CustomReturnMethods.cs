using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomReturnMethods
{
    //Returns a world position from a given screen position (3D or 2D)
    public static Vector3 ScreenToWorld3D(Camera i_UsedCamera, Vector3 i_Position, float i_RayDistance)
    {
        return i_UsedCamera.ScreenPointToRay(i_Position).GetPoint(i_RayDistance);
    }
    public static Vector3 ScreenToWorld2D(Camera i_UsedCamera, Vector3 i_Position)
    {
        i_Position.z = Camera.main.nearClipPlane;

        return i_UsedCamera.ScreenToWorldPoint(i_Position);
    }


    //Returns the closest value of two from a given value
    public static float ReturnClosestValue(float i_Value, float i_A, float i_B)
    {
        if (Mathf.Abs(i_A - i_Value) >= Mathf.Abs(i_B - i_Value))
        {
            return Mathf.Abs(i_B);
        }
        else
        {
            return Mathf.Abs(i_A);
        }
    }
}
