using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class GravityMenu
{
    private static Vector3 earthGravity = Vector3.up * -9.81f;
    private static Vector3 moonGravity = Vector3.up * -1.62f;

    public static void SetEarthGravity()
    {
        Physics.gravity = earthGravity;
    }

    public static void SetMoonGravity()
    {
        Physics.gravity = moonGravity;
    }

#if UNITY_EDITOR
    [MenuItem("Gravity/Earth")]
    static void Earth()
    {
        SetEarthGravity();
    }

    [MenuItem("Gravity/Earth", true)]
    static bool VaridateEarth()
    {
        var menuPath = "Gravity/Earth";
        Menu.SetChecked(menuPath, Physics.gravity == earthGravity);
        return true;
    }

    [MenuItem("Gravity/Moon")]
    static void Moon()
    {
        SetMoonGravity();
    }

    [MenuItem("Gravity/Moon", true)]
    static bool VaridateMoon()
    {
        var menuPath = "Gravity/Moon";
        Menu.SetChecked(menuPath, Physics.gravity == moonGravity);
        return true;
    }
#endif

}