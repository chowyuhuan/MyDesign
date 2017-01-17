using UnityEngine;
using System.Collections;

/// <summary>
/// 测试脚本，后期删除
/// </summary>
public class AnimationUpdate : MonoBehaviour 
{

    public bool InvokeUpdate = false;
    public float Speed = 1.0f;
    public Animator Controller = null;

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(InvokeUpdate)
        {
            //Controller.SetTimeUpdateMode(UnityEngine.Experimental.Director.DirectorUpdateMode.Manual);
            Controller.Update(Speed);
        }
	}

    void OnGUI()
    {
        if(GUI.Button(new Rect(100,100,100,100),"Frame 30"))
        {
            Application.targetFrameRate = 30;
        }

        if (GUI.Button(new Rect(100, 200, 100, 100), "Frame 60"))
        {
            Application.targetFrameRate = 60;
        }

        if (GUI.Button(new Rect(100, 300, 100, 100), "Scale 1"))
        {
            Time.timeScale = 1;
        }

        if (GUI.Button(new Rect(100, 400, 100, 100), "Scale 0.5"))
        {
            Time.timeScale = 0.5f;
        }

        if (GUI.Button(new Rect(100, 500, 100, 100), "Fixed 0.02"))
        {
            Time.fixedDeltaTime = 0.02f;
        }

        if (GUI.Button(new Rect(100, 600, 100, 100), "Fixed 0.2"))
        {
            Time.fixedDeltaTime = 0.2f;
        }

        if (GUI.Button(new Rect(100, 700, 100, 100), "Frame 1000"))
        {
            Application.targetFrameRate = 1000;
        }
    }
}
