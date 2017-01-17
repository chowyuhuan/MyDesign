using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_skill_template : CSVBase
{
    public class SkillGroupInfo
    {
        public int GroupId;
        public int MaxLevel;
        public int Level1SkillId;
        public int School;
    }

    public static CSV_b_skill_template FindData(int groupId, int level)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        return csv_data.Find(x => (x.GroupId == groupId && x.Level == level));
    }

    public static List<SkillGroupInfo> GetAllSkillGroupInfo()
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        List<SkillGroupInfo> simpleInfoList = new List<SkillGroupInfo>();

        SkillGroupInfo simpleInfo = null;
        for (int i = 0; i < csv_data.Count; ++i)
        {
            if (simpleInfo == null || simpleInfo.GroupId != csv_data[i].GroupId )
            {
                simpleInfo = new SkillGroupInfo();
                simpleInfo.GroupId = csv_data[i].GroupId;
                simpleInfo.School = csv_data[i].School;

                simpleInfoList.Add(simpleInfo);
            }

            simpleInfo.MaxLevel = csv_data[i].Level;
            if (csv_data[i].Level == 1) simpleInfo.Level1SkillId = csv_data[i].Id;
        }

        return simpleInfoList;
    }
}
