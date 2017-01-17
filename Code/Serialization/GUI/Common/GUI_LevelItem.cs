using UnityEngine;
// TODO：确保此文件中使用到的类型已经存在
public class GUI_LevelItem : GUI_ToggleItem
{
    public UnityEngine.UI.Text SectionInfo = null;
    public Color SectionSelectedColor = Color.blue;
    public UnityEngine.UI.Text LevelName = null;
    public Color LevelSelectedColor = Color.blue;
    public UnityEngine.UI.Text Difficulty = null;
    public Color DifficultySelectedColor = Color.blue;
    public UnityEngine.UI.Text CostCount = null;
    public Color CostSelectedColor = Color.blue;
    public UnityEngine.UI.Image CostIcon = null;
    public UnityEngine.UI.Image SelectionIcon = null;
    public GameObject TaskTag = null;
    public GameObject TaskFinishTag = null;
    void Awake()
    {
#if JIT && !UNITY_IOS
ScriptAssembly.Assemble(gameObject,"GUI_LevelItem_DL", this); // !!!不要删除，否则丢失逻辑组件
#else
        ScriptAssembly.Assemble<GUI_LevelItem_DL>(gameObject, this);
#endif
    }
}