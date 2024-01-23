using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static Vector3 GetRectWorldPosition(RectTransform rectTransform)
    {
        // Get the center point of the RectTransform in local space
        Vector2 pivot = rectTransform.pivot;
        Vector3 centerLocal = new Vector3((0.5f - pivot.x) * rectTransform.rect.width, (0.5f - pivot.y) * rectTransform.rect.height, 0f);

        // Transform the local center point to world space
        Vector3 worldPosition = rectTransform.TransformPoint(centerLocal);

        return worldPosition;
    }
}
