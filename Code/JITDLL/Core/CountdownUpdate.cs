using UnityEngine;
using System.Collections;
using DataCenter;

public class CountdownUpdate : MonoBehaviour {

    public static CountdownUpdate _instance = null;

    public static CountdownUpdate Instance
    {
        get
        {
            return _instance;
        }
    }

    uint _addCount;
    uint _diffCount;

	// Use this for initialization
	void Start () {
	
	}

    public static void CreateInstance()
    {
        GameObject go = new GameObject("CountdownUpdate");
        //go.hideFlags = HideFlags.HideAndDontSave;

        GameObject.DontDestroyOnLoad(go);

        _instance = go.AddComponent<CountdownUpdate>();

        _instance.Initialize();
    }

    void Initialize()
    {
        PlayerDataCenter.AdjustStaminaRecoverInterval();
    }

    void UpdateValue(ref uint curentValue, uint maxValue, ref uint upTime, uint intervalTime)
    {
        if (curentValue >= maxValue)
        {
            upTime = PlayerDataCenter.ServerTime;
        }

        if (upTime <= PlayerDataCenter.ServerTime - intervalTime)
        {
            _addCount = (PlayerDataCenter.ServerTime - upTime) / intervalTime;
            _diffCount = maxValue - curentValue;
            if (_addCount > _diffCount)
                _addCount = _diffCount;

            curentValue += _addCount;
            upTime += _addCount * intervalTime;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        // 体力 
        UpdateValue(ref PlayerDataCenter.Stamina, PlayerDataCenter.MaxStamina, ref PlayerDataCenter.StaminaUpTime, PlayerDataCenter.StaminaRecoverInterval);
	}
}
