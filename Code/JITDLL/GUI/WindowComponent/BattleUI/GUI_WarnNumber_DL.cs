using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUI_WarnNumber_DL : GUI_LogicObject
{
    public Rect _DisplayArea;
    public float _UpMove;
    public List<SpriteRenderer> _ImageNumList = new List<SpriteRenderer>();
    public string _NumberAtlasName;
    public string _NumberPrefix;
    GUI_Atlas _NumberAtlas;
    protected bool _InitDone = false;
    protected List<int> _DisplayNumber;
    public GUI_TweenAlpha _AlphaTweener;
    public GUI_TweenScale _ScaleTweener;
    public GUI_TweenPosition _PositionTweener;
    public float HeapUpDistance = 0.02f;
    public float HeapFrontDistance = -0.05f;
    public float BaseDepth = -1f;
    Actor _TargetActor;

    public void WarnNumber(Actor target, int number, SKILL.Camp camp, EWarnNumberType type, int sortOrder)
    {
        _TargetActor = target;
        Init();
        Transform targetTrans = _TargetActor.transform;
        float x = Random.Range(-_DisplayArea.width / 2 + targetTrans.localPosition.x, _DisplayArea.width / 2 + targetTrans.localPosition.x);
        float y = Random.Range(_DisplayArea.yMin + targetTrans.localPosition.y, _DisplayArea.yMax + targetTrans.localPosition.y);
        CachedTransform.SetAsLastSibling();
        CachedTransform.localPosition = new Vector3(x, y + HeapUpDistance * sortOrder, HeapFrontDistance * GetSelfIndexInUsingList() + BaseDepth);
        PlayTweener();
        DisplayNumber(number, camp);
    }

    public void Init()
    {
        if (!_InitDone)
        {
            _InitDone = true;
            _DisplayNumber = new List<int>(_ImageNumList.Count);
            _NumberAtlas = AssetManage.AM_Manager.LoadAssetSync<GUI_Atlas>("GUI/UIAtlas/" + _NumberAtlasName, true, AssetManage.E_AssetType.GUIAtlas);
        }
    }

    void DisplayNumber(int number, SKILL.Camp camp)
    {
        _DisplayNumber.Clear();
        while (number > 0)
        {
            int last = number % 10;
            number = number / 10;
            _DisplayNumber.Add(last);
        }
        int inversIndex = _DisplayNumber.Count - 1;
        for (int index = 0; index < _DisplayNumber.Count; ++index)
        {
            _ImageNumList[index].gameObject.SetActive(true);
            _ImageNumList[index].sprite = _NumberAtlas.GetSprite(_NumberPrefix + _DisplayNumber[inversIndex - index]);
        }
        inversIndex++;
        while (inversIndex < _ImageNumList.Count)
        {
            _ImageNumList[inversIndex++].gameObject.SetActive(false);
        }
    }

    void PlayTweener()
    {
        float duration = Mathf.Max(_AlphaTweener.duration, _ScaleTweener.duration);
        duration = Mathf.Max(duration, _PositionTweener.duration);
        float delay = Mathf.Max(_AlphaTweener.delay, _ScaleTweener.delay);
        delay = Mathf.Max(delay, _PositionTweener.delay);
        Invoke("Recycle", duration + delay);

        _AlphaTweener.ResetToBeginning();
        _AlphaTweener.PlayForward();
        _ScaleTweener.ResetToBeginning();
        _ScaleTweener.PlayForward();
        _PositionTweener.Play(CachedTransform.localPosition, new Vector3(CachedTransform.localPosition.x, CachedTransform.localPosition.y + _UpMove, CachedTransform.localPosition.z));
    }



    protected override void OnRecycle()
    {

        CachedTransform.localPosition = new Vector3(10000f, 10000f, 10000f);
        //CachedTransform.SetAsFirstSibling();
    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_WarnNumber dataComponent = gameObject.GetComponent<GUI_WarnNumber>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_WarnNumber,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            _DisplayArea = dataComponent._DisplayArea;
            _UpMove = dataComponent._UpMove;
            _ImageNumList = dataComponent._ImageNumList;
            _NumberAtlasName = dataComponent._NumberAtlasName;
            _NumberPrefix = dataComponent._NumberPrefix;
            _AlphaTweener = dataComponent._AlphaTweener;
            _ScaleTweener = dataComponent._ScaleTweener;
            _PositionTweener = dataComponent._PositionTweener;
            HeapUpDistance = dataComponent.HeapUpDistance;
            HeapFrontDistance = dataComponent.HeapFrontDistance;
            BaseDepth = dataComponent.BaseDepth;
        }
    }
}
