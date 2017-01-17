using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Observer;
using BUFF;

public class BuffBulletinBoard : MonoBehaviour
{
    [System.Serializable]
    public class ActorInfo
    {
        [HideInInspector]
        public string name;

        public List<string> subjects = new List<string>();
    }

    public List<ActorInfo> bulletinBoard;

    void Awake()
    {
        StartCoroutine(ClearBulletinBoard());
#if UNITY_EDITOR
        bulletinBoard = new List<ActorInfo>();
#endif
    }

    /// <summary>
    /// 每帧最后清空公告板
    /// </summary>
    IEnumerator ClearBulletinBoard()
    {
        while (true)
        {
            yield return Yielders.EndOfFrame;

    #if UNITY_EDITOR
            bulletinBoard.Clear();
    #endif
            
            foreach (Observable observable in ActorMonitor.BulletinBoard.GetNotifiedObservableMap().Values)
            {
                ActorMonitor actorMonitor = (ActorMonitor)observable;
    #if UNITY_EDITOR
                ShowOnBulletinBoard(actorMonitor);
    #endif
                actorMonitor.ClearChangedSubjectMap();
            }
            ActorMonitor.BulletinBoard.ClearNotifiedObservableList();
        }
    }

    private void ShowOnBulletinBoard(ActorMonitor actorMonitor)
    {
        ActorInfo actorInfo = new ActorInfo();
        actorInfo.name = actorMonitor.Tag;

        foreach (List<Subject> subjectList in actorMonitor.ChangedSubjectMap.Values)
        {
            foreach (Subject subject in subjectList)
            {
                actorInfo.subjects.Add(subject.Message());
            }
        }

        bulletinBoard.Add(actorInfo);
    }
}
