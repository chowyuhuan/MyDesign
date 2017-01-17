using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SKILL;

/// <summary>
/// 技能组的容器 
/// </summary>
public class MonsterSkillContainer : SpChange
{
    // 转换到下一状态的条件值 
    public float TransitionValue;

    List<SkillSegment> _spSegments = new List<SkillSegment>();
    int _spIndex;

    bool hasSp
    {
        get
        {
            return _spSegments.Count > 0;
        }
    }

    /// <summary>
    ///  初始化并添加技能序列 
    /// </summary>
    public void Initialize(float condition, int csvId)
    {
        TransitionValue = condition;

        CSV_c_monster_skill_group skillGroupCSV = CSV_c_monster_skill_group.FindData(csvId);

        _spSegments.Clear();
        for ( int i = 0; i < skillGroupCSV.skillCount; ++i)
        {
            MonsterSkillSegment segment = new MonsterSkillSegment();

            segment.Initialize(skillGroupCSV.skillIds[i], skillGroupCSV.durations[i]);


            _spSegments.Add(segment);
        }
    }

    /// <summary>
    /// 开始 
    /// </summary>
    public void Enter()
    {
        _spIndex = 0;

        DoNext();
    }

    public void DoNext()
    {
        RegisterSegmentCallback();

        _spSegments[_spIndex].Enter();

        CanChange = true;
    }

    void RegisterSegmentCallback()
    {
        _spSegments[_spIndex].OnSpProgressChange += SpProgressChange;
        _spSegments[_spIndex].OnSpSkillFilled += SpSkillFilled;
    }

    void UnregisterSegmentCallback()
    {
        _spSegments[_spIndex].OnSpProgressChange -= SpProgressChange;
        _spSegments[_spIndex].OnSpSkillFilled -= SpSkillFilled;
    }

    /// <summary>
    /// 减少Sp数值 
    /// </summary>
    /// <param name="amount"></param>
    public void ReduceSp(int amount)
    {
        if (hasSp && CanChange)
            _spSegments[_spIndex].Reduce(amount);
    }

    /// <summary>
    /// Sp技能进度 
    /// </summary>
    /// <param name="value"></param>
    public void SpProgressChange(float value)
    {
        RaiseSpProgressChange(value);
    }

    /// <summary>
    /// Sp技能填充完毕 
    /// </summary>
    /// <param name="skillId"></param>
    public void SpSkillFilled(int skillId)
    {
        RaiseSpSkillFilled(skillId);

        UnregisterSegmentCallback();

        _spIndex++;
        _spIndex %= _spSegments.Count;

        CanChange = false;
    }

    public void Tick()
    {
        if (hasSp && CanChange)
        {
            _spSegments[_spIndex].Tick();
        }
	}
}
