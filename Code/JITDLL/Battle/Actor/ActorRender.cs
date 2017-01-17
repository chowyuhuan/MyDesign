using UnityEngine;
using System.Collections;
using SKILL;

public class ActorRender : CompBase
{
    public enum Property
    {
        MainTex,
        MainColor,
        OutlineColor,
        OutlineWeight,

        Max
    }
    public Material[] Materials;
    public int[] Propertis;

    HighLightControl _highLightControl = null;
    OutLineControl _outlineControl = null;

    public override void Init(Actor a)
    {
        base.Init(a);

        Renderer[] renders = a.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renders.Length; ++i)
        {
            if (Materials == null)
            {
                Materials = new Material[renders[i].materials.Length];
                renders[i].materials.CopyTo(Materials, 0);
            }
            else
            {
                Material[] mats = new Material[Materials.Length + renders[i].materials.Length];
                Materials.CopyTo(mats, 0);
                renders[i].materials.CopyTo(mats, Materials.Length);
                Materials = mats;
            }
        }

        Propertis = new int[(int)Property.Max];
        Propertis[(int)Property.MainTex] = Shader.PropertyToID("_MainTex");
        Propertis[(int)Property.MainColor] = Shader.PropertyToID("_Color");
        Propertis[(int)Property.OutlineColor] = Shader.PropertyToID("_OutlineColor");
        Propertis[(int)Property.OutlineWeight] = Shader.PropertyToID("_Outline");

        _outlineControl = new OutLineControl();
        _outlineControl.Init(a);
        _highLightControl = new HighLightControl();
        _highLightControl.Init(a);
    }

    public void HighLight()
    {
        _highLightControl.Light();
    }

    public void Outline(int index)
    {
        _outlineControl.Light(index);
    }

    public int GetPropertyID(Property p)
    {
        if (p < Property.Max)
        {
            return Propertis[(int)p];
        }
        return -1;
    }

    public void CUpdate()
    {
        _outlineControl.CUpdate();
        _highLightControl.CUpdate();
    }

}
