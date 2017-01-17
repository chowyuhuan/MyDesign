
namespace BUFF
{
    /// <summary>
    /// 消块主题
    /// </summary>
    public class SubjectCube : Subject
    {
        // 消块类型
        public CubeEraseType cubeEraseType;

        public SubjectCube(ITargetWrapper caster, CubeEraseType cubeEraseType) 
        {
            this.type = SubjectType.Cube;
            Init(caster, cubeEraseType);
        }

        public void Init(ITargetWrapper caster, CubeEraseType cubeEraseType)
        {
            this.caster = caster;
            this.cubeEraseType = cubeEraseType;
        }

        public override string Message()
        {
            return base.Message() + " 消除类型:" + cubeEraseType.ToString();
        }
    }
}
