using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class GUI_BossInfo_DL : GUI_ActorBattleSimpleInfo_DL
{
    public List<Image> AttackInfoIcon = new List<Image>();

    public void SetBossInfo(int bossId)
    {
        CSV_b_monster_template monster = CSV_b_monster_template.FindData(bossId);
        SetMonsterInfo(monster);
        List<int> adList = CSVDataFile.ExtractIntArrayFromString(monster.AttackDefendDesList);
        SetAttackDeffendInfo(adList);
    }

    void SetAttackDeffendInfo(List<int> adList)
    {
        int index = 0;
        for (; index < adList.Count; ++index)
        {
            AttackInfoIcon[index].gameObject.SetActive(true);
            CSV_c_attack_defend_atrribute_description adad = CSV_c_attack_defend_atrribute_description.FindData(adList[index]);
            GUI_Tools.IconTool.SetIcon(adad.IconAtlas, adad.IconName, AttackInfoIcon[index]);
        }
        for (; index < AttackInfoIcon.Count; ++index)
        {
            AttackInfoIcon[index].gameObject.SetActive(false);
        }
    }

    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_BossInfo dataComponent = gameObject.GetComponent<GUI_BossInfo>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_BossInfo,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            AttackInfoIcon = dataComponent.AttackInfoIcon;
        }
    }
}
