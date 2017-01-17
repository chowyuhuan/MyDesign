using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_EnemyInfo_DL : MonoBehaviour
{
    public float _FadeTime = 1f;
    public Text _HpNumber;//todo：换成美术图片字
    public Text _Level;
    public Text _Name;
    public Text _Star;//todo：换成美术图片字
    public Slider _HpSlider;
    public Slider _SpSlider;
    public Image _HeadIcon;
    public GUI_TweenAlpha _AlphaTweener;
    protected GUI_HeadHpAndSp_DL _DisplayEnemy;
    protected bool _FadeIn = false;

    void Start()
    {
        Init();
        FadeOut(0f);
    }

    public void Init()
    {
        _FadeTime = _AlphaTweener.duration;
#if UNITY_EDITOR
        Debug.Assert(null != _Level);
        Debug.Assert(null != _Name);
        Debug.Assert(null != _HpSlider);
        Debug.Assert(null != _SpSlider);
        Debug.Assert(null != _HeadIcon);
#endif
    }

    public void EnemyInfoChange(GUI_HeadHpAndSp_DL enemyInfo)
    {
        if (null != enemyInfo)
        {
            _HpSlider.value = enemyInfo._HpSlider.Value;
            _SpSlider.value = enemyInfo._SpSlider.Value;
            _HpNumber.text = enemyInfo._CurHp.ToString();
        }
    }

    public void Dettach()
    {
        if (_DisplayEnemy != null)
        {
            _DisplayEnemy.AttachDisplay(null);
        }
    }

    public void Attach(GUI_HeadHpAndSp_DL enemyInfo)
    {
        if (null != enemyInfo && !_FadeIn)
        {
            FadeIn(_FadeTime);
        }
        if (_DisplayEnemy != enemyInfo)
        {
            _DisplayEnemy = enemyInfo;
            if (null != _DisplayEnemy)
            {
                _DisplayEnemy.AttachDisplay(this);
                _Level.text = "LV" + enemyInfo._Level;
                _Name.text = enemyInfo._TargetName;
                _Star.gameObject.SetActive(enemyInfo._HasStar);
                _HeadIcon.sprite = enemyInfo._HeadIconSprite;
                if (enemyInfo._HasStar)
                {
                    _Star.text = enemyInfo._Star.ToString();
                }
            }
        }
        if (null == enemyInfo && _FadeIn)
        {
            FadeOut(_FadeTime);
        }
        EnemyInfoChange(_DisplayEnemy);
    }

    void FadeIn(float duration)
    {
        _FadeIn = true;
        _AlphaTweener.PlayReverse();
    }

    void FadeOut(float duration)
    {
        _FadeIn = false;
        _AlphaTweener.PlayForward(duration);
    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_EnemyInfo dataComponent = gameObject.GetComponent<GUI_EnemyInfo>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_EnemyInfo,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            _HpNumber = dataComponent._HpNumber;
            _Level = dataComponent._Level;
            _Name = dataComponent._Name;
            _Star = dataComponent._Star;
            _HpSlider = dataComponent._HpSlider;
            _SpSlider = dataComponent._SpSlider;
            _HeadIcon = dataComponent._HeadIcon;
            _AlphaTweener = dataComponent._AlphaTweener;
        }
    }
}
