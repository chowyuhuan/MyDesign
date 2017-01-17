using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerClickHandler
{
    public string Sound = "";
    public float Delay = 0;

    public delegate void OnSoundAreaClick(string soundName, float soundDelay);
    static OnSoundAreaClick SoundAreaEvent;
    public static void RegistSoundHandler(OnSoundAreaClick soundHandler)
    {
        if(null != soundHandler)
        {
            SoundAreaEvent += soundHandler;
        }
    }

    public static void UnRegistSoundHandler(OnSoundAreaClick soundHandler)
    {
        if(null != soundHandler)
        {
            SoundAreaEvent -= soundHandler;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(Sound))
        {
            if(null != SoundAreaEvent)
            {
                SoundAreaEvent(Sound, Delay);
            }
        }
    }
}
