using UnityEngine;
using System.Collections;

/// <summary>
/// 运行前，角色需要预先定义的配置信息。主要是为了优化，减少运行时查找消耗
/// </summary>
/// 



public class ActorPreDefine : MonoBehaviour
{
    [Header("攻击部位")]
    [Tooltip("武器类：手\n非武器类：自定义，比如狼嘴")]
    public Transform AtkTransform;

    #region 人物边界盒子
    [System.Serializable]
    public struct Bounds
    {
        public float Offset;
        public float Width;
        public Bounds(float o, float w)
        {
            Offset = o;
            Width = w;
        }
    }
    [Header("盒子")]
    [Tooltip("用于控制玩家位置")]
    public float Offset = 0;
    public float Width = 1;

    public float XMin
    {
        get
        {
            return transform.position.x + Offset - Width / 2;
        }
    }
    public float XMax
    {
        get
        {
            return transform.position.x + Offset + Width / 2;
        }
    }
    #endregion
}