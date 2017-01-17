#define SKILL_USE_SCRIPTABLEOBJECT

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SKILL;
using BUFF;


/// <summary>
/// 技能数据中心
/// 存储技能的原始数据，不包含加成部分
/// </summary>
public class SkillDataCenter : MonoBehaviour
{
    private static SkillDataCenter _instance = null;
    public static SkillDataCenter Instance
    {
        get{return _instance;}
    }


    // map or list?
    private List<Skill> SkillList = new List<Skill>();
    private Dictionary<int,Skill> SkillMap = new Dictionary<int,Skill>();
    // get技能数据接口（可能会多个）

    void Awake()
    {
        _instance = this;

        Load();
    }

    void Start()
    {
        //Load();
    }

    // 测试发现，CSV表要比ScriptableObject小很多（10000条时：scriptableobject 466kb,csv 166kb）
    // ScriptableObject不能看到版本变更
#if SKILL_USE_SCRIPTABLEOBJECT
    void Load()
    {
        Load("Configs/SkillDataBase_Sword");
        Load("Configs/SkillDataBase_Knight");
        Load("Configs/SkillDataBase_Archer");
        Load("Configs/SkillDataBase_Hunter");
        Load("Configs/SkillDataBase_Wizard");
        Load("Configs/SkillDataBase_Flamen");
        for (int row = 0; row < SkillList.Count; ++row)
        {
            SkillMap.Add(SkillList[row].ID, SkillList[row]);
        }
    }
    void Load(string path)
    {
        SkillDataBase dataBase = Resources.Load<SkillDataBase>(path);
        if (dataBase == null)
        {
            return;
        }
        dataBase.hideFlags = HideFlags.DontUnloadUnusedAsset;
        SkillList.AddRange(dataBase.Data);
    }
#else
    readonly int IDX_ID = 0; // 技能ID
    readonly int IDX_ATK = 1; // 攻击效果序列
    readonly int IDX_BF = 2; // buff序列
        void Load()
    {
        CSVDataFile file = new CSVDataFile();
        file.ParseCSVFor("skill");
        List<string[]> allData = file.GetAllData();
        for (int row = 0; row < allData.Count; ++row)
        {
            string[] line = allData[row];
            for (int col = 0; col < line.Length; ++col)
            {
                try
                {
                    Skill skill = new Skill();
                    skill.ID = int.Parse(line[IDX_ID]);
                    int offset = 0;

                    // 攻击效果
                    if (!string.IsNullOrEmpty(line[IDX_ATK]))
                    {
                        string[] atkOffset = line[IDX_ATK].Split(new char[] { '|' });
                        for (int g = 0; g < atkOffset.Length; ++g)
                        {
                            offset = int.Parse(atkOffset[g]);
                            if (skill.Attacks == null)
                            {
                                skill.Attacks = new List<DCMeta>();
                            }
                            skill.Attacks.Add(CreateOneAttackMetaFromData(line, offset));
                        }
                    }

                    // buff效果
                    if (!string.IsNullOrEmpty(line[IDX_BF]))
                    {
                        string[] bfOffset = line[IDX_BF].Split(new char[] { '|' });
                        for (int g = 0; g < bfOffset.Length; ++g)
                        {
                            if(skill.Buffs == null)
                            {
                                skill.Buffs = new List<BuffMeta>();
                            }
                            offset = int.Parse(bfOffset[g]);
                            skill.Buffs.Add(CreateOneBuffMetaFromData(line, offset));
                        }
                    }

                    SkillMap.Add(skill.ID,skill);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(LogTag.CSV + "加载技能失败：" + ex.ToString());
                    continue;
                }
            }
        }
    }

    // 创建进攻效果数据并加载
    DCMeta CreateOneAttackMetaFromData(string[] _data, int _start)
    {
        DCMeta meta = new DCMeta();
        // _data[_start]为预留位置，可以用来做验证，目前先不用
        meta.TargetEx = (Target)int.Parse(_data[_start + 1]);
        meta.CarrierEx = (Carrier)int.Parse(_data[_start + 2]);
        meta.Value = float.Parse(_data[_start + 3]);
        meta.AppearanceID = int.Parse(_data[_start + 4]);
        meta.Intention = (Intent)int.Parse(_data[_start + 5]);
        meta.MotionID = int.Parse(_data[_start + 6]);
        if (!string.IsNullOrEmpty(_data[_start + 7]))
        {
            string[] bfOffset = _data[_start + 7].Split(new char[] { '|' });
            for (int i = 0; i < bfOffset.Length; ++i )
            {
                BuffMeta buff = new BuffMeta();
                LoadBuffMetaFromData(_data, int.Parse(bfOffset[i]), ref buff);
                if (meta.EnclosedBuffs == null)
                {
                    meta.EnclosedBuffs = new List<BuffMeta>();
                }
                meta.EnclosedBuffs.Add(buff);
            }
        }
        return meta;
    }

    // 创建buff数据并加载
    BuffMeta CreateOneBuffMetaFromData(string[] _data, int _start)
    {
        BuffMeta meta = new BuffMeta();
        // _data[_start]为预留位置，可以用来做验证，目前先不用
        meta.TargetEx = (Target)int.Parse(_data[_start + 1]);
        meta.CarrierEx = (Carrier)int.Parse(_data[_start + 2]);
        meta.Value = float.Parse(_data[_start + 3]);
        meta.AppearanceID = int.Parse(_data[_start + 4]);
        meta.EffectEx = (BuffEffect)int.Parse(_data[_start + 5]);
        return meta;
    }

    // 加载buff数据
    void LoadBuffMetaFromData(string[] _data, int _start, ref BuffMeta _meta)
    {
        // _data[_start]为预留位置，可以用来做验证，目前先不用
        _meta.TargetEx = (Target)int.Parse(_data[_start + 1]);
        _meta.CarrierEx = (Carrier)int.Parse(_data[_start + 2]);
        _meta.Value = float.Parse(_data[_start + 3]);
        _meta.AppearanceID = int.Parse(_data[_start + 4]);
        _meta.EffectEx = (BuffEffect)int.Parse(_data[_start + 5]);
    }
#endif
    // 获取技能
    public bool TryToGetSkill(int skillID, out Skill data)
    {
        return SkillMap.TryGetValue(skillID, out data);
    }

    public NewTriggerMeta GetSkillTrigger(int skillID, int triggerIndex)
    {
        Skill skill;
        TryToGetSkill(skillID, out skill);

        if (skill != null)
        {
            return skill.Triggers[triggerIndex];
        }
        return null;
    }

    public NewBuffMeta GetSkillBuff(int skillID, int buffIndex)
    {
        Skill skill;
        TryToGetSkill(skillID, out skill);

        if (skill != null)
        {
            return skill.Buffs[buffIndex];
        }
        return null;
    }
}
