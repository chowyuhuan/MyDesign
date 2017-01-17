
namespace BUFF
{
    /// <summary>
    /// 接口封装
    /// </summary>
    public interface ITargetWrapper
    {
        string Tag();

        ActorMonitor ActorMonitor();
        
        void AddTrigger(Trigger trigger);

        Trigger FindTrigger(string tag);

        bool HaveBuff(BuffType type, string buffId);

        Buff FindBuff(BuffType type, string buffId);

        void AddBuff(Buff buff);

        void ClearBuff(BuffType type, string buffId);

        void ClearBuff(BuffType type);

        void CastSkill(int skillId, SkillPriority priority);

        void StopSkill();

        int GetEnergy();

        void AddEnergy(int value);

        void GenerateCube();

        float Attribute(int field, bool buff = true);

        void Damage(string skillId, ITargetWrapper caster, object atk);

        void Cure(string skillId, ITargetWrapper caster, object atk);
    }
}
