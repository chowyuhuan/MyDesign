using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_hero_group_template : CSVBase
{
    public static int GetCurLevelExp(int totalExp, uint level)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        if (level > 1)
        {
            CSV_b_hero_group_template curGroupTemplate = FindData((int)level);
            if (totalExp >= curGroupTemplate.Exp)
            {
                return totalExp - curGroupTemplate.Exp;
            }
            CSV_b_hero_group_template preGroupTemplate = FindData((int)level - 1);
            return totalExp - preGroupTemplate.Exp;
        }
        else
        {
            CSV_b_hero_group_template groupTemplate = FindData((int)level);
            return totalExp - groupTemplate.Exp;
        }
    }

    public static int GetLevelGrowUpExp(uint level)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        if(level > 1)
        {
            CSV_b_hero_group_template curGroupTemplate = FindData((int)level);
            CSV_b_hero_group_template preGroupTemplate = FindData((int)level - 1);
            return curGroupTemplate.Exp - preGroupTemplate.Exp;
        }
        else
        {
            CSV_b_hero_group_template groupTemplate = FindData((int)level);
            return groupTemplate.Exp;
        }
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
