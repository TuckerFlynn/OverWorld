using UnityEngine;

public static class Vector3Extension
{
    public static Vector2Int ToInt2(this Vector3 v)
    {
        return new Vector2Int((int)v.x, (int)v.y);
    }
}
