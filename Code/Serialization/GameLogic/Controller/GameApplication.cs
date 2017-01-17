using UnityEngine;
using System.Collections;

public class GameApplication : MonoBehaviour {

    public static GameApplication Instance
    {
        get;
        protected set;
    }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
    }

    // 去掉了GC，如果加GC，请保证对GC了如指掌
    public static void ReleaseMemory()
    {
        Resources.UnloadUnusedAssets();
    }
}
