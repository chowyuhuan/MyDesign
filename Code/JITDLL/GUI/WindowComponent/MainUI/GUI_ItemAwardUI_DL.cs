using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_ItemAwardUI_DL : GUI_Window_DL
{
    #region window logic
    public Text ItemName;
    public GUI_ItemSimpleInfo ItemInfo;

    PbCommon.EAwardType AwardType;
    int CountOrCsvId;

    public void ShowAwardItem(PbCommon.EAwardType awardType, int itemCountOrCsvId)
    {
        AwardType = awardType;
        CountOrCsvId = itemCountOrCsvId;
    }

    protected override void OnStart()
    {
        GUI_Tools.ItemTool.SetAwardItemInfo(AwardType, ItemName, ItemInfo.Name_Star_Count, ItemInfo.Icon, CountOrCsvId);
    }
    #endregion

    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_ItemAwardUI dataComponent = gameObject.GetComponent<GUI_ItemAwardUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ItemAwardUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            ItemName = dataComponent.ItemName;
            ItemInfo = dataComponent.ItemInfo;
        }
    }
    #endregion
}
