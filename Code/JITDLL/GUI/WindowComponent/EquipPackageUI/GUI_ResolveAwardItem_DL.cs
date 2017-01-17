using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_ResolveAwardItem_DL : GUI_LogicObject {

    #region jit init
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_ResolveAwardItem dataComponent = gameObject.GetComponent<GUI_ResolveAwardItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ResolveAwardItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            ItemIcon = dataComponent.ItemIcon;
            ItemCount = dataComponent.ItemCount;
            AdditionalIcon = dataComponent.AdditionalIcon;
            IronCountFormater = dataComponent.IronCountFormater;
            CrystalPowderCountFormater = dataComponent.CrystalPowderCountFormater;
            CrystalPieceCountFormater = dataComponent.CrystalPieceCountFormater;
            CrystalRimeCountFormater = dataComponent.CrystalRimeCountFormater;
        }
    }
    #endregion

    #region logic area
    public Image ItemIcon;
    public Text ItemCount;
    public GameObject AdditionalIcon;
    public string IronCountFormater;
    public string CrystalPowderCountFormater;
    public string CrystalPieceCountFormater;
    public string CrystalRimeCountFormater;
    DataCenter.AwardInfo AwardItem;
    bool BigSuccess;

    public void ShowAward(DataCenter.AwardInfo award, bool bigSuccess)
    {
        AwardItem = award;
        if(null == AwardItem)
        {
            Recycle();
        }
    }

    void Start()
    {
        AdditionalIcon.SetActive(BigSuccess);
        switch((PbCommon.EAwardType)AwardItem.AwardType)
        {
            case PbCommon.EAwardType.E_Award_Iron:
                {
                    GUI_Tools.IconTool.SetItemIcon(PbCommon.EAwardType.E_Award_Iron, ItemIcon);
                    ItemCount.text = string.Format(IronCountFormater, AwardItem.AwardValue.ToString());
                    break;
                }
            case PbCommon.EAwardType.E_Award_Crystal_Powder:
                {
                    GUI_Tools.IconTool.SetItemIcon(PbCommon.EAwardType.E_Award_Crystal_Powder, ItemIcon);
                    ItemCount.text = string.Format(CrystalPowderCountFormater, AwardItem.AwardValue.ToString());
                    break;
                }
            case PbCommon.EAwardType.E_Award_Crystal_Piece:
                {
                    GUI_Tools.IconTool.SetItemIcon(PbCommon.EAwardType.E_Award_Crystal_Piece, ItemIcon);
                    ItemCount.text = string.Format(CrystalPieceCountFormater, AwardItem.AwardValue.ToString());
                    break;
                }
            case PbCommon.EAwardType.E_Award_Crystal_Rime:
                {
                    GUI_Tools.IconTool.SetItemIcon(PbCommon.EAwardType.E_Award_Crystal_Rime, ItemIcon);
                    ItemCount.text = string.Format(CrystalRimeCountFormater, AwardItem.AwardValue.ToString());
                    break;
                }
        }
    }

    protected override void OnRecycle()
    {
        AwardItem = null;
        BigSuccess = false;
    }
    #endregion
}
