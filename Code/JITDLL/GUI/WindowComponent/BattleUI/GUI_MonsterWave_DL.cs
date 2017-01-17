using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_MonsterWave_DL : MonoBehaviour
{
    public Text _PreNumber;
    public Text _PostNumber;
    /*Todo:图片代替文字
    public Image _PreNumberImage;
    public Image _PostNumberImage;
     * */
    public GUI_TweenPosition _TweenIn;
    public GUI_TweenPosition _TweenOut;

    public void Warn(MonsterWaveType waveType, int wave, int total)
    {
        Reset();
        if (waveType == MonsterWaveType.NormalMonster)
        {
            _PreNumber.text = wave.ToString();
            _PostNumber.text = total.ToString();
            _TweenIn.PlayForward();
            _TweenOut.PlayForward();
        }
    }

    void Reset()
    {
        _TweenIn.ResetToBeginning();
        _TweenOut.ResetToBeginning();
    }
    void Awake()
    {
        CopyDataFromDataScript();
    }

    protected void CopyDataFromDataScript()
    {
        GUI_MonsterWave dataComponent = gameObject.GetComponent<GUI_MonsterWave>();
        if (dataComponent == null)
        {
            UnityEngine.Debug.LogError("[热更新]没有找到数据组件：GUI_MonsterWave,GameObject：" + gameObject.name, gameObject);
        }
        else
        {
            _PreNumber = dataComponent._PreNumber;
            _PostNumber = dataComponent._PostNumber;
            _TweenIn = dataComponent._TweenIn;
            _TweenOut = dataComponent._TweenOut;
        }
    }
}
