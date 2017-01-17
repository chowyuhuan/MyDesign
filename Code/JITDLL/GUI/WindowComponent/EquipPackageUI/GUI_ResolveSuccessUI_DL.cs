using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class GUI_ResolveSuccessUI_DL : GUI_Window_DL {

    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ResolveSuccessUI dataComponent = gameObject.GetComponent<GUI_ResolveSuccessUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ResolveSuccessUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            SuccessTitle = dataComponent.SuccessTitle;
            SuccessString = dataComponent.SuccessString;
            BigSuccessString = dataComponent.BigSuccessString;
            AwardItemSpawnRoot = dataComponent.AwardItemSpawnRoot;
            dataComponent.ConfirmSuccessButton.onClick.AddListener(HideWindow);
        }
    }
    #endregion

    #region window logic
    public Text SuccessTitle;
    public string SuccessString;
    public string BigSuccessString;
    public GameObject AwardItemSpawnRoot;
    List<DataCenter.AwardInfo> AwardInfoList;
    List<DataCenter.AwardInfo> ExtraAwardList;
    bool BigSuccesss;
    GUI_LogicObjectPool AwardItemPool;

    public void ResovleSuccess(List<DataCenter.AwardInfo> awardList, List<DataCenter.AwardInfo> extraAwardList, bool isBigSuccess)
    {
        AwardInfoList = awardList;
        ExtraAwardList = extraAwardList;
        BigSuccesss = isBigSuccess;
        if(null == awardList)
        {
            HideWindow();
        }
    }

    protected override void OnStart()
    {
        SuccessTitle.text = BigSuccesss ? BigSuccessString : SuccessString;
        GameObject go = AssetManage.AM_Manager.LoadAssetSync<GameObject>("GUI/UIPrefab/DecomposeSuccess_Item", true, AssetManage.E_AssetType.UIPrefab);
        AwardItemPool = new GUI_LogicObjectPool(go);
        DisplayAwardInfo(AwardInfoList, false);
        DisplayAwardInfo(ExtraAwardList, true);
    }

    void DisplayAwardInfo(List<DataCenter.AwardInfo> awardList, bool extra)
    {
        for (int index = 0; index < awardList.Count; ++index)
        {
            GUI_ResolveAwardItem_DL awardItem = AwardItemPool.GetOneLogicComponent() as GUI_ResolveAwardItem_DL;
            awardItem.ShowAward(awardList[index], extra);
            GUI_Tools.CommonTool.AddUIChild(AwardItemSpawnRoot, awardItem.CachedGameObject, false);
        }
    }
    #endregion
}
