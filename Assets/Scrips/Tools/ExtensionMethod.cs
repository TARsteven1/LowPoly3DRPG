using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 扩展方法：
/// </summary>
public static class ExtensionMethod
{
    private const float dotThreshold = 0.5f;
    public static bool isFacingTarget(this Transform transform,Transform Target)
    {
        var vectorToTarget = Target.position - transform.position;
        vectorToTarget.Normalize();
        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        return dot>=dotThreshold;
    }
}
