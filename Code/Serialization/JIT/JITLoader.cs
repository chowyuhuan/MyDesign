using UnityEngine;
using System.Collections;
using System.Reflection;

public class JITLoader : MonoBehaviour
{
	// Use this for initialization
	void Awake () 
    {
        StartCoroutine(LoadAndRunDll());
	}

    /// <summary>
    /// 注意：这是协程，即其执行结束的时间不定，所以，如果不限制其他GameObject的执行时机，就可能出现提前于协程结束执行的情况
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadAndRunDll()
    {
#if JIT
#if UNITY_STANDALONE_WIN || UNITY_ANDROID
#if UNITY_EDITOR
        string path = System.IO.Path.Combine("file://" + Application.streamingAssetsPath, "jit/JITDLL.dll");
#else
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "jit/JITDLL.dll");
#endif
        WWW www = new WWW(path);
        yield return www;
        if(!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(LogTag.JIT + "加载DLL失败：" + www.error);
            yield break;
        }
        Assembly assembly = Assembly.Load(www.bytes);
        if(assembly == null)
        {
            Debug.LogError(LogTag.JIT + "加载DLL失败：Assembly创建失败");
            yield break;
        }
        ScriptAssembly.LoadAllLogicTypes(assembly);
#endif
#endif
        StartGame();
        yield break;
    }

    /// <summary>
    /// 确保所有的游戏内容从这里开始，不要有任何代码提前于这个接口执行
    /// </summary>
    void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");        
    }
}