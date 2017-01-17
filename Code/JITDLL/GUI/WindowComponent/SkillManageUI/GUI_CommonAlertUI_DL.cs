using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GUI_CommonAlertUI_DL : GUI_Window_DL {
    Action OnConfirm;
    Action OnCancel;

    void OnConfirmButtonClicked()
    {
        if(null != OnConfirm)
        {
            OnConfirm();
        }
        HideWindow();
    }

    void OnCancelButtonClicked()
    {
        if(null != OnCancel)
        {
            OnCancel();
        }
        HideWindow();
    }

    public void Alert(Action onConfirm, Action onCancel)
    {
        OnConfirm = onConfirm;
        OnCancel = onCancel;
    }


    protected override void CopyDataFromDataScript()
    {
        base.CopyDataFromDataScript();
        GUI_CommonAlertUI dataComponent = gameObject.GetComponent<GUI_CommonAlertUI>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_CommonAlertUI,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            dataComponent.ConfirmButton.onClick.AddListener(OnConfirmButtonClicked);
            dataComponent.CancelButton.onClick.AddListener(OnCancelButtonClicked);
        }
    }
}
