using UnityEngine;
using System.Collections;
using SKILL;

public class OutLineControl : CompBase
{
    int PropertyID;
    Color _defaultColor;
    Color[] _color;
    float _totalTime;
    float _startTime;
    bool _light = false;

    public override void Init(Actor a)
    {
        base.Init(a);
        _color = new Color[3];
        _totalTime = 0;
        _startTime = 0;
    }
    public void Init()
    {
        PropertyID = Owner.ActorReference.ActorRenderEx.GetPropertyID(ActorRender.Property.OutlineColor);
        _defaultColor = DefaultConfig.GetColor("OutLineDefaultColor");
        _color[0] = DefaultConfig.GetColor("OutLineColor1");
        _color[1] = DefaultConfig.GetColor("OutLineColor2");
        _color[2] = DefaultConfig.GetColor("OutLineColor3");
        _totalTime = DefaultConfig.GetFloat("OutLineTime");
    }

    public void Light(int index)
    {
        if (index > -1 && index < 4)
        {
            SetColor(_color[index]);
            _startTime = GameTimer.time;
            _light = true;
        }
    }

    public void CUpdate()
    {
        if (!_light)
        {
            return;
        }

        if (GameTimer.time - _startTime >= _totalTime)
        {
            SetColor(_defaultColor);
            _light = false;
        }
    }

    void SetColor(Color color)
    {
        Material[] materials = Owner.ActorReference.ActorRenderEx.Materials;
        for (int i = 0; i < materials.Length; ++i)
        {
            materials[i].SetColor(PropertyID, color);
        }
    }
}
