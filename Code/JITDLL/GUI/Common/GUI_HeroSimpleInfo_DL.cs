using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class GUI_HeroSimpleInfo_DL : GUI_ToggleItem_DL
{
    public Text Level;
    public Color SelectedColor;
    public Image SchoolIcon;
    public bool WeaponMax;
    public Text TrainingLevel;
    public Image HeadIcon;
    GameObject HeroStarBarObject;
    public GUI_HeroStarBar_DL HeroStar;
    public CSV_b_hero_template HeroTemplate
    {
        get;
        protected set;
    }

    public DataCenter.Hero Hero
    {
        get;
        protected set;
    }

    protected void SetHeroSimpleInfo(DataCenter.Hero hero)
    {
        HeroStar = HeroStarBarObject.GetComponent<GUI_HeroStarBar_DL>();
        Hero = hero;
        HeroTemplate = CSV_b_hero_template.FindData(hero.CsvId);
        Level.text = "LV" + Hero.Level.ToString();
        CSV_b_hero_limit heroLimit = CSV_b_hero_limit.FindData(HeroTemplate.Star);
        GUI_Tools.IconTool.SetIcon(HeroTemplate.DisplayIconAtlas, HeroTemplate.DisplayIcon, HeadIcon);
        if (Hero.EnhanceLevel > 0)
        {
            if (hero.EnhanceLevel >= heroLimit.TrainingLevel)
            {
                string maxValue = null;
                if (TextLocalization.GetText(TextId.Max_Value, out maxValue))
                {
                    TrainingLevel.text = maxValue;
                }
                else
                {
                    TrainingLevel.text = "Max";
                }
            }
            else
            {
                TrainingLevel.text = "+" + Hero.EnhanceLevel;
            }
        }
        else
        {
            TrainingLevel.text = "";
        }
        HeroStar.SetStarNum(HeroTemplate.Star);
        CSV_c_school_config heroSchool = CSV_c_school_config.FindData(HeroTemplate.School);
        GUI_Tools.IconTool.SetIcon(heroSchool.Atlas, heroSchool.Icon, SchoolIcon);
    }

    protected void SelectHero()
    {
        Level.text = GUI_Tools.RichTextTool.Color(SelectedColor, "LV" + Hero.Level.ToString());
    }

    protected void DeSelectHero()
    {
        if (null != Hero)
        {
            Level.text = "LV" + Hero.Level.ToString();
        }
        else
        {
            Level.text = "";
        }
    }

    public static CompareComponent GetCompareFunc(E_Hero_OrderType orderType)
    {
        switch (orderType)
        {
            case E_Hero_OrderType.Attack:
                {
                    return CompareByAttack;
                }
            case E_Hero_OrderType.CritDamage:
                {
                    return CompareByCritDamage;
                }
            case E_Hero_OrderType.CritRatio:
                {
                    return CompareByCritRatio;
                }
            case E_Hero_OrderType.Difficulty:
                {
                    return CompareByDifficulty;
                }
            case E_Hero_OrderType.Dodge:
                {
                    return CompareByDodge;
                }
            case E_Hero_OrderType.Hp:
                {
                    return CompareByHp;
                }
            case E_Hero_OrderType.Level:
                {
                    return CompareByLevel;
                }
            case E_Hero_OrderType.MagicalDefend:
                {
                    return CompareByMagicDefend;
                }
            case E_Hero_OrderType.PhysicalDefend:
                {
                    return CompareByPhysicalDefend;
                }
            case E_Hero_OrderType.Precision:
                {
                    return CompareByPrecision;
                }
            case E_Hero_OrderType.StarNum:
                {
                    return CompareByStar;
                }
            case E_Hero_OrderType.TrainingLevel:
                {
                    return CompareByTrainingLevel;
                }
            default:
                {
                    return CompareByAttack;
                }
        }
    }

    public static int CompareByAttack(int a, int b)
    {
        return CompareHero(a, b, E_Hero_OrderType.Attack);
    }

    public static int CompareByHp(int a, int b)
    {
        return CompareHero(a, b, E_Hero_OrderType.Hp);
    }

    public static int CompareByCritDamage(int a, int b)
    {
        return CompareHero(a, b, E_Hero_OrderType.CritDamage);
    }

    public static int CompareByCritRatio(int a, int b)
    {
        return CompareHero(a, b, E_Hero_OrderType.CritRatio);
    }

    public static int CompareByDodge(int a, int b)
    {
        return CompareHero(a, b, E_Hero_OrderType.Dodge);
    }

    public static int CompareByLevel(int a, int b)
    {
        return CompareHero(a, b, E_Hero_OrderType.Level);
    }

    public static int CompareByMagicDefend(int a, int b)
    {
        return CompareHero(a, b, E_Hero_OrderType.MagicalDefend);
    }

    public static int CompareByPhysicalDefend(int a, int b)
    {
        return CompareHero(a, b, E_Hero_OrderType.PhysicalDefend);
    }

    public static int CompareByPrecision(int a, int b)
    {
        return CompareHero(a, b, E_Hero_OrderType.Precision);
    }

    public static int CompareByStar(int a, int b)
    {
        return CompareHero(a, b, E_Hero_OrderType.StarNum);
    }

    public static int CompareByTrainingLevel(int a, int b)
    {
        return CompareHero(a, b, E_Hero_OrderType.TrainingLevel);
    }

    public static int CompareByDifficulty(int a, int b)
    {
        return CompareHero(a, b, E_Hero_OrderType.StarNum);
    }

    static bool ValidHeroIndex(int heroIndex)
    {
        return heroIndex >= 0 && heroIndex < DataCenter.PlayerDataCenter.HeroList.Count;
    }

    static int CompareHero(int a, int b, E_Hero_OrderType orderType)
    {
        int result;
        DataCenter.Hero heroA = ValidHeroIndex(a) ? DataCenter.PlayerDataCenter.HeroList[a] : null;
        DataCenter.Hero heroB = ValidHeroIndex(b) ? DataCenter.PlayerDataCenter.HeroList[b] : null;
        if (null != heroA && null != heroB)
        {
            switch (orderType)
            {
                case E_Hero_OrderType.Attack:
                    {
                        result = Mathf.Clamp((int)(heroA.TotalAttribute.Attack - heroB.TotalAttribute.Attack), -1, 1);
                        break;
                    }
                case E_Hero_OrderType.Hp:
                    {
                        result = Mathf.Clamp((int)(heroA.TotalAttribute.Hp - heroB.TotalAttribute.Hp), -1, 1);
                        break;
                    }
                case E_Hero_OrderType.CritDamage:
                    {
                        result = Mathf.Clamp((int)(heroA.TotalAttribute.CrticalDamage - heroB.TotalAttribute.CrticalDamage), -1, 1);
                        break;
                    }
                case E_Hero_OrderType.CritRatio:
                    {
                        result = Mathf.Clamp((int)(heroA.TotalAttribute.CrticalRate - heroB.TotalAttribute.CrticalRate), -1, 1);
                        break;
                    }
                case E_Hero_OrderType.Dodge:
                    {
                        result = Mathf.Clamp((int)(heroA.TotalAttribute.Dodge - heroB.TotalAttribute.Dodge), -1, 1);
                        break;
                    }
                case E_Hero_OrderType.Level:
                    {
                        result = Mathf.Clamp((int)(heroA.Level - heroB.Level), -1, 1);
                        break;
                    }
                case E_Hero_OrderType.MagicalDefend:
                    {
                        result = Mathf.Clamp((int)(heroA.TotalAttribute.MagicDefence - heroB.TotalAttribute.MagicDefence), -1, 1);
                        break;
                    }
                case E_Hero_OrderType.PhysicalDefend:
                    {
                        result = Mathf.Clamp((int)(heroA.TotalAttribute.PhysicalDefence - heroB.TotalAttribute.PhysicalDefence), -1, 1);
                        break;
                    }
                case E_Hero_OrderType.Precision:
                    {
                        result = Mathf.Clamp((int)(heroA.TotalAttribute.Precision - heroB.TotalAttribute.Precision), -1, 1);
                        break;
                    }
                case E_Hero_OrderType.TrainingLevel:
                    {
                        result = Mathf.Clamp((int)(heroA.EnhanceLevel - heroB.EnhanceLevel), -1, 1);
                        break;
                    }
                case E_Hero_OrderType.StarNum:
                    {
                        CSV_b_hero_template aTemplate = CSV_b_hero_template.FindData(heroA.CsvId);
                        CSV_b_hero_template bTemplate = CSV_b_hero_template.FindData(heroB.CsvId);
                        result = Mathf.Clamp(aTemplate.Star - bTemplate.Star, -1, 1);
                        break;
                    }
                default:
                    {
                        result = Mathf.Clamp((int)(heroA.TotalAttribute.Attack - heroB.TotalAttribute.Attack), -1, 1);
                        break;
                    }
            }
        }
        else if (null == heroA)
        {
            result = 0;
        }
        else
        {
            result = 0;
        }
        return result;
    }
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_HeroSimpleInfo dataComponent = gameObject.GetComponent<GUI_HeroSimpleInfo>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroSimpleInfo,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            Level = dataComponent.Level;
            SchoolIcon = dataComponent.SchoolIcon;
            WeaponMax = dataComponent.WeaponMax;
            TrainingLevel = dataComponent.TrainingLevel;
            HeadIcon = dataComponent.HeadIcon;
            HeroStarBarObject = dataComponent.HeroStar;
        }
    }
}
