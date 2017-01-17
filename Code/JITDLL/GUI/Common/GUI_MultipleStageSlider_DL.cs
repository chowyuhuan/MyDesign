using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_MultipleStageSlider_DL : MonoBehaviour
{
    public ESliderStage StageType;
    public RectTransform StageContainer;
    public List<Image> StageImage;
    bool _InitDone = false;
    List<RectTransform> _StageRect = new List<RectTransform>();

    public void SetStageData(ESliderStage stageType, int maxValue, int stage1, int stage2 = 0, int stage3 = 0)
    {
        SetStageData(stageType, (float)maxValue, (float)stage1, (float)stage2, (float)stage3);
    }

    public void SetStageData(ESliderStage stageType, float maxValue, float stage1, float stage2 = 0, float stage3 = 0)
    {
        Init();
        switch (stageType)
        {
            case ESliderStage.Single:
                {
                    SingleStage(maxValue, stage1);
                    break;
                }
            case ESliderStage.Double:
                {
                    DoubleStage(maxValue, stage1, stage2);
                    break;
                }
            case ESliderStage.Trible:
                {
                    TribleStage(maxValue, stage1, stage2, stage3);
                    break;
                }
        }
    }

    void Init()
    {
        if (!_InitDone)
        {
            _InitDone = true;

#if UNITY_EDITOR
            Debug.Assert(StageImage.Count == (int)StageType);
#endif

            for (int index = 0; index < StageImage.Count; ++index)
            {
                _StageRect.Add(StageImage[index].rectTransform);
            }
        }
    }

    void SingleStage(int maxValue, int stageValue)
    {
        SingleStage((float)maxValue, (float)stageValue);
    }

    void SingleStage(float maxValue, float stageValue)
    {
        FillStage(0, maxValue, Mathf.Clamp(stageValue, 0, maxValue));
        FillStage(1, maxValue, 0);
        FillStage(2, maxValue, 0);
    }

    void DoubleStage(int maxValue, int stage1Value, int stage2Value)
    {
        DoubleStage((float)maxValue, (float)stage1Value, (float)stage2Value);
    }

    void DoubleStage(float maxValue, float stage1Value, float stage2Value)
    {
        stage1Value = Mathf.Clamp(stage1Value, 0, maxValue);
        stage2Value = Mathf.Clamp(stage2Value, 0, maxValue - stage1Value);
        FillStage(0, maxValue, stage1Value);
        FillStage(1, maxValue, stage2Value);
        FillStage(2, maxValue, 0);
    }

    void TribleStage(int maxValue, int stage1Value, int stage2Value, int stage3Value)
    {
        TribleStage((float)maxValue, (float)stage1Value, (float)stage2Value, (float)stage3Value);
    }

    void TribleStage(float maxValue, float stage1Value, float stage2Value, float stage3Value)
    {
        stage1Value = Mathf.Clamp(stage1Value, 0, maxValue);
        stage2Value = Mathf.Clamp(stage2Value, 0, maxValue - stage1Value);
        stage3Value = Mathf.Clamp(stage3Value, 0, maxValue - stage1Value - stage2Value);
        FillStage(0, maxValue, stage1Value);
        FillStage(1, maxValue, stage2Value);
        FillStage(2, maxValue, stage3Value);
    }

    void FillStage(int stage, int maxValue, int stageValue)
    {
        FillStage(stage, (float)maxValue, (float)stageValue);
    }

    void FillStage(int stage, float maxValue, float stageValue)
    {
        if (stage < _StageRect.Count && stage < (int)StageType)
        {
            float stageWidth = Mathf.Floor((stageValue / maxValue) * StageContainer.sizeDelta.x);
            FillStage(_StageRect[stage], stageWidth);
            AdjustStagePos(stage);
        }
    }

    void AdjustStagePos(int stage)
    {
        float pos = 0f;
        for (int index = 0; index < stage && index < _StageRect.Count; ++index)
        {
            pos += _StageRect[index].sizeDelta.x;
        }
        pos += (_StageRect[stage].sizeDelta.x / 2);
        Vector3 oldPos = _StageRect[stage].anchoredPosition;
        _StageRect[stage].anchoredPosition = new Vector3(pos, oldPos.y, oldPos.z);
    }

    void FillStage(RectTransform stage, float stageWidth)
    {
        stage.sizeDelta = new Vector2(stageWidth, stage.sizeDelta.y);
    }

    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_MultipleStageSlider dataComponent = gameObject.GetComponent<GUI_MultipleStageSlider>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_MultipleStageSlider,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            StageType = dataComponent.StageType;
            StageContainer = dataComponent.StageContainer;
            StageImage = dataComponent.StageImage;
        }
    }
}
