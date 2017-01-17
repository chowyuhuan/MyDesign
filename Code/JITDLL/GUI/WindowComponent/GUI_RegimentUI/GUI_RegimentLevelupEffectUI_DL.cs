using UnityEngine;
using System.Collections;

public sealed class GUI_RegimentLevelupEffectUI_DL : GUI_Window_DL
{
    #region jit init
    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_RegimentLevelupEffectUI dataComponent = gameObject.GetComponent<GUI_RegimentLevelupEffectUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_RegimentLevelupEffectUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
        }
    }
    #endregion
}
