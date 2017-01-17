using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_c_monster_skill_group : CSVBase
{
    public List<int> skillIds = new List<int>();
    public List<float> durations = new List<float>();

    public override void OnReadRow(CSVDataFile csvFile)
    {
        for (int i = 0; i < skillCount; ++i)
        {
            skillIds.Add(csvFile.GetInt("skillId" + (i + 1)));
            durations.Add(csvFile.GetFloat("duration" + (i + 1)));
        }
    }
}
