using System.Collections.Generic;

namespace BUFF
{
    /// <summary>
    /// Trigger管理
    /// </summary>
    public class TriggerManager
    {
        private List<Trigger> triggerList;

        public TriggerManager()
        {
            triggerList = new List<Trigger>();
        }

        public List<Trigger> GetAllTrigger()
        {
            return triggerList;
        }
       
        public void AddTrigger(Trigger trigger)
        {
            triggerList.Add(trigger);
        }

        public Trigger FindTrigger(string tag)
        {
            foreach (Trigger trigger in triggerList)
            {
                if (trigger.Tag == tag)
                {
                    return trigger;
                }
            }

            return null;
        }
        
        public void Update()
        {
            UpdateTrigger();
        }

        /// <summary>
        /// 检测所有Trigger
        /// </summary>
        private void UpdateTrigger()
        {
            for (int i = triggerList.Count - 1; i >= 0; i--)
            {
                Trigger trigger = triggerList[i];

                trigger.Update();

                if (trigger.Finish())
                {
                    triggerList.RemoveAt(i);
                }
            }
        }
    }
}
