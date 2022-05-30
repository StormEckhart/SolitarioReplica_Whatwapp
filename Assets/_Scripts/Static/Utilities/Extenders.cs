using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extenders
{
    #region Transforms

    //Breadth-first search
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        if (aParent != null)
        {
            var result = aParent.Find(aName);
            if (result != null)
                return result;

            foreach (Transform child in aParent)
            {
                result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
        }

        return null;
    }

    public static T FindDeepChild<T>(this Transform aParent, string aName)
    {
        T result = default(T);

        var transform = aParent.FindDeepChild(aName);

        if (transform != null)
        {
            result = (typeof(T) == typeof(GameObject)) ? (T)Convert.ChangeType(transform.gameObject, typeof(T)) : transform.GetComponent<T>();
        }

        if (result == null)
        {
            Debug.LogError($"FindDeepChild didn't find: '{aName}' on GameObject: '{aParent.name}'");
        }

        return result;
    }

    #endregion

    #region Collections

    public static T GetLoop<T>(this IEnumerable<T> i_Array, int i_Index)
    {
        int len = i_Array.Count();
        if (i_Index >= 0)
        {
            return i_Array.ElementAt(i_Index % i_Array.Count());
        }
        else
        {
            i_Index = -i_Index;
            i_Index %= len;
            i_Index = len - i_Index;
            return i_Array.ElementAt(i_Index % len);
        }
    }

    #endregion

    #region Lists

    //Shuffling the array using the Fisher-Yates algorithm
    public static void Shuffle<T>(this List<T> i_List)
    {
        System.Random l_Random = new System.Random();

        int l_N = i_List.Count;

        while (l_N > 1)
        {
            int l_K = l_Random.Next(l_N--);
            T temp = i_List[l_N];
            i_List[l_N] = i_List[l_K];
            i_List[l_K] = temp;
        }
    }

    #endregion

    #region Arrays

    //Shuffling the array using the Fisher-Yates algorithm
    public static void Shuffle<T>(this T[] i_Array)
    {
        System.Random l_Random = new System.Random();

        int l_N = i_Array.Length;

        while (l_N > 1)
        {
            int l_K = l_Random.Next(l_N--);
            T temp = i_Array[l_N];
            i_Array[l_N] = i_Array[l_K];
            i_Array[l_K] = temp;
        }
    }

    #endregion

    #region Vector3s

    public static bool IsGreaterOrEqual(this Vector3 local, Vector3 other)
    {
        if (local.x >= other.x && local.y >= other.y && local.z >= other.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsLesserOrEqual(this Vector3 local, Vector3 other)
    {
        if (local.x <= other.x && local.y <= other.y && local.z <= other.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}
