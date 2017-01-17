using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Toggle))]
public abstract class GUI_ToggleItem_DL : GUI_LogicObject
{
    public GameObject ExtendCheckMark;
    public int ToggleIndex;
    bool _IsOn = false;
    bool _AsButton = false;
    Toggle _toggle;
    public Toggle Target
    {
        get
        {
            if (null == _toggle)
            {
                _toggle = GetComponent<Toggle>();
            }
            return _toggle;
        }
    }

    public bool IsSelect
    {
        get
        {
            return Target.isOn;
        }
    }

    void OnEnable()
    {
        Target.onValueChanged.AddListener(OnValueChange);
    }

    void OnDisable()
    {
        Target.onValueChanged.RemoveListener(OnValueChange);
    }

    void OnValueChange(bool isOn)
    {
        if (_AsButton)
        {
            OnButtonEvent(isOn);
        }
        else
        {
            OnToggleEvent(isOn);
        }
    }

    void OnButtonEvent(bool isOn)
    {
        if (isOn)
        {
            OnSelected();
        }
        else
        {
            OnDeSelected();
        }
    }

    void OnToggleEvent(bool isOn)
    {
        if (isOn != _IsOn)
        {
            if (!_IsOn)
            {
                if (ExtendCheckMark != null)
                {
                    ExtendCheckMark.SetActive(true);
                }
                OnSelected();
            }
            else
            {
                OnDeSelected();
                if (ExtendCheckMark != null)
                {
                    ExtendCheckMark.SetActive(false);
                }
            }
            _IsOn = isOn;
        }
        else
        {
            Target.isOn = _IsOn;
        }
    }

    protected abstract void OnSelected();
    protected abstract void OnDeSelected();

    public void AsButton()
    {
        _AsButton = true;
    }

    public void AsToggle()
    {
        _AsButton = false;
    }

    public void Select()
    {
        Target.isOn = true;
    }

    public void DeSelect()
    {
        Target.isOn = false;
    }

    public void RegistToGroup(ToggleGroup group)
    {
        Target.group = group;
    }

    public void DisableToggle()
    {
        Target.interactable = false;
    }

    public void EnableToggle()
    {
        Target.interactable = true;
    }

    void Awake()
    {
        CopyDataFromDataScript();
        OnAwake();
    }

    protected virtual void OnAwake()
    {

    }

    protected virtual void CopyDataFromDataScript()
    {
        GUI_ToggleItem dataComponent = gameObject.GetComponent<GUI_ToggleItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_ToggleItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            ExtendCheckMark = dataComponent.ExtendCheckMark;
            ToggleIndex = dataComponent.ToggleIndex;
        }
    }
}
