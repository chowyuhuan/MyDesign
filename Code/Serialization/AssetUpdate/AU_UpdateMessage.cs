using UnityEngine;
using System.Collections;

public class AU_UpdateMessage : MonoBehaviour {
    public static AU_UpdateMessage Instance
    {
        get;
        private set;
    }

    public AssetUpdate.AU_VersionInfo Version
    {
        get;
        protected set;
    }

    public bool InitFromRemote
    {
        get;
        protected set;
    }


	// Use this for initialization
	void Awake () {
        Instance = this;
        DontDestroyOnLoad(gameObject);
	}

    public void SetUpdateMessage(AssetUpdate.AU_VersionInfo version, bool remote)
    {
        Version = version;
        InitFromRemote = remote;
    }
	
    public void DestroyMessage()
    {
        GameObject.Destroy(gameObject);
    }
}
