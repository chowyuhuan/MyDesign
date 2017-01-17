using UnityEngine;
using System.Collections;

public class ShakeScreen : MonoBehaviour
{
    [System.Serializable]
    public struct OneShake
    {
        public float Degrees;
        public float Range;
        public int Count;
        public float Time;
    }

    public OneShake[] ShakeParams = null;

    public void Shake(int index)
    {
        if (ShakeParams != null && index > -1 && index < ShakeParams.Length)
        {
            OneShake temp = ShakeParams[index];
            CameraControl.Instance.Shake(temp.Degrees, temp.Range, temp.Count, temp.Time);
        }
    }
}
