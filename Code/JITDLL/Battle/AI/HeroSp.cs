using UnityEngine;
using System.Collections;

public class HeroSp : ActorSp
{
    int _spPoint = 0;

    bool _valid = false;

    int _threshold = 100;

    public override void Init(Actor a)
    {
        base.Init(a);

        _spPoint = 0;
        _valid = Owner.specialSkillId > 0;
        Owner.SkillController.Caster.OnStartCast += OnStartCaskSkill;
    }

    public override void IncreaseSp(int amount)
    {
        if (!_valid) return;

        _spPoint += amount;

        while(_spPoint >= _threshold)
        {
            _spPoint -= 100;

            SkillGenerator.Instance.AppendSkill(Owner, 3);
        }

        RaiseOnSpProgressChange();
    }

    public override void ReduceSp(int amount)
    {
        if (!_valid) return;

        if (_spPoint > amount)
        {
            _spPoint -= amount;
        }
        else
        {
            _spPoint = 0;
        }

        RaiseOnSpProgressChange();
    }

    void RaiseOnSpProgressChange()
    {
        this.Value = (float)_spPoint / _threshold;

        if (OnSpProgressChange != null)
        {
            OnSpProgressChange(this.Value);
        }
    }

    void OnDisable()
    {
        Owner.SkillController.Caster.OnStartCast -= OnStartCaskSkill;
    }

    void OnStartCaskSkill(SKILL.Skill skill, int block)
    {
        if (block > 0 && block < 4)
        {
            IncreaseSp(20 * block);
        }
    }
}
