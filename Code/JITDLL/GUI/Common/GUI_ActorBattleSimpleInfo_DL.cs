using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_ActorBattleSimpleInfo_DL : MonoBehaviour
{
    public Image HeadIcon;
    public Image SchoolIcon;
    public Text LevelNumber;
    public Text StarNumber;

    protected void SetHeroInfo(CSV_b_hero_template heroTemplate)
    {
        if (null != heroTemplate)
        {
            SchoolIcon.gameObject.SetActive(true);
            StarNumber.text = heroTemplate.Star.ToString();
            GUI_Tools.IconTool.SetIcon(heroTemplate.HeadIconAtlas, heroTemplate.HeadIcon, HeadIcon);
            CSV_c_school_config heroSchool = CSV_c_school_config.FindData(heroTemplate.School);
            GUI_Tools.IconTool.SetIcon(heroSchool.Atlas, heroSchool.Icon, SchoolIcon);
        }
        else
        {
            Clear();
        }
    }

    void Clear()
    {
        HeadIcon.sprite = null;
        SchoolIcon.sprite = null;
        LevelNumber.text = "";
        StarNumber.text = "";
    }

    protected void SetMonsterInfo(CSV_b_monster_template monster)
    {
#if UNITY_EDITOR
        Debug.Assert(null != monster);
#endif
        GUI_Tools.IconTool.SetIcon(monster.HeadIconAtlas, monster.HeadIcon, HeadIcon);
        SchoolIcon.gameObject.SetActive(false);
        if (monster.Star > 0)
        {
            StarNumber.text = monster.Star.ToString();
        }
        else
        {
            StarNumber.text = "";
        }
        LevelNumber.text = monster.Level.ToString();
    }

    void Awake()
    {
        this.CopyDataFromDataScript();
        OnAwake();
    }

    protected virtual void OnAwake()
    {

    }

    protected virtual void CopyDataFromDataScript()
    {
        GUI_ActorBattleSimpleInfo dataComponent = gameObject.GetComponent<GUI_ActorBattleSimpleInfo>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ActorBattleSimpleInfo,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            HeadIcon = dataComponent.HeadIcon;
            SchoolIcon = dataComponent.SchoolIcon;
            LevelNumber = dataComponent.LevelNumber;
            StarNumber = dataComponent.StarNumber;
        }
    }
}
