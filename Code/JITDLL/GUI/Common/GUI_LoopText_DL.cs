using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class GUI_LoopText_DL : MonoBehaviour {
    float LoopSpeed = 2f;
    Text TargetText = null;
    RectMask2D RectMask = null;
    bool Looping = false;
    float LoopLength = 0f;
    float LoopPos = 0f;
    RectTransform CachedTextTrans;

    public void TryLoopText(string text)
    {
        if(null != TargetText)
        {
            TargetText.text = text;
            PrepareLoop();
        }
    }

    public void StartLoop()
    {
        PrepareLoop();
    }

    public void StopLoop()
    {
        if(Looping)
        {
            Looping = false;
            CachedTextTrans.localPosition = Vector3.zero;
        }
    }

    void PrepareLoop()
    {
        if (null != TargetText)
        {
            CachedTextTrans = TargetText.rectTransform;
            Looping = (TargetText.preferredWidth > RectMask.canvasRect.size.x);
            if (Looping)
            {
                CachedTextTrans.sizeDelta = new Vector2(TargetText.preferredWidth, CachedTextTrans.sizeDelta.y);
            }
            else
            {
                CachedTextTrans.sizeDelta = new Vector2(RectMask.canvasRect.size.x, CachedTextTrans.sizeDelta.y);
            }
            CachedTextTrans.localPosition = new Vector3(0f, CachedTextTrans.localPosition.y, CachedTextTrans.localPosition.z);
            LoopLength = TargetText.preferredWidth + RectMask.canvasRect.size.x / 2;
            LoopPos = RectMask.canvasRect.size.x;
        }
        else
        {
            Looping = false;
        }
    }


    void Update()
    {
        if(Looping)
        {
            CachedTextTrans.Translate(-LoopSpeed, 0f, 0f);
            if (Mathf.Abs(CachedTextTrans.localPosition.x) > LoopLength)
            {
                CachedTextTrans.anchoredPosition = new Vector2(LoopPos, CachedTextTrans.anchoredPosition.y);
            }
        }
    }

    #region jit init
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_LoopText dataComponent = gameObject.GetComponent<GUI_LoopText>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_LoopText,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            LoopSpeed = dataComponent.LoopSpeed;
            TargetText = dataComponent.TargetText;
            RectMask = dataComponent.RectMask;
        }
    }
    #endregion
}
