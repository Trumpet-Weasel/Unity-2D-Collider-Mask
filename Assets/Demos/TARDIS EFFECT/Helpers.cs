using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A helper class for copying/pasting the points of a polygon collider.
public static class Helpers
{
    public static void CopyPoints(PolygonCollider2D source, PolygonCollider2D target)
    {
        if (source != null && target != null)
        {
            target.pathCount = source.pathCount;
            for (int i = 0; i < source.pathCount; i++)
            {
                target.SetPath(i, source.GetPath(i));
            }
        }
    }
}
