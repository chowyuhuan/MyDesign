using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_level_template : CSVBase
{
    public static int GetLevelGrowExp(int level)
    {
        int exp = 0;
        if (level > 1)
        {
            int preLevel = level - 1;
            CSV_b_level_template curLevelTemplate = CSV_b_level_template.FindData(level);
            CSV_b_level_template preLevelTemplate = CSV_b_level_template.FindData(preLevel);
            exp = curLevelTemplate.Exp - preLevelTemplate.Exp;
        }
        else
        {
            CSV_b_level_template levelTemplate = CSV_b_level_template.FindData(level);
            exp = levelTemplate.Exp;
        }
        return exp;
    }

    public static int GetCurrentLevelExp(int totalExp, int currentLevel)
    {
        int preExp = 0;

        if (currentLevel > 1)
        {
            CSV_b_level_template curLevelTemplate = CSV_b_level_template.FindData(currentLevel);
            if (totalExp >= curLevelTemplate.Exp)
            {
                return totalExp - curLevelTemplate.Exp;
            }
            int preLevel = currentLevel - 1;
            CSV_b_level_template preLevelTemplate = CSV_b_level_template.FindData(preLevel);
            preExp = preLevelTemplate.Exp;
        }
        else
        {
            CSV_b_level_template levelTemplate = CSV_b_level_template.FindData(currentLevel);
            preExp = levelTemplate.Exp;
        }
        return totalExp - preExp;
    }

    public static int GetLevelTotalExp(uint level)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        if (level > 0 && level <= csv_data.Count)
        {
            return csv_data[(int)level - 1].Exp;
        }

        return csv_data[csv_data.Count - 1].Exp;
    }

    public static int GetLevel(uint totalExp)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        for (int i = csv_data.Count - 1; i >= 0; --i)
        {
            if (totalExp >= csv_data[i].Exp)
            {
                return i + 1;
            }
        }

        return 1;
    }
}
