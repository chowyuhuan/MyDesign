using UnityEngine;
using System;
using System.Collections;

public class SpChange
{
    public Action<float> OnSpProgressChange;
    public Action<int> OnSpSkillFilled;

    protected bool CanChange = true;

    protected void RaiseSpProgressChange(float value)
    {
        if (OnSpProgressChange != null)
        {
            OnSpProgressChange(value);
        }
    }

    protected void RaiseSpSkillFilled(int skilId)
    {
        if (OnSpSkillFilled != null)
        {
            OnSpSkillFilled(skilId);
        }
    }
}
