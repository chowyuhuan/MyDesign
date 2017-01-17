using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_monster_template : CSVBase
{
    public List<int> skillGroups = new List<int>();
    public List<float> conditions = new List<float>();

    public List<int> NormalSkillIDs = new List<int>();

    public override void OnReadRow(CSVDataFile csvFile)
    {
        for (int i = 0; i < stateCount; ++i)
        {
            skillGroups.Add(csvFile.GetInt("skillGroup" + (i + 1)));
            conditions.Add(csvFile.GetFloat("condition" + (i + 1)));
        }

        for (int i = 0; i < NormalSkillCount; ++i)
        {
            NormalSkillIDs.Add(csvFile.GetInt("NormalSkillID" + (i + 1)));
        }
    }
}
