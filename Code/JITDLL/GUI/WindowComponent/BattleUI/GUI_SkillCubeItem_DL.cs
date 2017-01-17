using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_SkillCubeItem_DL : GUI_LogicObject
{
    #region cube
    public int _SkillId;
    public int _HeroBattleId;
    public int _GroupId;
    public bool _SpacialSkill;
    public Image _SkillIcon;
    public Image _SkillBGBox;
    public string _BGBoxAtlasName;
    public List<string> _BGBoxName;
    List<Sprite> _BGBoxSprite = new List<Sprite>();
    public string _SpecialBGBoxAtlasName;
    public List<string> _SpecialBGBoxName;
    List<Sprite> _SpecialBGBoxSprite = new List<Sprite>();
    bool _InitDone = false;
    public Image GreyMask;

    GUI_EffectPlayer_DL _DoubleCubeAlignEffect;
    GUI_EffectPlayer_DL _TribleCubeAlignEffect;

    public int HeroDisplayOrder { get; protected set; }

    public void SetCubeData(int skillId, int heroId, bool specialSkill, int displayIndex, bool alive)
    {
        Init();
        SetSpecialSkill(specialSkill, displayIndex);
        _SkillId = skillId;
        _HeroBattleId = heroId;
        _SpacialSkill = specialSkill;
        HeroDisplayOrder = displayIndex;

        SKILL.Skill data;
        if (SkillDataCenter.Instance.TryToGetSkill(skillId, out data))
        {
            GUI_Atlas ua = AssetManage.AM_Manager.LoadAssetSync<GUI_Atlas>("GUI/UIAtlas/" + data.IconAtlas, true, AssetManage.E_AssetType.GUIAtlas);
            if ((null != ua))
            {
                _SkillIcon.sprite = ua.GetSprite(data.IconSprite);
            }
        }
        HeroAlive(alive);
    }

    public bool PlayingDoubleAlignEffect()
    {
        return null != _DoubleCubeAlignEffect;
    }

    public void PlayDoubleAlignEffect(GUI_EffectPlayer_DL doubleAlignEffect, GUI_Transform uiTransform)
    {
        if (null != _DoubleCubeAlignEffect)
        {
            doubleAlignEffect.Recycle();
            return;
        }
        StopTribleAlignEffect();
        _DoubleCubeAlignEffect = doubleAlignEffect;
        if (null != _DoubleCubeAlignEffect)
        {
            GUI_Tools.CommonTool.AddUIChild(CachedGameObject, doubleAlignEffect.CachedGameObject, uiTransform);
            _DoubleCubeAlignEffect.Play();
        }
    }

    public void StopDoubleAlignEffect()
    {
        if (null != _DoubleCubeAlignEffect)
        {
            _DoubleCubeAlignEffect.Recycle();
            _DoubleCubeAlignEffect = null;
        }
    }

    public bool PlayingTribleAlignEffect()
    {
        return null != _TribleCubeAlignEffect;
    }

    public void PlayTribleAlignEffect(GUI_EffectPlayer_DL tribleAlignEffect, GUI_Transform uiTransorm)
    {
        if (null != _TribleCubeAlignEffect)
        {
            tribleAlignEffect.Recycle();
            return;
        }
        StopDoubleAlignEffect();
        _TribleCubeAlignEffect = tribleAlignEffect;
        if (null != _TribleCubeAlignEffect)
        {
            _TribleCubeAlignEffect.Play();
            GUI_Tools.CommonTool.AddUIChild(CachedGameObject, _TribleCubeAlignEffect.CachedGameObject, uiTransorm);
        }
    }

    public void StopTribleAlignEffect()
    {
        if (null != _TribleCubeAlignEffect)
        {
            _TribleCubeAlignEffect.Recycle();
            _TribleCubeAlignEffect = null;
        }
    }

    public void HideAlignEffect()
    {
        StopDoubleAlignEffect();
        StopTribleAlignEffect();
    }

    public void HeroAlive(bool alive)
    {
        GreyMask.gameObject.SetActive(!alive);
    }

    void SetSpecialSkill(bool specialSkill, int heroDisplayIndex)
    {
        if (!specialSkill)
        {
            _SkillBGBox.sprite = _BGBoxSprite[heroDisplayIndex];
        }
        else
        {
            _SkillBGBox.sprite = _SpecialBGBoxSprite[heroDisplayIndex];
        }
    }

    void Init()
    {
        if (!_InitDone)
        {
            _InitDone = true;
            InitSpriteList(_BGBoxAtlasName, _BGBoxName, _BGBoxSprite);
            InitSpriteList(_SpecialBGBoxAtlasName, _SpecialBGBoxName, _SpecialBGBoxSprite);
        }
    }

    void InitSpriteList(string atlasName, List<string> spriteNameList, List<Sprite> spriteList)
    {
        GUI_Atlas ua = AssetManage.AM_Manager.LoadAssetSync<GUI_Atlas>("GUI/UIAtlas/" + atlasName, true, AssetManage.E_AssetType.GUIAtlas);
#if UNITY_EDITOR
        Debug.Assert(null != ua);
#endif
        for (int index = 0; index < spriteNameList.Count; ++index)
        {
            spriteList.Add(ua.GetSprite(spriteNameList[index]));
        }
    }

    public void OnSkillCubeClick()
    {
        GUI_SkillCubeManager.Instance.RecycleOneSkillCube(this);
    }

    protected override void OnRecycle()
    {
        _SkillId = 0;
        _HeroBattleId = 0;
        _SpacialSkill = false;
        _DoubleCubeAlignEffect = null;
        _TribleCubeAlignEffect = null;
        RecycleTween();
    }
    #endregion

    #region tween position
    Vector3 _From;
    Vector3 _To;
    float _Duration;
    float _MoveTime;
    public bool Tweening = false;
    public int _CurIndex = GUI_SkillCubeManager._SkillCubeCount - 1;

    // Update is called once per frame
    void Update()
    {
        if (Tweening)
        {
            if (CachedTransform.localPosition != _To)
            {
                _MoveTime += GameTimer.deltaTime;
                float percent = Mathf.Clamp01(_MoveTime / _Duration);
                CachedTransform.localPosition = Vector3.Lerp(_From, _To, percent);
            }
            else
            {
                Tweening = false;
                GUI_SkillCubeManager.Instance.RefreshGroupInfo();
            }
        }
    }

    public void Tween(Vector3 tp, float duration, int index)
    {
        _From = CachedTransform.localPosition;
        _To = tp;
        _Duration = duration;
        Tweening = true;
        _MoveTime = 0f;
        _CurIndex = index;
    }

    void RecycleTween()
    {
        _From = Vector3.zero;
        _To = Vector3.zero;
        _Duration = 0f;
        _MoveTime = 0f;
        Tweening = false;
        _CurIndex = GUI_SkillCubeManager._SkillCubeCount - 1;
    }
    #endregion
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_SkillCubeItem dataComponent = gameObject.GetComponent<GUI_SkillCubeItem>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_SkillCubeItem,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            _SkillId = dataComponent._SkillId;
            _HeroBattleId = dataComponent._HeroBattleId;
            _GroupId = dataComponent._GroupId;
            _SpacialSkill = dataComponent._SpacialSkill;
            _SkillIcon = dataComponent._SkillIcon;
            _SkillBGBox = dataComponent._SkillBGBox;
            _BGBoxAtlasName = dataComponent._BGBoxAtlasName;
            _BGBoxName = dataComponent._BGBoxName;
            _SpecialBGBoxAtlasName = dataComponent._SpecialBGBoxAtlasName;
            _SpecialBGBoxName = dataComponent._SpecialBGBoxName;
            GreyMask = dataComponent.GreyMask;
            Tweening = dataComponent.Tweening;
            _CurIndex = dataComponent._CurIndex;

            dataComponent.ItemButton.onClick.AddListener(OnSkillCubeClick);
        }
    }
}
