using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExpeditionAward
{
    public int awardType;
    public int awardValue;
}



public partial class CSV_b_expedition_quest_template : CSVBase
{
    public List<ExpeditionAward> FixedAwards = new List<ExpeditionAward>();

    public override void OnReadRow(CSVDataFile csvFile)
    {
        for (int i = 0; i < 3; ++i)
        {
            ExpeditionAward award = new ExpeditionAward();
            award.awardType = csvFile.GetInt("FixAwardType" + (i + 1).ToString());
            award.awardValue = csvFile.GetInt("FixAwardValue" + (i + 1).ToString());

            FixedAwards.Add(award);
        }
    }
}
