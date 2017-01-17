using System.Collections.Generic;

namespace Assets.Scripts.Observer
{
    public class BulletinBoard : IObserver
    {
        private Dictionary<Observable, Observable> notifiedObservableMap; 

        public BulletinBoard()
        {
            notifiedObservableMap = new Dictionary<Observable, Observable>();
        }

        public void Update(Observable observable)
        {
            notifiedObservableMap.Add(observable, observable);
        }

        public Dictionary<Observable, Observable> GetNotifiedObservableMap()
        {
            return notifiedObservableMap;
        }

        public void ClearNotifiedObservableList()
        {
            notifiedObservableMap.Clear();
        }
    }
}
