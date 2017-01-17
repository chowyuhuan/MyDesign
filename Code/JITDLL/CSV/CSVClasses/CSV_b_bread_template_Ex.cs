using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_bread_template : CSVBase
{
    public List<int> TrainCoins = new List<int>();

    public override void OnReadRow(CSVDataFile csvFile)
    {
        for (int i = 2; i <= 6; ++i)
        {
            TrainCoins.Add(csvFile.GetInt("TrainCoin" + i + "Star"));
        }
    }

    public int GetTrainCoin(int star)
    {
        if (star >= 2 && star <= 6)
            return TrainCoins[star - 2];

        return 0;
    }
}
