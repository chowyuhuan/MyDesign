using UnityEngine;
using System.Collections;
using SKILL;

public class HighLightControl : CompBase
{
    float _highLitTime = 0.2f;
    Color _highLitColor = Color.white;
    float _startTime = 0;
    bool _light = false;

    public override void Init(Actor a)
    {
        base.Init(a);
        _highLitTime = DefaultConfig.GetFloat("HighLitTime");
        _highLitColor = DefaultConfig.GetColor("HighLitColor");
    }


    public void Light()
    {
        Material[] materials = Owner.ActorReference.ActorRenderEx.Materials;
        for (int i = 0; i < materials.Length; ++i)
        {
            materials[i].color = _highLitColor;
        }
        _startTime = GameTimer.time;
        _light = true;
    }

    public void CUpdate()
    {
        if (!_light)
        {
            return;
        }
        if (GameTimer.time - _startTime >= _highLitTime)
        {
            ClearLight();
            _light = false;
        }
    }

    void ClearLight()
    {
        Material[] materials = Owner.ActorReference.ActorRenderEx.Materials;
        for (int i = 0; i < materials.Length; ++i)
        {
            materials[i].color = Color.clear;
        }
    }
}
