using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_SpecialSkillDisplayItem_DL : GUI_ToggleItem_DL {

    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_SpecialSkillDisplayItem dataComponent = gameObject.GetComponent<GUI_SpecialSkillDisplayItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_SepcialSkillDisplayItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            SkillSimpleInfo = dataComponent.SkillSimpleInfo;
            SkillType = dataComponent.SkillType;
            SkillName = dataComponent.SkillName;
            ConstrainedHeroStar = dataComponent.ConstrainedHeroStar;
            AttackIntentType = dataComponent.AttackIntentType;
            Description = dataComponent.Description;
            CostHonor = dataComponent.CostHonor;
            CostGold = dataComponent.CostGold;
            LevelCondtion = dataComponent.LevelCondtion;
            LevelDescription = dataComponent.LevelDescription;

            GetSkillText = dataComponent.GetSkillText;
            GetSkillButton = dataComponent.GetSkillButton;
            LockMask = dataComponent.LockMask;
            ShrinkInfo = dataComponent.ShrinkInfo;
           
            dataComponent.GetConditionButton.onClick.AddListener(OnGetConditionButtonClicked);
            dataComponent.GetSkillButton.onClick.AddListener(OnGetSkillButtonClicked);
        }
    }
    #endregion

    #region item logic
    public GUI_SkillSimpleInfo SkillSimpleInfo;
    public Text SkillType;
    public Text SkillName;
    public Text ConstrainedHeroStar;
    public Text AttackIntentType;
    public Text Description;
    public Text CostHonor;
    public Text CostGold;
    public Text LevelCondtion;
    public Text LevelDescription;
    public Button GetConditionButton;
    public Button GetSkillButton;
    public Text GetSkillText;
    public GameObject LockMask;

    public DataCenter.SpecialSkillUnit SpecialSkillUnit{get; protected set;}
    CSV_b_skill_template SkillTemplate;
    CSV_b_skill_template NextSkillTemplate;

    public delegate void SelectSkill(GUI_SpecialSkillDisplayItem_DL skillDisplayItem);
    SelectSkill OnSelectSkill;

    public delegate DataCenter.Hero GetHero();
    GetHero GetCurrentHero;

    public void DisplaySkill(GUI_ScrollItem bindItem, DataCenter.SpecialSkillUnit specialSkill, SelectSkill onSelectSkill, GetHero getCurrentHero)
    {
        BindItem = bindItem;
        BindItem.ItemLayout.preferredHeight = ShrinkInfo.ShrinkHeight;
        ShrinkInfo.ShrinkTarget.SetActive(false);
        SpecialSkillUnit = specialSkill;
        OnSelectSkill = onSelectSkill;
        GetCurrentHero = getCurrentHero;
        RefreshSkillInfo();
        RegistEvent();
    }

    public void RefreshSkillInfo()
    {
        RefreshSkillTemplate();
        RefreshSkillType();
        RefreshSkillSimpleInfo();
        RefreshLevelCondition();
        RefreshButtonState();
    }

    void RefreshSkillTemplate()
    {
        if (null != SpecialSkillUnit)
        {
            SkillTemplate = CSV_b_skill_template.FindData(SpecialSkillUnit.ShowSkillId);
            if(SpecialSkillUnit.Level == 0 || SpecialSkillUnit.Level == SpecialSkillUnit.MaxLevel)
            {
                NextSkillTemplate = SkillTemplate;
            }
            else
            {
                NextSkillTemplate = CSV_b_skill_template.FindData((int)SpecialSkillUnit.GroupId, (int)SpecialSkillUnit.Level + 1);
            }
        }
    }

    void RefreshSkillType()
    {
        if(null != SkillTemplate)
        {
            string skillType;
            if (SkillTemplate.Type == 1)
            {
                TextLocalization.GetText("Special_Skill", out skillType);
            }
            else
            {
                TextLocalization.GetText("Special_Skill", out skillType);
            }
            SkillType.text = skillType;
        }
    }

    void RefreshSkillSimpleInfo()
    {
        if(null != SkillTemplate && null != SpecialSkillUnit)
        {
            GUI_Tools.IconTool.SetSkillIcon(SkillTemplate.Id, SkillSimpleInfo.Icon);
            SkillSimpleInfo.Level.text = string.Format("{0}/{1}", SkillTemplate.Level.ToString(), SpecialSkillUnit.MaxLevel.ToString());
            SkillName.text = SkillTemplate.Name;
            ConstrainedHeroStar.text = SkillTemplate.EquipedRequireStar.ToString();
            AttackIntentType.text = SkillTemplate.IntentEffect;
            Description.text = SkillTemplate.Description;
            CostHonor.text = SkillTemplate.EquipCostHonorNum.ToString();
            CostGold.text = SkillTemplate.EquipCostGoldNum.ToString();
        }
    }

    void RefreshLevelCondition()
    {
        if (null != SkillTemplate && null != SpecialSkillUnit)
        {
            string condtion;
            if(SkillTemplate.Level == SpecialSkillUnit.MaxLevel - 1)
            {
                if (TextLocalization.GetText("HighestLevel_Condition_With_Colons", out condtion))
                {
                    LevelCondtion.text = condtion;
                }
                if (TextLocalization.GetText("Get_Special_Skill_Big_Success", out condtion))
                {
                    LevelDescription.text = condtion;
                }
            }
            else 
            {
                if (TextLocalization.GetText("LevelUp_Condition_With_Colons", out condtion))
                {
                    LevelCondtion.text = condtion;
                }
                LevelDescription.text = NextSkillTemplate.ConditionDescription;
            }
        }
    }

    void RefreshButtonState()
    {
        if(null != SpecialSkillUnit)
        {
            switch(SpecialSkillUnit.State)
            {
                case DataCenter.SpecialSkillUnit.SkillState.NotAcquired:
                    {
                        GetSkillButton.interactable = false;
                        GetSkillText.text = "huoqu";
                        LockMask.SetActive(true);
                        break;
                    }
                case DataCenter.SpecialSkillUnit.SkillState.CanUpgrade:
                    {
                        GetSkillButton.interactable = true;
                        GetSkillText.text = SpecialSkillUnit.Level == 0 ? "huoqu" : "shengji";
                        LockMask.SetActive(false);
                        break;
                    }
                case DataCenter.SpecialSkillUnit.SkillState.Acquired:
                    {
                        GetSkillButton.interactable = true;
                        GetSkillText.text = "lingwu";
                        LockMask.SetActive(false);
                        break;
                    }
            }
        }
    }

    void OnGetConditionButtonClicked()
    {
        GUI_MessageManager.Instance.ShowErrorTip(10001);
    }

    void OnGetSkillButtonClicked()
    {
        if (null != SpecialSkillUnit)
        {
            switch (SpecialSkillUnit.State)
            {
                case DataCenter.SpecialSkillUnit.SkillState.NotAcquired:
                    {
                        GUI_MessageManager.Instance.ShowErrorTip("满足条件可以获得技能");
                        break;
                    }
                case DataCenter.SpecialSkillUnit.SkillState.CanUpgrade:
                    {
                        FetchOrUpgradeSkill();
                        break;
                    }
                case DataCenter.SpecialSkillUnit.SkillState.Acquired:
                    {
                        GetSkill();
                        break;
                    }
            }
        }
    }

    void FetchOrUpgradeSkill()
    {
        if (null != SpecialSkillUnit)
        {
            gsproto.AcquireSkillReq req = new gsproto.AcquireSkillReq();
            req.session_id = DataCenter.PlayerDataCenter.SessionId;
            req.skill_gid = SpecialSkillUnit.GroupId;
            req.skill_glevel = SpecialSkillUnit.Level + 1;
            Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
        }
    }

    void RegistEvent()
    {
        DataCenter.PlayerDataCenter.OnAcquireSkill += OnFetchOrUpgradeSkillRsp;
    }

    void UnRegistEvent()
    {
        DataCenter.PlayerDataCenter.OnAcquireSkill -= OnFetchOrUpgradeSkillRsp;
    }

    void OnFetchOrUpgradeSkillRsp(uint groupId)
    {
        if(null != SpecialSkillUnit && groupId == SpecialSkillUnit.GroupId)
        {
            RefreshSkillInfo();
            GUI_MessageManager.Instance.ShowErrorTip("获得技能成功:" + SkillTemplate.Name);
            GUI_FetchNewSkillUI_DL fetchNewSkill = GUI_Manager.Instance.ShowWindowWithName<GUI_FetchNewSkillUI_DL>("UI_HeroNewSkill_Message", false);
            if(null != fetchNewSkill)
            {
                DataCenter.SpecialSkillUnit specialSkill = DataCenter.PlayerDataCenter.SpecialSkillData.GetSpecialSkill(groupId);
                fetchNewSkill.ShowNewSkill(specialSkill);
            }
        }
    }

    void GetSkill()
    {
        if(null != GetCurrentHero)
        {
            DataCenter.Hero hero = GetCurrentHero();
            if(null != hero)
            {
                if(hero.SkillServerId > 0)
                {
                    GUI_CommonAlertUI_DL coverAlert = GUI_Manager.Instance.ShowWindowWithName<GUI_CommonAlertUI_DL>("UI_HeroSkill_lingwuConfirm", false);
                    if(null != coverAlert)
                    {
                        coverAlert.Alert(OnConfirmGetSkill, null);
                    }
                }
                else
                {
                    DoGetSkill(hero);
                }
            }
        }
    }

    void OnConfirmGetSkill()
    {
         if(null != GetCurrentHero)
         {
             DoGetSkill(GetCurrentHero());
         }
    }

    void DoGetSkill(DataCenter.Hero hero)
    {
        if(null != hero)
        {
            CSV_b_skill_template curLevel = CSV_b_skill_template.FindData(SpecialSkillUnit.ShowSkillId);

            if (null != curLevel)
            {
                gsproto.EquipSkillReq req = new gsproto.EquipSkillReq();
                req.session_id = DataCenter.PlayerDataCenter.SessionId;
                req.skill_gid = SpecialSkillUnit.GroupId;
                req.skill_glevel = (uint)curLevel.Level;
                req.hero_id = hero.ServerId;
                Network.NetworkManager.SendRequest(Network.ProtocolDataType.TcpShort, req);
            }
        }
    }
    #endregion

    #region toggle logic
    GUI_ScrollItem BindItem;
    public GUI_ItemShrinkInfo ShrinkInfo;

    protected override void OnSelected()
    {
        if (null != BindItem)
        {
            if(null != OnSelectSkill)
            {
                OnSelectSkill(this);
            }
            StartCoroutine("ExtendItemHeight");
        }
    }

    IEnumerator ExtendItemHeight()
    {
        ShrinkInfo.ShrinkTarget.SetActive(true);
        ShrinkInfo.ExtendingItem = true;
        int extendCounter = 0;
        while (BindItem.ItemLayout.preferredHeight < ShrinkInfo.ExtendHeight)
        {
            float oldHeight = BindItem.ItemLayout.preferredHeight;
            BindItem.ItemLayout.preferredHeight = Mathf.Lerp(ShrinkInfo.ShrinkHeight, ShrinkInfo.ExtendHeight, ShrinkInfo.ItemExtendRate * extendCounter);
            ++extendCounter;
            CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, CachedTransform.localPosition.y + (BindItem.ItemLayout.preferredHeight - oldHeight) / 2, CachedTransform.localPosition.z);
            BindItem.SetDirty();
            BindItem.FocusOn();
            yield return null;
        }
        BindItem.ItemLayout.preferredHeight = ShrinkInfo.ExtendHeight;
        CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x,(ShrinkInfo.ExtendHeight - ShrinkInfo.ShrinkHeight) / 2, CachedTransform.localPosition.z);
        BindItem.SetDirty();       
        ShrinkInfo.ExtendingItem = false;
        BindItem.FocusOn();
    }

    protected override void OnDeSelected()
    {
        if (null != BindItem)
        {
            StartCoroutine("ShrinkItemHeight");
        }
    }

    IEnumerator ShrinkItemHeight()
    {
        ShrinkInfo.ShrinkingItem = true;
        int ShrinkCounter = 0;
        while (BindItem.ItemLayout.preferredHeight > ShrinkInfo.ShrinkHeight)
        {
            float oldHeight = BindItem.ItemLayout.preferredHeight;
            BindItem.ItemLayout.preferredHeight = Mathf.Lerp(ShrinkInfo.ExtendHeight, ShrinkInfo.ShrinkHeight, ShrinkInfo.ItemExtendRate * ShrinkCounter);
            ++ShrinkCounter;
            CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, CachedTransform.localPosition.y - (oldHeight - BindItem.ItemLayout.preferredHeight) / 2, CachedTransform.localPosition.z);
            BindItem.SetDirty();
            BindItem.FocusOn();
            yield return null;
        }
        BindItem.ItemLayout.preferredHeight = ShrinkInfo.ShrinkHeight;
        CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, 0f , CachedTransform.localPosition.z);
        ShrinkInfo.ShrinkingItem = false;
        BindItem.SetDirty();
        BindItem.FocusOn();
        ShrinkInfo.ShrinkTarget.SetActive(false);
    }

    public override void RefreshObject()
    {
        RefreshSkillInfo();
    }

    protected override void OnRecycle()
    {
        UnRegistEvent();
        if (ShrinkInfo.ExtendingItem)
        {
            StopCoroutine("ExtendItemHeight");
            ShrinkInfo.ExtendingItem = false;
            ShrinkInfo.ShrinkTarget.SetActive(false);
        }
        if (ShrinkInfo.ShrinkingItem)
        {
            StopCoroutine("ShrinkItemHeight");
            ShrinkInfo.ShrinkingItem = false;
            ShrinkInfo.ShrinkTarget.SetActive(false);
        }
    }
    #endregion
}
