using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_Tools
{
    public class ObjectTool
    {
        public static void ActiveObject(GameObject go, bool active)
        {
            if(null != go)
            {
                if(active && !go.activeInHierarchy)
                {
                    go.SetActive(true);
                }
                else if(!active && go.activeInHierarchy)
                {
                    go.SetActive(false);
                }
            }
        }

        public static T CloneObject<T>(GameObject proto, out GameObject cloneOb) where T : Component
        {
            if (null != proto)
            {
                cloneOb = GameObject.Instantiate(proto);
                return cloneOb.GetComponent<T>();
            }
            cloneOb = null;
            return null;
        }
    }
    public class RichTextTool
    {
        public static string IntProgressString(int progress, int baseValue)
        {
            return string.Format("{0}{1}{2}", progress.ToString(), "/", baseValue.ToString());
        }

        public static string Color(Color color, string text)
        {
            return string.Format("{0}{1}{2}{3}{4}", "<color=\"#", ColorUtility.ToHtmlStringRGBA(color), "\">", text, "</color>");
        }
        /// <summary>
        /// where "htmlcolor" is a html color string, like "#ff0000" or "red"
        /// </summary>
        /// <param name="htmlcolor"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Color(string htmlcolor, string text)
        {
            return string.Format("{0}{1}{2}{3}{4}", "<color=\"", htmlcolor, "\">", text, "</color>");
        }

        public static string Size(int textSize, string text)
        {
            return string.Format("{0}{1}{2}{3}{4}", "<size=\"", textSize.ToString(), "\">", text, "</size>");
        }

        public static string Italic(string text)
        {
            return string.Format("{0}{1}{2}", "<b>", text, "</b>");
        }

        public static string Bold(string text)
        {
            return string.Format("{0}{1}{2}", "<i>", text, "</i>");
        }
    }

    public class ModelTool
    {
        public static bool SpawnModel(GameObject parent, string prefabName, GUI_Transform modelTrans, out GameObject model)
        {
            model = null;
            if (!string.IsNullOrEmpty(prefabName))
            {
                model = AssetManage.AM_Manager.LoadAssetSync<GameObject>(AssetManage.AM_PathHelper.GetActorFullPathByName(prefabName), true, AssetManage.E_AssetType.ActorPrefab);
                model = GameObject.Instantiate(model);
                if (model != null && parent != null)
                {
                    Transform t = model.transform;
                    Vector3 originalScal = t.localScale;
                    t.SetParent(parent.transform);
                    if (null != modelTrans)
                    {
                        t.localPosition = modelTrans.Position;
                        t.localRotation = Quaternion.Euler(modelTrans.Rotation);
                        t.localScale = new Vector3(originalScal.x * modelTrans.Scale.x, originalScal.y * modelTrans.Scale.y, originalScal.z * modelTrans.Scale.z);
                    }
                }
            }
            return null != model;
        }

        public static bool AnimateUIModel(GameObject model, string animationControllerPath, out Animator anim)
        {
            return AnimateModel(model, AssetManage.AM_PathHelper.GetActorUIAnimCtrlFullPathByName(animationControllerPath), out anim);
        }
        
        public static bool AnimateEvolutionModel(GameObject model, string animationControllerPath, out Animator anim)
        {
            return AnimateModel(model, AssetManage.AM_PathHelper.GetActorUIEvoAnimCtrlFullPathByName(animationControllerPath), out anim);
        }

        static bool AnimateModel(GameObject model, string animationControllerPath, out Animator anim)
        {
            anim = null;
            if(null != model && !string.IsNullOrEmpty(animationControllerPath))
            {
                anim = model.AddComponent<Animator>();
                anim.runtimeAnimatorController = AssetManage.AM_Manager.LoadAssetSync<AnimatorOverrideController>(animationControllerPath, true, AssetManage.E_AssetType.AnimatorOverrideController);
                if (anim.runtimeAnimatorController != null)
                {
                    anim.updateMode = AnimatorUpdateMode.Normal;
                    anim.applyRootMotion = false;
                    anim.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
                }
            }
            return null != anim;
        }
    }

    public class IconTool
    {
        public static void SetSkillIcon(int skillId, Image skillIcon)
        {
            SKILL.Skill skillData;
            if (SkillDataCenter.Instance.TryToGetSkill(skillId, out skillData))
            {
                GUI_Tools.IconTool.SetIcon(skillData.IconAtlas, skillData.IconSprite, skillIcon);
            }
        }

        public static void SetSexIcon(PbCommon.EHeroSexType sexType, Image target)
        {
            SetSexIcon((int)sexType, target);
        }

        public static void SetSexIcon(int sexType, Image target)
        {
            CSV_c_item_descripion sex = CSV_c_item_descripion.FindData(sexType + 1000);
            if (null != sex)
            {
                SetIcon(sex.IconAtlas, sex.IconSprite, target);
            }
        }

        public static void SetHeroTypeIcon(PbCommon.EHeroType heroType, Image target)
        {
            SetHeroTypeIcon((int)heroType, target);
        }

        public static void SetHeroTypeIcon(int heroType, Image target)
        {
            CSV_c_item_descripion typeItem = CSV_c_item_descripion.FindData(heroType + 2000);
            if (null != typeItem)
            {
                SetIcon(typeItem.IconAtlas, typeItem.IconSprite, target);
            }
        }

        public static void SetHeroNationalityIcon(int nationality, Image target)
        {
            CSV_c_item_descripion nationalityItem = CSV_c_item_descripion.FindData(nationality + 3000);
            if (null != nationalityItem)
            {
                SetIcon(nationalityItem.IconAtlas, nationalityItem.IconSprite, target);
            }
        }

        public static void SetShoolIcon(int school, Image target, bool exclusiveWeaponMax)
        {
            CSV_c_school_config schoolConfig = CSV_c_school_config.FindData(school);
            if(null != schoolConfig)
            {
                SetIcon(schoolConfig.Atlas, exclusiveWeaponMax ? schoolConfig.ExclusiveWeaponMaxIcon : schoolConfig.Icon, target);
            }
        }

        public static void SetIcon(string altasName, string iconName, Image target)
        {
            if (null != target)
            {
                GUI_Atlas uia = AssetManage.AM_Manager.LoadAssetSync<GUI_Atlas>("GUI/UIAtlas/" + altasName, true, AssetManage.E_AssetType.GUIAtlas);
#if UNITY_EDITOR
                Debug.Assert(null != uia);
#endif
                if (null != uia)
                {
                    target.sprite = uia.GetSprite(iconName);
                }
            }
        }

        public static void SetItemIcon(PbCommon.EAwardType itemType, Image icon)
        {
            CSV_c_item_descripion item = CSV_c_item_descripion.FindData((int)itemType);
            if (null != item)
            {
                SetIcon(item.IconAtlas, item.IconSprite, icon);
            }
        }
    }

    public class ItemTool
    {
        public static string GetItemStarString(int starCount)
        {
            starCount = Mathf.Clamp(starCount, 1, 6);
            switch(starCount)
            {
                case 1:
                    {
                        return "*";
                    }
                case 2:
                    {
                        return "**";
                    }
                case 3:
                    {
                        return "***";
                    }
                case 4:
                    {
                        return "****";
                    }
                case 5:
                    {
                        return "*****";
                    }
                case 6:
                    {
                        return "******";
                    }
            }
            return null;
        }
        /// <summary>
        /// 根据物品类型（服务器奖励枚举）设置物品图标、个数（星级），面包、武器等需要显示星级的物品要给出csvId
        /// </summary>
        /// <param name="awardType"></param>
        /// <param name="itemInfo"></param>
        /// <param name="countOrId"></param>
        public static void SetAwardItemInfo(int awardType, Text name, Text countOrStar, Image icon, int countOrId = 0)
        {
            SetAwardItemInfo((PbCommon.EAwardType)awardType, name, countOrStar, icon, countOrId);
        }

        /// <summary>
        /// 根据物品类型（服务器奖励枚举）设置物品图标、个数（星级），面包、武器等需要显示星级的物品要给出csvId
        /// </summary>
        /// <param name="awardType"></param>
        /// <param name="itemInfo"></param>
        /// <param name="countOrId"></param>
        public static void SetAwardItemInfo(PbCommon.EAwardType awardType, Text name, Text countOrStar, Image icon, int countOrId = 0)
        {
            switch (awardType)
            {
                case PbCommon.EAwardType.E_Award_Type_Gold_Coin:
                case PbCommon.EAwardType.E_Award_Map_Piece:
                case PbCommon.EAwardType.E_Award_Crystal_Powder:
                case PbCommon.EAwardType.E_Award_Crystal_Piece:
                case PbCommon.EAwardType.E_Award_Crystal_Rime:
                case PbCommon.EAwardType.E_Award_Arena_Ticket:
                case PbCommon.EAwardType.E_Award_Dungeon_Key:
                case PbCommon.EAwardType.E_Award_Iron:
                case PbCommon.EAwardType.E_Award_Type_Diamond:
                case PbCommon.EAwardType.E_Award_Reset_Ticket:
                    {
                        CSV_c_item_descripion item = CSV_c_item_descripion.FindData((int)awardType);
                        if (null != item)
                        {
                            IconTool.SetIcon(item.IconAtlas, item.IconSprite, icon);
                            TextTool.SetText(name, item.Name);
                        }
                        TextTool.SetText(countOrStar, countOrId.ToString());
                        break;
                    }
                case PbCommon.EAwardType.E_Award_Type_Bread:
                    {
                        CSV_b_bread_template breadItem = CSV_b_bread_template.FindData(countOrId);
                        if (null != breadItem)
                        {
                            IconTool.SetIcon(breadItem.IconAtlas, breadItem.BreadIcon, icon);
                            TextTool.SetText(countOrStar, GetItemStarString(breadItem.Star));
                            TextTool.SetText(name, breadItem.Name);
                        }
                        break;
                    }
                case PbCommon.EAwardType.E_Award_Type_Equip:
                    {
                        CSV_b_equip_template equipItem = CSV_b_equip_template.FindData(countOrId);
                        if (null != equipItem)
                        {
                            IconTool.SetIcon(equipItem.IconAtlas, equipItem.IconSprite, icon);
                            TextTool.SetText(countOrStar, GetItemStarString(equipItem.Star));
                            TextTool.SetText(name, equipItem.Name);
                        }
                        break;
                    }
                case PbCommon.EAwardType.E_Award_Type_Fruit:
                    {
                        CSV_b_fruit_template fruitItem = CSV_b_fruit_template.FindData(countOrId);
                        if (null != fruitItem)
                        {
                            IconTool.SetIcon(fruitItem.IconAtlas, fruitItem.IconSrite, icon);
                            TextTool.SetText(countOrStar, "");
                            TextTool.SetText(name, fruitItem.Name);
                        }
                        break;
                    }
            }
        }
    }

    public class CommonTool
    {
        public static GameObject AddChild(GameObject parent, GameObject go)
        {
            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.parent = parent.transform;
                //go.layer = parent.layer;
            }
            return go;
        }

        public static GameObject AddUIChild(GameObject parent, GameObject go, GUI_Transform uiTransform)
        {
            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.parent = parent.transform;
                if (null != uiTransform)
                {
                    t.localPosition = uiTransform.Position;
                    t.localRotation = Quaternion.Euler(uiTransform.Rotation);
                    t.localScale = uiTransform.Scale;
                }
                else
                {
                    t.localPosition = Vector3.zero;
                    t.localRotation = Quaternion.identity;
                    t.localScale = Vector3.one;
                }
            }
            return go;
        }

        public static GameObject AddUIChild(GameObject parent, GameObject go, bool worldPositionStays)
        {
            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.SetParent(parent.transform, worldPositionStays);
                t.localPosition = new Vector3(0f, 0f, t.localPosition.z);
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parent.layer;
            }
            return go;
        }
        /// <summary>
        /// Finds the transform under the parent given.
        /// </summary>
        /// <returns>The transform.</returns>
        /// <param name="target">Target (can be path : child/child/child).</param>
        /// <param name="parentToSearch">Parent to search.</param>
        public static Transform FindTransform(string target, GameObject parentToSearch)
        {
            if (parentToSearch == null)
            {
                return null;
            }
            return parentToSearch.transform.Find(target);
        }
        /// <summary>
        /// Finds the object under the parent given.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="target">Target (can be path : child/child/child).</param>
        /// <param name="parentToSearch">Parent to search.</param>
        public static GameObject FindObject(string target, GameObject parentToSearch)
        {
            Transform ts = FindTransform(target, parentToSearch);
            if (ts != null)
            {
                return ts.gameObject;
            }
            return null;
        }
        /// <summary>
        /// Gets the component by given type.
        /// </summary>
        /// <returns>The component.</returns>
        /// <param name="target">Target (can be path : child/child/child).</param>
        /// <typeparam name="TargetComponent">The given type parameter, must be a MonoBehaviour type.</typeparam>
        public static TargetComponent GetComponent<TargetComponent>(string target) where TargetComponent : MonoBehaviour
        {
            TargetComponent comp = null;
            GameObject go = GameObject.Find(target);
            if (go != null)
            {
                comp = go.GetComponent<TargetComponent>();
            }
            return comp;
        }
        /// <summary>
        /// Gets the component by given type.
        /// </summary>
        /// <returns>The component.</returns>
        /// <param name="target">Target (can be path : child/child/child).</param>
        /// <param name="parentToSearch">Parent to search.</param>
        /// <typeparam name="TargetComponent">The given type parameter,must be a MonoBehaviour type.</typeparam>
        public static TargetComponent GetComponent<TargetComponent>(string target, GameObject parentToSearch) where TargetComponent : MonoBehaviour
        {
            TargetComponent tg = null;
            Transform ts = FindTransform(target, parentToSearch);
            if (ts != null)
            {
                tg = ts.gameObject.GetComponent<TargetComponent>();
            }
            return tg;
        }

        public static string GetClassNameWithoutNameSpace<ClassType>() where ClassType : class
        {
            string name = typeof(ClassType).FullName;
            int pos = name.LastIndexOf(".");
            name = name.Substring(pos + 1);
            return name;
        }
    }

    public class TextTool
    {
        public static void SetHour(Text text, int hour)
        {
            if(null != text)
            {
                string hourStr;
                if (TextLocalization.GetText(TextId.Hour, out hourStr))
                {
                    text.text = hour.ToString() + hourStr;
                }
                else
                {
                    text.text = hour.ToString();
                }
            }
        }

        public static void SetMinute(Text text, int minute)
        {
            if (null != text)
            {
                string minuteStr;
                if (TextLocalization.GetText(TextId.Minute, out minuteStr))
                {
                    text.text = minute.ToString() + minuteStr;
                }
                else
                {
                    text.text = minute.ToString();
                }
            }
        }

        public static void SetText(Text text, string str)
        {
            if (null != text)
            {
                text.text = str;
            }
        }

        public static string GetSchoolText(PbCommon.ESchoolType school)
        {
            string text = null;
            switch (school)
            {
                case PbCommon.ESchoolType.E_School_Sword:
                    {
                        TextLocalization.GetText(TextId.School_Sword, out text);
                        break;
                    }
                case PbCommon.ESchoolType.E_School_Knight:
                    {
                        TextLocalization.GetText(TextId.School_Knight, out text);
                        break;
                    }
                case PbCommon.ESchoolType.E_School_Archer:
                    {
                        TextLocalization.GetText(TextId.School_Archer, out text);
                        break;
                    }
                case PbCommon.ESchoolType.E_School_Hunter:
                    {
                        TextLocalization.GetText(TextId.School_Hunter, out text);
                        break;
                    }
                case PbCommon.ESchoolType.E_School_Wizard:
                    {
                        TextLocalization.GetText(TextId.School_Wizard, out text);
                        break;
                    }
                case PbCommon.ESchoolType.E_School_Flamen:
                    {
                        TextLocalization.GetText(TextId.School_Flamen, out text);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return text;
        }

        public static string GetDifficultText(E_GameLevel_Difficulty difficult)
        {
            string text = null;
            switch (difficult)
            {
                case E_GameLevel_Difficulty.Easy:
                    {
                        TextLocalization.GetText(TextId.Difficulty_Easy, out text);
                        break;
                    }
                case E_GameLevel_Difficulty.Normal:
                    {
                        TextLocalization.GetText(TextId.Difficulty_Normal, out text);
                        break;
                    }
                case E_GameLevel_Difficulty.Difficult:
                    {
                        TextLocalization.GetText(TextId.Difficulty_Difficult, out text);
                        break;
                    }
            }
            return text;
        }

        public static string GetReformTextFormater(PbCommon.EPropertyType type)
        {
            string formater = "";
            switch (type)
            {
                case PbCommon.EPropertyType.E_Damage_Add_Value:
                    {
                        TextLocalization.GetText("Reform_Attack_Add_Value", out formater);
                        break;
                    }
                case PbCommon.EPropertyType.E_Crit_Damage_Add_Percent:
                    {
                        TextLocalization.GetText("Reform_Crite_Damage", out formater);
                        break;
                    }
                case PbCommon.EPropertyType.E_Physical_Penetration_Add_Value:
                    {
                        TextLocalization.GetText("Reform_Physical_Penetration", out formater);
                        break;
                    }
                case PbCommon.EPropertyType.E_Magical_Penetration_Add_Value:
                    {
                        TextLocalization.GetText("Reform_Magical_Panetration", out formater);
                        break;
                    }
                case PbCommon.EPropertyType.E_Damage_Add_Percent:
                    {
                        TextLocalization.GetText("Reform_Attack_Add_Percent", out formater);
                        break;
                    }
                case PbCommon.EPropertyType.E_HP_Add_Value:
                    {
                        TextLocalization.GetText("Reform_Hero_Hp_Add_Value", out formater);
                        break;
                    }
                case PbCommon.EPropertyType.E_Be_Damage_Reduce_Percent:
                    {
                        TextLocalization.GetText("Reform_Reduce_Damage_Percent", out formater);
                        break;
                    }
                case PbCommon.EPropertyType.E_Physical_Defense_Add_Value:
                    {
                        TextLocalization.GetText("Reform_Physical_Defense_Add_Value", out formater);
                        break;
                    }
                case PbCommon.EPropertyType.E_Magical_Defense_Add_Value:
                    {
                        TextLocalization.GetText("Reform_Magical_Defense_Add_Value", out formater);
                        break;
                    }
                case PbCommon.EPropertyType.E_HP_Add_Percent:
                    {
                        TextLocalization.GetText("Reform_Hero_Hp_Add_Percent", out formater);
                        break;
                    }
                case PbCommon.EPropertyType.E_Crit_Rate_Add_Percent:
                    {
                        TextLocalization.GetText("Reform_Crit_Rate_Add_Percent", out formater);
                        break;
                    }
                case PbCommon.EPropertyType.E_Attack_Speed_Add_Percent:
                    {
                        TextLocalization.GetText("Reform_Attack_Speed_Add_Percent", out formater);
                        break;
                    }
                case PbCommon.EPropertyType.E_Suck_Rate_Add_Percent:
                    {
                        TextLocalization.GetText("Reform_Suck_Rate_Add_Percent", out formater);
                        break;
                    }
                case PbCommon.EPropertyType.E_Evasion_Rate_Add_Percent:
                    {
                        TextLocalization.GetText("Reform_Dodge_Rate_Add_Percent", out formater);
                        break;
                    }
                case PbCommon.EPropertyType.E_Hit_Add_Percent:
                    {
                        TextLocalization.GetText("Reform_Precision_Add_Percent", out formater);
                        break;
                    }
                default:
                    {
                        TextLocalization.GetText("Reform_Non", out formater);
                        break;
                    }
            }
            return formater;
        }

        public static string GetWordPropertyText(PbCommon.EReformType type)
        {
            string word = "";
            switch (type)
            {
                case PbCommon.EReformType.E_Refine_Type_Damage:
                    {
                        TextLocalization.GetText(TextId.Reform_Attack_Text, out word);
                        break;
                    }
                case PbCommon.EReformType.E_Refine_Type_Defense:
                    {
                        TextLocalization.GetText(TextId.Reform_Defend_Text, out word);
                        break;
                    }
                case PbCommon.EReformType.E_Refine_Type_Capacity:
                    {
                        TextLocalization.GetText(TextId.Reform_Function_Text, out word);
                        break;
                    }
            }
            return word;
        }
    }
}