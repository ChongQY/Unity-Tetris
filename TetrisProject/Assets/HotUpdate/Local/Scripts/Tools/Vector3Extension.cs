using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extension
{
    public static Vector2 Round(this Vector3 v) {
        int x = Mathf.RoundToInt(v.x);
        int y = Mathf.RoundToInt(v.y);
        return new Vector2(x, y);
    }

    /// <summary>
    /// 获取当前物体相对于指定祖先物体的坐标
    /// </summary>
    /// <param name="currentTrans">当前物体的Transform</param>
    /// <param name="ancestorTrans">祖先物体的Transform</param>
    /// <returns>相对于祖先的坐标</returns>
    public static Vector3 GetRelativePositionToAncestor(this Transform currentTrans, Transform ancestorTrans) {
        if (ancestorTrans == null) {
            Debug.LogError("祖先物体不能为null！");
            return Vector3.zero;
        }
        return ancestorTrans.InverseTransformPoint(currentTrans.position);
    }

    /// <summary>
    /// 设置当前物体相对于指定祖先物体的坐标
    /// </summary>
    /// <param name="currentTrans">当前物体的Transform</param>
    /// <param name="ancestorTrans">祖先物体的Transform</param>
    /// <param name="targetRelativePos">想要相对于祖先的坐标</param>
    public static void SetRelativePositionToAncestor(this Transform currentTrans, Transform ancestorTrans, Vector3 targetRelativePos) {
        if (ancestorTrans == null) {
            Debug.LogError("祖先物体不能为null！");
            return;
        }
        currentTrans.position = ancestorTrans.TransformPoint(targetRelativePos);
    }
}
