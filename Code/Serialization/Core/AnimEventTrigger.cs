using UnityEngine;
using System.Collections;

public class AnimEventTrigger : StateMachineBehaviour
{
    public delegate void OnStateChangedCallBack(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
    public OnStateChangedCallBack EnterTriggerFunc;
    public OnStateChangedCallBack ExitTriggerFunc;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (EnterTriggerFunc != null)
        {
            EnterTriggerFunc(animator, stateInfo, layerIndex);
        }
    }

    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    if (ExitTriggerFunc != null)
    //    {
    //        ExitTriggerFunc(animator, stateInfo, layerIndex);
    //    }
    //}
}
