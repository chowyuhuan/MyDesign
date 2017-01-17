using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_monster_wave : CSVBase
{
    public List<int> monsterIds = new List<int>();
    public List<float> monsterOffsets = new List<float>();

    public override void OnReadRow(CSVDataFile csvFile)
    {
        for (int i = 0; i < monsterCount; ++i)
        {
            monsterIds.Add(csvFile.GetInt("monsterId" + (i + 1)));
        }

        for (int i = 0; i < monsterCount; ++i)
        {
            monsterOffsets.Add(csvFile.GetFloat("monsterOffset" + (i + 1)));
        }
    }
}
