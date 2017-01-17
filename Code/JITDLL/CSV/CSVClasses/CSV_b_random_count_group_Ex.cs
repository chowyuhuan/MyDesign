using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CSV_b_random_count_group : CSVBase
{
    class ValueRateCollection
    {
        List<int> values = new List<int>();
        List<int> weights = new List<int>();
        int totalWeight = 0;

        public void Init(List<CSV_b_random_count_group> csvLines)
        {
            totalWeight = 0;
            for (int i = 0; i < csvLines.Count; ++i)
            {
                if (csvLines[i].Rate > 0)
                {
                    weights.Add(csvLines[i].Rate);
                    values.Add(csvLines[i].Value);

                    totalWeight += csvLines[i].Rate;
                }
            }
        }

        public int GetRandomCount()
        {
            int randomValue = Random.Range(0, totalWeight);

            for (int i = 0; i < weights.Count; ++i)
            {
                if (randomValue < weights[i])
                {
                    return values[i];
                }

                randomValue -= weights[i];
            }

            return 0;
        }
    }

    static Dictionary<int, ValueRateCollection> valueRateDic = new Dictionary<int, ValueRateCollection>();

    public static int GetRandomCount(int id)
    {
        if (IsInited == false)
        {
            InitCSVTable();
        }

        ValueRateCollection valueRateCollection = null;
        valueRateDic.TryGetValue(id, out valueRateCollection);
        if (valueRateCollection == null)
        {
            valueRateCollection = new ValueRateCollection();
            valueRateCollection.Init(FindAll(id));

            valueRateDic.Add(id, valueRateCollection);
        }

        return valueRateCollection.GetRandomCount();
    }
}
