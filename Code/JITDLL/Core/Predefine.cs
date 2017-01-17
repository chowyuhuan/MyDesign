#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

public class Predefine {
    const string _UseABAndResource = "USE_ABAR";//使用AssetBundle并且执行更新检查
    const string Art = "ART"; // 美术专用
    const string Test = "TEST";//内部测试
    const string JIT = "JIT"; // 代码热更新
    const string Detect = "DETECT"; // 测试监控

    #region RichLog control
    const string LogToUI = "LOG_TO_UI";//输出到UI
    const string NoRichLog = "NO_LOG";//屏蔽一切RichLog接口输出;
    #endregion
}
#endif
