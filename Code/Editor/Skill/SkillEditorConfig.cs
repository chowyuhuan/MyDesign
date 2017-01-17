using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SKILL;
using ACTOR;
using UnityEditor;

namespace SKILL_EDITOR
{

    public enum Node
    {
        Skill,
        ATK,
        DC,
        Buff,
        VolAtk,
        VolField,
        Trigger,

        BuffState,
        BuffCondition,
        BuffConditionTarget,
        BuffConditionRed,
        BuffConditionGreen,
        TriggerBehaviorFork,
        TriggerBehavior,
    }

    // 在SKILL.Condition基础上，再进行一次便利整合
    // 之所以没有把SKILL.Condition直接换成SKILL_EDITOR.Condition，是因为Checker完全独立了，与Condition没有任何关系，
    // Checker中的Condition仅仅为了存储其条件，并不打算作为Condition内部逻辑的验证基准，故没有针对具体的Condition做
    // 逻辑（比如：Conditon == HP时，OP就应该为Assign，Comparer就应该为<=，FixChanged必然为false，就没有必要使用Op、
    // Comparer、FixChanged等），如果不好理解，可以认为这个Condition应该被挪出Checker的。
    // 通过配置Checker中除了Condition外的其他选项，可以做到无限的扩展。而一旦使用Condition做限定的逻辑，就不能做到无
    // 限扩展。而此处的Condition为编辑器使用，即不会影响运行时内容。所以，这里怎么做，都是OK的，尽量做到最简化。
    public enum Condition
    {
        EraseBlock      = 0, // 消块
        EraseBlockSum   = 1, // 累计消块
        EraseBlockTime  = 2, // 消块次数
        Effect          = 3, // 战斗效果
        HP              = 4, // 体力
        Probability     = 5, // 概率
    }

    public abstract class SkillPopup : EditorWindow
    {
        public SkillPopup ParentWin = null;

        public void ShowWin()
        {
            if (ParentWin != null)
            {
                //ParentWin.Close();
            }
            //Show();
            ShowTab();
            //ShowPopup();
        }

        void OnGUI()
        {
            OnGUIImp();
            //if (GUI.Button(new Rect(position.width - 200, position.height - 43, 197, 40), "返回") && ParentWin != null)
            //{
            //    ParentWin.Show();
            //    Close();
            //}
        }

        protected abstract void OnGUIImp();
    }

    public class ObjectToStringField
    {
        GUIContent _content;
        UnityEngine.Object _selectedObj;
        Type _type;
        bool _forceNonNull = false;
        public void Init(GUIContent content, UnityEngine.Object obj, System.Type type, bool nonNull)
        {
            _content = content;
            _selectedObj = obj;
            _type = type;
            _forceNonNull = nonNull;
        }
        public void Init(GUIContent content, string path, System.Type type, bool nonNull)
        {
            _content = content;
            if (!string.IsNullOrEmpty(path))
            {
                _selectedObj = AssetDatabase.LoadAssetAtPath(path, type);
            }
            _type = type;
            _forceNonNull = nonNull;
        }
        public string ObjectField()
        {
            UnityEngine.Object curObject = _selectedObj;
            curObject = EditorGUILayout.ObjectField(_content, curObject, _type, true);
            if(!_forceNonNull || curObject != null)
            {
                _selectedObj = curObject;
            }
            return _selectedObj == null ? null : _selectedObj.name;
        }
    }

}
