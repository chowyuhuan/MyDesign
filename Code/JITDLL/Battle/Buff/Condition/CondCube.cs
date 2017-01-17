
namespace BUFF
{
    /// <summary>
    /// 消块条件
    /// </summary>
    public class CondCube : TargetCondition
    {
        // 消块类型
        private CubeEraseType cubeEraseType;

        // 累计消块
        private int count;

        // 计数器
        private int counter;

        public CondCube(CubeEraseType cubeEraseType, int count)
        {
            this.cubeEraseType = cubeEraseType;
            this.count = count;
        }
        
        public override bool Result()
        {
            foreach (Subject subject in GetChangedSubjectList(SubjectType.Cube))
            {
                SubjectCube subjectCube = (SubjectCube)subject;

                switch (cubeEraseType)
                {
                    case CubeEraseType.Single:
                    case CubeEraseType.Double:
                    case CubeEraseType.Triple:
                        return cubeEraseType == subjectCube.cubeEraseType;
                    case CubeEraseType.Any:
                        return CubeEraseType.Single == subjectCube.cubeEraseType ||
                               CubeEraseType.Double == subjectCube.cubeEraseType ||
                               CubeEraseType.Triple == subjectCube.cubeEraseType;
                    case CubeEraseType.Total:
                        if (CubeEraseType.Single == subjectCube.cubeEraseType ||
                            CubeEraseType.Double == subjectCube.cubeEraseType ||
                            CubeEraseType.Triple == subjectCube.cubeEraseType)
                        {
                            counter += (int)subjectCube.cubeEraseType;
                        }
                        break;
                }
            }

            if (cubeEraseType == CubeEraseType.Total &&  counter >= count)
            {
                counter -= count;
                return true;
            }

            return false;
        }

        public override void Reset()
        {
            counter = 0;
        }

        public override object Clone()
        {
            return new CondCube(cubeEraseType, count);
        }

        public override string Message()
        {
            return base.Message() + " Cube type:" + cubeEraseType.ToString() + (cubeEraseType == CubeEraseType.Total ? (" " + counter + "/" + count) : "");
        }
    }
}
