using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExpeditionChest
{
    public int ConditionCount;
}

public class ExpeditionAwardCondition
{
    public int ConditionType;
    public int ConditionValue;

    public static int Compare(ExpeditionAwardCondition e1, ExpeditionAwardCondition e2)
    {
        return (e1.ConditionType * 10000 - e1.ConditionValue) - (e2.ConditionType * 10000 - e2.ConditionValue);
    }
}

public partial class CSV_b_expedition_random_award : CSVBase
{
    public List<ExpeditionChest> ExtraChests = new List<ExpeditionChest>();
    public List<ExpeditionAwardCondition> ExtraConditions = new List<ExpeditionAwardCondition>();

    public override void OnReadRow(CSVDataFile csvFile)
    {

        for (int i = 0; i < 3; ++i)
        {
            int awardGroupId = csvFile.GetInt("RandomAwardGroup" + (i + 1).ToString());
            if (awardGroupId > 0)
            {
                ExpeditionChest expeditionChest = new ExpeditionChest();

                expeditionChest.ConditionCount = csvFile.GetInt("ConditionCount" + (i + 1).ToString());

                ExtraChests.Add(expeditionChest);
            }
        }

        for (int i = 0; i < 10; ++i)
        {
            ExpeditionAwardCondition awardCondition = new ExpeditionAwardCondition();
            awardCondition.ConditionType = csvFile.GetInt("ConditionType" + (i + 1).ToString());
            awardCondition.ConditionValue = csvFile.GetInt("ConditionValue" + (i + 1).ToString());

            ExtraConditions.Add(awardCondition);
        }
    }

}
