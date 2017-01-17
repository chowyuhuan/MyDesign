using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_DailyTaskItem : MonoBehaviour {
    public Image TaskIcon;
    public Image TaskTypeBg;
    public Text TaskTypeName;
    public Text TaskName;
    public Text Description;
    public Slider TaskSchedule;
    public Text TaskScheduleText;
    public GUI_ItemSimpleInfo AwardItem;
    public Button TaskJumpButton;
    public GameObject AwardItemRoot;
    public GameObject TaskFinishLock;
    public GameObject AwardGotTag;
    public GameObject GetAwardAlert;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_DailyTaskItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_DailyTaskItem_DL>(gameObject, this);
#endif
    }
}
