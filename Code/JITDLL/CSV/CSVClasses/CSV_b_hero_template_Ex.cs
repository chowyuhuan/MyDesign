using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_hero_template : CSVBase
{
    public int PassiveSkillID;

    public List<int> SkillIDs = new List<int>();

    public List<int> NormalSkillIDs = new List<int>();

    public override void OnReadRow(CSVDataFile csvFile)
    {
        PassiveSkillID = csvFile.GetInt("SkillPassive");
        
        for (int i = 0; i < 3; ++i)
        {
            SkillIDs.Add(csvFile.GetInt("SkillID" + (i + 1)));
        }

        for (int i = 0; i < NormalSkillCount; ++i)
        {
            NormalSkillIDs.Add(csvFile.GetInt("NormalSkillID" + (i + 1)));
        }
    }
}
