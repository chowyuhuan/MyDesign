using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_EvolutionHeroInfo_DL : MonoBehaviour
{

    public Image HeroSchool;
    public Image HeroIcon;
    public Text HeroLevel;
    public Text HeroName;
    GameObject StarBarObject;
    public GUI_HeroStarBar_DL StarBar;
    public CSV_b_hero_template HeroTemplate;

    public void SetHeroInfo(CSV_b_hero_template hero, int heroLevel, bool evolutionHero)
    {
        StarBar = StarBarObject.GetComponent<GUI_HeroStarBar_DL>();
        if (null != hero)
        {
            HeroTemplate = hero;
            GUI_Tools.IconTool.SetIcon(hero.DisplayIconAtlas, hero.DisplayIcon, HeroIcon);
            CSV_c_school_config heroSchool = CSV_c_school_config.FindData(hero.School);
            GUI_Tools.IconTool.SetIcon(heroSchool.Atlas, heroSchool.Icon, HeroSchool);
            HeroName.text = hero.Name;
            StarBar.SetStarNum(hero.Star);
            if (evolutionHero)
            {
                string level;
                TextLocalization.GetText(TextId.Level, out level);
                HeroLevel.text = level + heroLevel;
            }
            else
            {
                CSV_b_hero_limit heroLimit = CSV_b_hero_limit.FindData(hero.Star);
                HeroLevel.text = heroLevel + "/" + heroLimit.MaxLevel;
            }
        }
    }

    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_EvolutionHeroInfo dataComponent = gameObject.GetComponent<GUI_EvolutionHeroInfo>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_EvolutionHeroInfo,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            HeroSchool = dataComponent.HeroSchool;
            HeroIcon = dataComponent.HeroIcon;
            HeroLevel = dataComponent.HeroLevel;
            HeroName = dataComponent.HeroName;
            StarBarObject = dataComponent.StarBar;
        }
    }
}
