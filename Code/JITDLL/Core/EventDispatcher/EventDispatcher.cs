using System.Collections.Generic;

namespace EVENT
{
    public delegate void EventHandler(params object[] args);

    public class EventDispatcher
    {
        private Dictionary<string, EventHandler> listenerMap;

        public EventDispatcher()
        {
            listenerMap = new Dictionary<string, EventHandler>();
        }

        public void AddListener(string id, EventHandler handler)
        {
            if (listenerMap.ContainsKey(id))
            {
                listenerMap[id] += handler;
            }
            else
            {
                EventHandler newHandler = null;
                newHandler += handler;
                listenerMap.Add(id, newHandler);
            }
        }

        public void RemoveListener(string id, EventHandler handler)
        {
            if (listenerMap.ContainsKey(id))
            {
                listenerMap[id] -= handler;
                if (listenerMap[id] == null)
                {
                    listenerMap.Remove(id);
                }
            }
        }

        public void Dispatch(string id, params object[] args)
        {
            if (listenerMap.ContainsKey(id))
            {
                listenerMap[id].Invoke(args);
            }
        }
    }
}
