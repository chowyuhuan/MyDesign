using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ACTOR;
using SKILL;

public class GUI_SkillCubeManager : Singleton<GUI_SkillCubeManager>
{
    public const int _SkillCubeCount = 8;
    List<GUI_SkillCubeItem_DL> _SkillCubeList = new List<GUI_SkillCubeItem_DL>();
    List<Transform> _SkillCubePosList = new List<Transform>();
    List<bool> _SkillCubePosTag;
    List<int> _SkillGroupList = new List<int>();//相同的技能块为一组
    List<GUI_SkillCubeItem_DL> _GroupSkill = new List<GUI_SkillCubeItem_DL>();
    GUI_LogicObjectPool _SkillCubeItemPool;
    float _SkillCubeMoveTime;
    float _SkillCubeDistance;

    protected override void OnConstract()
    {
        _SkillCubePosTag = new List<bool>(_SkillCubeCount);

        for (int index = 0; index < _SkillCubeCount; ++index)
        {
            _SkillCubePosTag.Add(true);
        }

        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/SkillCubeItem", true, AssetManage.E_AssetType.UIPrefab);
        _SkillCubeItemPool = new GUI_LogicObjectPool(go);
    }

    public void InitSkillCubePos(GUI_BattleUI_DL battleUI)
    {
        _SkillCubePosList.Clear();
        _SkillCubeList.Clear();
        _SkillCubeMoveTime = battleUI.SkillCubeMoveTime;
        _SkillCubeDistance = battleUI.SkillCubeDistance;
        for (int index = battleUI._SkillCubePosList.Count - 1; index >= 0; --index)
        {
            _SkillCubePosList.Add(battleUI._SkillCubePosList[index]);
        }
        for (int index = 0; index < _SkillCubeCount; ++index)
        {
            _SkillCubePosTag[index] = true;
        }
    }

    public GUI_SkillCubeItem_DL GetOneFreeSkillCube()
    {
        GUI_SkillCubeItem_DL sci = _SkillCubeItemPool.GetOneLogicComponent() as GUI_SkillCubeItem_DL;
        return sci;
    }

    public void UseSkillCube(GUI_SkillCubeItem_DL sci)
    {
        if (null != sci)
        {
            _SkillCubeList.Add(sci);
            RefreshSkillCubePos();
        }
    }

    List<int> GetSkillGroup(GUI_SkillCubeItem_DL sci)
    {
#if UNITY_EDITOR
        Debug.Assert(null != sci);
#endif
        for (int index = 0; index < _SkillCubeList.Count; ++index)
        {
            if (sci._GroupId == _SkillCubeList[index]._GroupId)
            {
                _SkillGroupList.Add(_SkillCubeList[index]._CurIndex);
                if (_SkillGroupList.Count == 3)
                {
                    break;
                }
            }
        }
        return _SkillGroupList;
    }

    void ResetGroupInfo()
    {
        for (int index = 0; index < _SkillCubeList.Count; ++index)
        {
            _SkillCubeList[index]._GroupId = -1;
        }
    }

    int RefreshGroupInfo(int index, int targetGroupId)
    {
        int grouped = 1;
        _SkillCubeList[index]._GroupId = targetGroupId;
        if (SameSkillCube(index, index + 1))//后面的一个
        {
            ++grouped;
            _SkillCubeList[index + 1]._GroupId = targetGroupId;
            if (SameSkillCube(index + 1, index + 2))//后面的第二个
            {
                ++grouped;
                _SkillCubeList[index + 2]._GroupId = targetGroupId;
            }
        }
        return grouped;
    }


    public void RefreshGroupInfo()
    {
        ResetGroupInfo();
        int groupId = 1;
        for (int index = 0; index < _SkillCubeList.Count; )
        {
            int groupStep = RefreshGroupInfo(index, groupId);
            ++groupId;
            if (groupStep == 2)
            {
                GUI_BattleManager.Instance.BattleUI.ShowDoubleAlignEffect(_SkillCubeList[index]);
                _SkillCubeList[index + 1].HideAlignEffect();
            }
            else if (groupStep == 3)
            {
                GUI_BattleManager.Instance.BattleUI.ShowTribleAlignEffect(_SkillCubeList[index]);
                _SkillCubeList[index + 1].HideAlignEffect();
                _SkillCubeList[index + 2].HideAlignEffect();
            }
            else
            {
                _SkillCubeList[index].HideAlignEffect();
            }
            index += groupStep;
        }
    }

    bool ValidGUIItem(int index)
    {
        return index >= 0 && index < _SkillCubeCount && !_SkillCubePosTag[index];
    }

    bool SameSkillCube(int preindex, int postindex)
    {
        GUI_SkillCubeItem_DL pre = null;
        GUI_SkillCubeItem_DL post = null;

        if (ValidGUIItem(preindex))
        {
            pre = _SkillCubeList[preindex];
        }
        else
        {
            return false;
        }
        if (ValidGUIItem(postindex))
        {
            post = _SkillCubeList[postindex];
        }
        else
        {
            return false;
        }

        return SameSkillCube(pre, post);
    }

    bool SameSkillCube(GUI_SkillCubeItem_DL pre, GUI_SkillCubeItem_DL post)
    {
#if UNITY_EDITOR
        Debug.Assert(null != pre);
        Debug.Assert(null != post);
#endif
        if (pre._SpacialSkill || post._SpacialSkill)
        {
            return false;
        }
        float distance = Mathf.Abs(pre.CachedTransform.localPosition.x - post.CachedTransform.localPosition.x);
        return pre._HeroBattleId == post._HeroBattleId && pre._SkillId == post._SkillId && distance < _SkillCubeDistance;
    }

    public void RecycleOneSkillCube(GUI_SkillCubeItem_DL sci)
    {
        if (null != sci)
        {
            List<int> group = GetSkillGroup(sci);
            SkillGenerator.Instance.SkillCast(sci._HeroBattleId, sci._SkillId, group.Count);
            Actor actor = ActorManager.Instance.FindActorById(sci._HeroBattleId); // TODO:待优化
            if (actor != null)
            {
                int skillIndex = sci._SpacialSkill ? 3 : group.Count - 1;
                actor.SkillController.InputEx.Input(skillIndex); // TODO:这个地方，需要UI层修改传入的参数：1、2、3消还是特殊技能
                actor.ActorReference.ActorRenderEx.Outline(sci.HeroDisplayOrder);
            }
            for (int index = 0; index < group.Count; ++index)
            {
                GUI_BattleManager.Instance.BattleUI.ShowCubeClickEffect(_SkillCubeList[group[index]].HeroDisplayOrder, 7 - group[index]);
                _GroupSkill.Add(_SkillCubeList[group[index]]);
            }
            for (int index = 0; index < _GroupSkill.Count; ++index)
            {
                RecycleSkillCubeItem(_GroupSkill[index]);
            }
            group.Clear();
            _GroupSkill.Clear();
            RefreshSkillCubePos();
        }
    }

    void RecycleSkillCubeItem(GUI_SkillCubeItem_DL sci)
    {
        if (null != sci)
        {
            RecyclePos(sci._CurIndex);
            _SkillCubeList.Remove(sci);
            sci.HideAlignEffect();
            sci.Recycle();
            //RefreshSkillCubePos();
        }
    }

    void RecyclePos(int index)
    {
        if (index < _SkillCubeCount && index >= 0)
        {
            _SkillCubePosTag[index] = true;
        }
    }

    void OcupaPos(int index)
    {
        if (index < _SkillCubeCount && index >= 0)
        {
            _SkillCubePosTag[index] = false;
        }
    }

    public int GetFirstEtmptyPos()
    {
        int pos = 0;
        for (int index = 0; index < _SkillCubePosTag.Count; ++index)
        {
            if (_SkillCubePosTag[index])
            {
                pos = index;
                break;
            }
        }
        return pos;
    }

    public void RefreshSkillCubePos()
    {
        for (int index = 0; index < _SkillCubeList.Count; ++index)
        {
            GUI_SkillCubeItem_DL sci = _SkillCubeList[index];
            if (sci._CurIndex != index)
            {
                int emptypos = GetFirstEtmptyPos();
                RecyclePos(sci._CurIndex);
                OcupaPos(emptypos);
                Vector3 tp = _SkillCubePosList[emptypos].localPosition;
                sci.Tween(tp, _SkillCubeMoveTime * (sci._CurIndex - index), emptypos);
            }
            else
            {
                if (sci._CurIndex == _SkillCubeCount - 1)
                {
                    OcupaPos(sci._CurIndex);
                }
            }
        }
        RefreshGroupInfo();
    }

    public void OnHeroDead(Actor actor)
    {
        for (int index = 0; index < _SkillCubeList.Count; ++index)
        {
            if (_SkillCubeList[index]._HeroBattleId == actor.BattleId)
            {
                _SkillCubeList[index].HeroAlive(false);
            }
        }
    }

    public void OnBattleEnd()
    {
        _SkillCubeItemPool.RecycleAll();
        _SkillCubeItemPool.ClearPool();
    }
}