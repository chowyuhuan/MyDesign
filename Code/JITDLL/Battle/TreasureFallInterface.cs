using UnityEngine;
using System;
using System.Collections;

public class TreasureFallInterface
{
    public static Action<TreasureFall.TreasureType, int> OnTreasuseReach;

    public static void RaiseOnTreasuseReach(TreasureFall.TreasureType treasureType, int amount)
    {
        //Debug.Log(string.Format("OnTreasuseReach treasureType: {0} amount: {1}", treasureType, amount));
        if (OnTreasuseReach != null)
        {
            OnTreasuseReach(treasureType, amount);
        }
    }
}
