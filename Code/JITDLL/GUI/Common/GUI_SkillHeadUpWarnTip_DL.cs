using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_SkillHeadUpWarnTip_DL : MonoBehaviour
{
    public GameObject _WarnTip;
    public RectTransform _WarnTipTrans;
    public GUI_TweenAlpha _AlphaTweener;
    public Text _SkillName;
    public float _HeadUpDistance;
    public Actor TargetActor { get; protected set; }

    public bool Warning { get; protected set; }

    public void ShowHeadUpWarnTip(Actor actor, CSV_c_skill_description skillDes, CSV_c_skill_cast_warn_pattern warnPattern)
    {
        Warning = true;
        TargetActor = actor;
        _WarnTip.SetActive(true);
        _SkillName.text = GUI_Tools.RichTextTool.Color(warnPattern.CastColor, skillDes.Name);
        Vector3 cameraViewPos = Camera.main.WorldToViewportPoint(actor.transform.position);
        _WarnTipTrans.anchoredPosition = new Vector2((cameraViewPos.x - 0.5f) * GUI_Root_DL.Instance.ScreenScaler.referenceResolution.x, (cameraViewPos.y - 0.5f) * GUI_Root_DL.Instance.ScreenScaler.referenceResolution.y + _HeadUpDistance);
        _AlphaTweener.ResetToBeginning();
        _AlphaTweener.PlayForward(OnHeadUpWarnEnd);
    }

    void OnHeadUpWarnEnd()
    {
        Warning = false;
        TargetActor = null;
    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_SkillHeadUpWarnTip dataComponent = gameObject.GetComponent<GUI_SkillHeadUpWarnTip>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_SkillHeadUpWarnTip,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            _WarnTip = dataComponent._WarnTip;
            _WarnTipTrans = dataComponent._WarnTipTrans;
            _AlphaTweener = dataComponent._AlphaTweener;
            _SkillName = dataComponent._SkillName;
            _HeadUpDistance = dataComponent._HeadUpDistance;
        }
    }
}
