using UnityEngine;
using System.Collections;

namespace ProgressLoading
{
    /// <summary>
    /// 加载处理基类
    /// </summary>
    public class PL_LoadingProcessor
    {
        public virtual void Prepare(string _strParam)
        {
            ;
        }
        public virtual void Prepare(string _strParam, int _intParam)
        {
            ;
        }
        /// <summary>
        /// 每帧都会调用，所以，内部的工作量为一帧的工作量.
        /// </summary>
        /// <returns>加载进度：1：加载结束；小于1：正在加载</returns>
        public virtual float Load()
        {
            return Progress(0.0f);
        }

        protected float Progress(float _p)
        {
            return Mathf.Clamp(_p, 0, 1);
        }
    }
}