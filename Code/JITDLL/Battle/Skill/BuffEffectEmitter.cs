using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ACTOR;
using SKILL;
using BUFF;

public class BuffEffectEmitter : MonoCompBase
{
    private Dictionary<string, GameObject> effects = new Dictionary<string,GameObject>();

    public override void Init(Actor a)
    {
        base.Init(a);
    }

    public void Update()
    {
        UpdateEffect();
    }

    private void UpdateEffect()
    {
        foreach (GameObject effect in effects.Values)
        {
            effect.transform.position = Owner.transform.position;
        }
    }

    public void SpwanEffect(BUFF.Buff buff)
    {
        GameObject effect = EntityPool.Spwan(AssetManage.AM_PathHelper.GetActorEffectFullPathByName(buff.Effect), Owner.transform.position) as GameObject;
        if (effect != null)
        {
            effects.Add(buff.Id, effect);
        }
    }

    public void DestroyEffect(BUFF.Buff buff)
    {
        GameObject effect;
        if (effects.TryGetValue(buff.Id, out effect))
        {
            effects.Remove(buff.Id);
            EntityPool.Destroy(effect);
        }
    }
}
