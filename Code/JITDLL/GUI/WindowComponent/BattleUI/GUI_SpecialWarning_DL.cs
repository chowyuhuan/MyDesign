using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ESpecialWarningType
{
    Boss = 1,
    Hidden = 2,
    PassLevel = 3,
    FailLevel = 4,
}
public class GUI_SpecialWarning_DL : MonoBehaviour
{
    public float BossWarningTime;
    public GameObject BossWarning;
    public float HiddenWarningTime;
    public GameObject HiddenWarning;
    public float PassLevelEffectTime;
    public GameObject PassLevelEffect;
    public float FailLevelEffectTime;
    public GameObject FailLevelEffect;

    public void Warning(ESpecialWarningType type)
    {
        switch (type)
        {
            case ESpecialWarningType.Boss:
                {
                    BossWarning.SetActive(true);
                    Invoke("BossWarnEnd", BossWarningTime);
                    break;
                }
            case ESpecialWarningType.Hidden:
                {
                    HiddenWarning.SetActive(true);
                    Invoke("HiddenWarnEnd", HiddenWarningTime);
                    break;
                }
            case ESpecialWarningType.PassLevel:
                {
                    PassLevelEffect.SetActive(true);
                    Invoke("PassLevelEffectEnd", PassLevelEffectTime);
                    break;
                }
            case ESpecialWarningType.FailLevel:
                {
                    FailLevelEffect.SetActive(true);
                    Invoke("FailLevelEffectEnd", FailLevelEffectTime);
                    break;
                }
        }
    }

    void BossWarnEnd()
    {
        BossWarning.SetActive(false);
    }

    void HiddenWarnEnd()
    {
        HiddenWarning.SetActive(false);
    }

    void PassLevelEffectEnd()
    {
        PassLevelEffect.SetActive(false);
        GUI_Manager.Instance.ShowWindowWithName("PassLevelUI", false);
    }

    void FailLevelEffectEnd()
    {
        FailLevelEffect.SetActive(false);
        GUI_Manager.Instance.ShowWindowWithName("FailLevelUI", false);
    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_SpecialWarning dataComponent = gameObject.GetComponent<GUI_SpecialWarning>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_SpecialWarning,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            BossWarningTime = dataComponent.BossWarningTime;
            BossWarning = dataComponent.BossWarning;
            HiddenWarningTime = dataComponent.HiddenWarningTime;
            HiddenWarning = dataComponent.HiddenWarning;
            PassLevelEffectTime = dataComponent.PassLevelEffectTime;
            PassLevelEffect = dataComponent.PassLevelEffect;
            FailLevelEffectTime = dataComponent.FailLevelEffectTime;
            FailLevelEffect = dataComponent.FailLevelEffect;
        }
    }
}
