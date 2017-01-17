using UnityEngine;
using System.Collections;

public class GUI_EffectPlayer_DL : GUI_LogicObject
{
    public ParticleSystem TargetEffect;

    protected override void OnInit()
    {
        if (null == TargetEffect)
        {
            TargetEffect = GetComponent<ParticleSystem>();
            TargetEffect.playOnAwake = false;
        }
    }

    public void Play()
    {
        if (null != TargetEffect)
        {
            TargetEffect.Play();
        }
    }

    public void Stop()
    {
        if (null != TargetEffect)
        {
            TargetEffect.Stop();
        }
    }

    protected override void OnRecycle()
    {
        Stop();
    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_EffectPlayer dataComponent = gameObject.GetComponent<GUI_EffectPlayer>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_EffectPlayer,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            TargetEffect = dataComponent.TargetEffect;
        }
    }
}
