using UnityEngine;
using System.Collections;

namespace SKILL
{
    /// <summary>
    /// 技能块控制器
    /// 控制技能块的产生和删除
    /// 与UI互动
    /// </summary>
    public class BlockController : MonoBehaviour
    {
        // 8块技能存储结构，删除添加比较频繁，需要优化对待
        // 技能块添加回调（调UI表现）
        // 技能块删除回调（调UI表现）

        /// <summary>
        /// 添加一个技能块
        /// </summary>
        /// <param name="_warrior">勇士</param>
        /// <param name="_index">技能块索引（应该就是0和1）</param>
        public void Add(int _warrior, int _index)
        {

        }

        /// <summary>
        /// 删除技能块
        /// </summary>
        /// <param name="_index">技能块索引</param>
        /// <param name="_count">块数</param>
        public void Remove(int _index, int _count)
        {
            ;
        }

        /// <summary>
        /// 定时产生技能块，代替Update
        /// </summary>
        /// <returns></returns>
        IEnumerator Generate()
        {
            yield return null;
        }
    }
}