using UnityEngine;
using System.Collections;

public class CheckBulletDestroy : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {
        Invoke("Warning",2);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Warning()
    {
        GUI_MessageManager.Instance.ShowErrorTip("子弹：" + gameObject.name + "没有正常销毁，请检查配置");
    }
}
