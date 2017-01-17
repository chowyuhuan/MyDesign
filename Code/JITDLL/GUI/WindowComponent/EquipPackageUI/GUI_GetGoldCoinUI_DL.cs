using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_GetGoldCoinUI_DL : GUI_Window_DL {
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_GetGoldCoinUI dataComponent = gameObject.GetComponent<GUI_GetGoldCoinUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_GetGoldCoinUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            GoldCoinCount = dataComponent.GoldCoinCount;
        }
    }
    #endregion

    #region window logic
    public Text GoldCoinCount;
    uint Count;
    public void GetGoldCoin(uint count)
    {
        Count = count;        
    }

    protected override void OnStart()
    {
        GoldCoinCount.text = Count.ToString();
    }
    #endregion
}
