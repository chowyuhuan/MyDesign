using System.Collections.Generic;

namespace Assets.Scripts.Observer
{
    public abstract class Observable
    {
        private List<IObserver> observerList = new List<IObserver>();

        private bool changed = false;

        public void AddObserver(IObserver observer)
        {
            observerList.Add(observer);
        }

        public void DeleteObserver(IObserver observer)
        {
            observerList.Remove(observer);
        }

        public void NotifyObserver()
        {
            if (changed)
            {
                foreach (IObserver observer in observerList)
                {
                    observer.Update(this);
                }

                changed = false;
            }
        }

        public void SetChanged()
        {
            changed = true;
        }
    }
}
