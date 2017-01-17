using UnityEngine;


public class ActorWeapon : SKILL.CompBase
{
    public SkillDC dc;
    public GameObject Weapon;
    public Collider collider;

    Transform _root;
    public override void Init(Actor a)
    {
        base.Init(a);
        ActorPreDefine pre = Owner.GetComponent<ActorPreDefine>();
        if (pre.AtkTransform != null)
        {
            _root = pre.AtkTransform;
            collider = _root.GetComponent<Collider>();
        }
    }

    public void SetWeapon(Transform weapon, Vector3 localPosition, Vector3 localRotation)
    {
        if(weapon != null)
        {
            Discharge();
            weapon.parent = _root;
            weapon.localPosition = localPosition;
            weapon.localRotation = Quaternion.Euler(localRotation);
            Weapon = weapon.gameObject;
            collider = weapon.GetComponent<Collider>();
        }
    }

    public void Active(int skillId, SKILL.DCMeta meta)
    {
        SkillDC dc = PrepareDC();
        if(dc != null)
        {
            dc.SkillId = skillId;
            dc.MetaEx = meta;
            dc.enabled = true;
            collider.enabled = true;
        }
    }

    public void Deactive()
    {
        dc.enabled = false;
        collider.enabled = false;
    }

    SkillDC PrepareDC()
    {
        if (dc == null)
        {
            if(collider != null)
            {
                GameObject weaponObj = collider.gameObject;
                weaponObj.layer = LayerMask.NameToLayer(Owner.SelfCamp == SKILL.Camp.Enemy ? "EnemyBullet" : "ComradeBullet"); // TODO:优化，待layer稳定后，直接用int
                collider.enabled = false;
                dc = weaponObj.AddComponent<SkillDC>();
                dc.Init(Owner);
                dc.enabled = false;
                return dc;
            }
        }
        else
        {
            dc.Countdown();
            return dc;
        }
        return null;
    }

    void Discharge()
    {
        if(Weapon != null)
        {
            GameObject.Destroy(Weapon);
        }
    }
}
