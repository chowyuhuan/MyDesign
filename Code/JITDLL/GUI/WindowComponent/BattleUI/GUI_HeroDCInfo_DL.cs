using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_HeroDCInfo_DL : MonoBehaviour
{
    public Text _NameText;
    public Text _DamageText;
    public Text _CureText;

    private int _DamageAmount = 0;
    private int _CureAmount = 0;

    public void Init(string name)
    {
#if UNITY_EDITOR
        Debug.Assert(null != _NameText);
#endif
        _NameText.text = name;
        AddDamage(0);
        AddCure(0);
    }

    public void AddDamage(int amount)
    {
#if UNITY_EDITOR
        Debug.Assert(amount >= 0);
        Debug.Assert(null != _DamageText);
#endif
        _DamageAmount += amount;
        DisplayNumber(_DamageText, _DamageAmount);
    }

    public void AddCure(int amount)
    {
#if UNITY_EDITOR
        Debug.Assert(amount >= 0);
        Debug.Assert(_CureText != null);
#endif
        _CureAmount += amount;
        DisplayNumber(_CureText, _CureAmount);
    }

    void DisplayNumber(Text target, int num)
    {
        if (num < 10000)
        {
            target.text = num.ToString();
        }
        else
        {
            target.text = (num / 1000) + "k";
        }
    }
void Awake()
{
CopyDataFromDataScript();
}

protected void CopyDataFromDataScript()
{
GUI_HeroDCInfo dataComponent = gameObject.GetComponent<GUI_HeroDCInfo>();
if (dataComponent == null)
{
UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroDCInfo,GameObject：" + gameObject.name, gameObject);
}
else
{
_NameText = dataComponent._NameText;
_DamageText = dataComponent._DamageText;
_CureText = dataComponent._CureText;
}
}
}
