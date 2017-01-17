using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_game_level : CSVBase
{
    public List<int> MonsterWaveIds = new List<int>();

    public override void OnReadRow(CSVDataFile csvFile)
    {
        string[] strArray = MonsterWaveList.Split('|');
        for (int i = 0; i < strArray.Length; ++i)
        {
            int id;
            if (int.TryParse(strArray[i], out id))
            {
                MonsterWaveIds.Add(id);
            }
            else
            {
                Debug.LogError("Some thing wrong in csv game_level.MonsterWaveList LevelId " + LevelId);
            }
        }
    }
}
