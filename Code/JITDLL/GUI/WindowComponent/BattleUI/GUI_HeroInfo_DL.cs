using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_HeroInfo_DL : MonoBehaviour
{
    public Text _LevelInfoText;
    public Slider _HpSlider;
    public Slider _SpSlider;
    public Image _GrayMask;
    public Image _CaptainIcon;
    public Image _HeadIcon;
    public Text _StarNum;
    public Text _CurHpText;
    public Text _CurSpText;

    GameObject DCInfoObject;
    public GUI_HeroDCInfo_DL _DCInfo;

    protected int _HeroId;
    protected bool _Captain;
    protected int _Level;
    protected int _CurHp;

    protected int _MaxHp;
    Actor _TargetHero;
    public DataCenter.Hero DisplayHero;

    public void Init(Actor hero, int heroId, int heroLevel, bool captain, int hp)
    {
        _TargetHero = hero;
        _HeroId = heroId;
        _Level = heroLevel;
        _Captain = captain;
        _CurHp = hp;
        _MaxHp = hp;

#if UNITY_EDITOR
        Debug.Assert(null != _LevelInfoText);
#endif
        if(null == _DCInfo)
        {
            _DCInfo = DCInfoObject.GetComponent<GUI_HeroDCInfo_DL>();
        }
        DisplayHero = DataCenter.PlayerDataCenter.GetHero(hero.ServerId);
        CSV_b_hero_template ht = CSV_b_hero_template.FindData((int)DisplayHero.CsvId);
        GUI_Atlas uiatlas = AssetManage.AM_Manager.LoadAssetSync<GUI_Atlas>("GUI/UIAtlas/" + ht.HeadIconAtlas, true, AssetManage.E_AssetType.GUIAtlas);
        _HeadIcon.sprite = uiatlas.GetSprite(ht.HeadIcon);
        _StarNum.text = ht.Star.ToString();
        _LevelInfoText.text = "LV" + heroLevel;//Todo：看策划需求，将来可能用美术图片和艺术字
        SetCaptainTag(captain);
        _CurHpText.text = _CurHp.ToString();
        _DCInfo.Init(ht.Name);

        OnHpChange(0);
        OnSpChange(0);

        RegistEvent();
    }

    public void SetCaptainTag(bool captain)
    {
        if (_CaptainIcon != null)
        {
            _CaptainIcon.gameObject.SetActive(captain);
        }
    }

    public void OnHpChange(int changeValue)
    {
        _CurHp += changeValue;
        if (_CurHp < 0)
        {
            _CurHp = 0;
        }
        _HpSlider.value = (float)_CurHp / _MaxHp;
        _CurHpText.text = _CurHp.ToString();
    }

    public void OnSpChange(float sp)
    {
        _CurSpText.text = Mathf.RoundToInt(sp * 100).ToString();
        _SpSlider.value = sp;
    }

    public void OnHeroDamaged(Actor attack, Actor attacked, int amount)
    {
        if (attack.BattleId == _TargetHero.BattleId)
        {
            _DCInfo.AddDamage(amount);
        }
        else if (attacked.BattleId == _TargetHero.BattleId)
        {
            OnHpChange(-amount);
        }
    }

    public void OnHeroCure(Actor attack, Actor attacked, int amount)
    {
        if (attack.BattleId == _TargetHero.BattleId)
        {
            _DCInfo.AddCure(amount);
        }
        if (attacked.BattleId == _TargetHero.BattleId)
        {
            OnHpChange(amount);
        }
    }

    public void OnHeroDead(Actor hero)
    {
        SetGrayMask(true);
        UnRegistEvent();
    }

    public void OnHeroReborn()
    {
        _CurHp = _MaxHp;
        SetGrayMask(false);
    }

    void SetGrayMask(bool gray)
    {
#if UNITY_EDITOR
        Debug.Assert(null != _GrayMask);
#endif
        _GrayMask.gameObject.SetActive(gray);
    }

    void OnDestroy()
    {
        UnRegistEvent();
    }

    void RegistEvent()
    {
        if (null != _TargetHero)
        {
            _TargetHero.SkillController.Mixer.RegisterEffectNotify(SKILL.MixEffect.Damage, OnHeroDamaged);
            _TargetHero.SkillController.Mixer.RegisterEffectNotify(SKILL.MixEffect.Cure, OnHeroCure);
            _TargetHero.ActorReference.ActorSpEx.OnSpProgressChange += OnSpChange;
            _TargetHero.OnDeath += OnHeroDead;
        }
    }

    void UnRegistEvent()
    {
        if (null != _TargetHero)
        {
            _TargetHero.SkillController.Mixer.RemoveOnEffectNotify(SKILL.MixEffect.Damage, OnHeroDamaged);
            _TargetHero.SkillController.Mixer.RemoveOnEffectNotify(SKILL.MixEffect.Cure, OnHeroCure);
            _TargetHero.ActorReference.ActorSpEx.OnSpProgressChange -= OnSpChange;
            _TargetHero.OnDeath -= OnHeroDead;
        }
    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_HeroInfo dataComponent = gameObject.GetComponent<GUI_HeroInfo>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_HeroInfo,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            _LevelInfoText = dataComponent._LevelInfoText;
            _HpSlider = dataComponent._HpSlider;
            _SpSlider = dataComponent._SpSlider;
            _GrayMask = dataComponent._GrayMask;
            _CaptainIcon = dataComponent._CaptainIcon;
            _HeadIcon = dataComponent._HeadIcon;
            _StarNum = dataComponent._StarNum;
            _CurHpText = dataComponent._CurHpText;
            _CurSpText = dataComponent._CurSpText;
            DCInfoObject = dataComponent.DCInfoObject;
        }
    }
}
