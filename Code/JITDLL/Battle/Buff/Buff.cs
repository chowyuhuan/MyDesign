
namespace BUFF
{
    /// <summary>
    /// Buff
    /// </summary>
    public class Buff
    {
        // 类别
        private BuffType type;

        public BuffType Type
        {
            get { return type; }
        }

        // Id
        private string id;

        public string Id
        {
            get { return id; }
        }

        // 特效
        private string effect;

        public string Effect
        {
            get { return effect; }
        }

        // 生命周期
        private LifeCycle lifeCycle;

        public LifeCycle LifeCycle
        {
            get { return lifeCycle; }
        }

        // 叠层
        private Overlay overlay;

        // 状态表
        private State[] stateList;

        // 记录运行时Buff释放者
        private ITargetWrapper caster;

        public ITargetWrapper Caster
        {
            get { return caster; }
        }

        public Buff(ITargetWrapper caster, BuffType type, string id, string effect, LifeCycle lifeCycle, Overlay overlay, State[] stateList)
        {
            this.caster = caster;
            this.type = type;
            this.id = id;
            this.effect = effect;
            this.lifeCycle = lifeCycle;
            this.overlay = overlay;
            this.stateList = stateList;
        }

        public bool Removable()
        {
            return lifeCycle.Removable;
        }

        public bool Finish()
        {
            return lifeCycle.Finish();
        }

        public int Layer()
        {
            return overlay.Layer;
        }

        /// <summary>
        /// 合并buff
        /// </summary>
        /// <param name="buff"></param>
        public void Merge(Buff buff)
        {
            overlay.Merge(buff.overlay);
        }

        /// <summary>
        /// 状态表生效
        /// </summary>
        /// <param name="blackboard"></param>
        public void Enforce(StateBlackboard blackboard)
        {
            foreach (State state in stateList)
            {
                state.StateBlackboard = blackboard;
                state.Enforce(overlay.Layer);
            }
        }

        public string Detail()
        {
            return id + " " + overlay.Layer + " " + lifeCycle.Detail();
        }
    }
}
