using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SKILL
{
    /// <summary>
    /// 技能持有器
    /// 每个勇士挂一个，记录持有哪些技能
    /// </summary>
    public class SkillPossessor : CompBase
    {
        public class Track
        {
            /// <summary>
            /// 计数方式
            /// </summary>
            public enum CountDown
            {
                Time    =   0,      // 时间
                Count   =   1,      // 次数
            }
            public int ID; // 数据
            public int ExchangeCount; // 转换次数
            public float LastTime; // 持续时间
            public CountDown CountDownEx; // 计数方式

            public int Reference;
            public int CurExchangedCount; // 已经转换的次数
            public float CurLastTime; // 已经持续的时间
            public Track()
            {
                Clear();
            }
            public int GetID()
            {
                int preID = ID;
                if (CountDown.Count == CountDownEx && ++CurExchangedCount >= ExchangeCount)
                {
                    Clear();
                }
                return preID;
            }
            public void Exchange(int id, CountDown type, int count, float time)
            {
                ID = id;
                CountDownEx = type;
                ExchangeCount = count;
                LastTime = time;

                Reference = 0;
                CurExchangedCount = 0;
                CurLastTime = 0;
            }
            // 添加一次记录
            public void Stroke(int block)
            {
                Reference = Reference | block;
            }

            // 去除一次记录
            public void Erase(int block)
            {
                Reference = Reference & (~block);
                if ((Reference & 0x111) == 0)
                {
                    Clear();
                }
            }
            public void Clear()
            {
                Reference = 0;
                ID = 0;
            }
            public void Update(float deltaTime)
            {
                if (CountDownEx == CountDown.Time && CurLastTime + deltaTime >= LastTime)
                {
                    Clear();
                }
            }
            public bool Idle()
            {
                return Reference == 0;
            }
        }

        class Buffer
        {
            Track[] _cache = null;
            public Buffer()
            {
                _cache = new Track[]{new Track(), new Track(), new Track()};
            }
            public Track GetIdleTrack()
            {
                for (int i = 0; i < _cache.Length; ++i)
                {
                    if (_cache[i].Idle())
                    {
                        return _cache[i];
                    }
                }
                return null;
            }
            public void Erase(int block)
            {
                for (int i = 0; i < _cache.Length; ++i)
                {
                    _cache[i].Erase(block);
                }
            }
            public void Clear()
            {
                for (int i = 0; i < _cache.Length; ++i)
                {
                    _cache[i].Clear();
                }
            }
            public void Update(float deltaTime)
            {
                for (int i = 0; i < _cache.Length; ++i)
                {
                    if(!_cache[i].Idle())
                    {
                        _cache[i].Update(deltaTime);
                    }
                }
            }
        }
        // Idx为Index缩写
        Buffer _idxBuffer = null;
        Buffer _skillBuffer = null;

        Track[] _idxToIdx = new Track[3]; // m消被视为n消
        Track[] _idxToSkill = new Track[3]; // n消对应x技能;n消技能被替换为x技能（发生替换时）

        // 原始数据
        int[] _primitiveSkill = new int[4]; // 1、2、3消、特殊对应的原始技能

        // 被动技能
        int _passiveSkill = -1;

        public override void Init(Actor a)
        {
            base.Init(a);
            LoadSkills();
            for (int i = 0; i < 3; ++i)
            {
                _idxToIdx[i] = _idxBuffer.GetIdleTrack();
                _idxToSkill[i] = _skillBuffer.GetIdleTrack();
            }
        }

        /// <summary>
        /// 取得技能ID
        /// </summary>
        /// <param name="_index">勇士最多有4中输入，1、2、3消方框（_index分别为0、1、2）和1消圆（_index为3），为了通用，这里用索引</param>
        /// <returns>技能ID</returns>
        public int GetSkillID(int index)
        {
            if (index < 3)
            {
                int id = _idxToIdx[index].Idle() ? index : _idxToIdx[index].GetID();
                return _idxToSkill[id].Idle() ? _primitiveSkill[id] : _idxToSkill[id].GetID();
            }
            else
            {
                return _primitiveSkill[3];
            }
        }

        /// <summary>
        /// 取得原始技能ID
        /// </summary>
        /// <param name="_index">勇士最多有4中输入，1、2、3消方框（_index分别为0、1、2）和1消圆（_index为3），为了通用，这里用索引</param>
        /// <returns>技能ID</returns>
        public int GetPrimitiveSkillID(int index)
        {
            if (index < 4 && index > -1)
            {
                return _primitiveSkill[index];
            }
            return 0;
        }

        /// <summary>
        /// 取得被动技能ID
        /// </summary>
        /// <returns></returns>
        public int GetPassiveSkillID()
        {
            return _passiveSkill;
        }

        public void Update()
        {
            _idxBuffer.Update(GameTimer.deltaTime);
            _skillBuffer.Update(GameTimer.deltaTime);
        }

        /// <summary>
        /// 加载持有的技能数据
        /// 应该与勇士的星数和领悟的特殊技能决定
        /// </summary>
        void LoadSkills()
        {
            // 读取配置信息
            _idxBuffer = new Buffer();
            _skillBuffer = new Buffer();

            if (!Owner.actorPrepareInfo.NeedAutoCast)
            {
                // 1\2\3消对应技能 
                CSV_b_hero_template heroCSV = CSV_b_hero_template.FindData(Owner.ConfigId);
                for (int i = 0; i < heroCSV.SkillIDs.Count; ++i)
                {
                    _primitiveSkill[i] = heroCSV.SkillIDs[i];
                }

                // 特殊技能 
                _primitiveSkill[3] = Owner.specialSkillId;

                _passiveSkill = heroCSV.PassiveSkillID;
            }
        }

    }

}