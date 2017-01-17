
namespace BUFF
{
    /// <summary>
    /// Buff叠层信息
    /// </summary>
    public class Overlay
    {
        // 叠层上限
        private int limit;

        // 当前层数
        private int layer;

        public int Layer
        {
            get { return layer; }
        }

        public Overlay(int limit)
        {
            this.limit = limit;
            this.layer = 1;
        }

        public void Merge(Overlay overlay)
        {
            layer += overlay.layer;
            layer = layer < limit ? layer : limit;
        }
    }
}
