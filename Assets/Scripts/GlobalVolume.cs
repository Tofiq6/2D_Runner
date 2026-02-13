using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVolumeInitializer : MonoBehaviour
{
    private const string VolumeKey = "MasterVolume";

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        AudioListener.volume = savedVolume;
    }
}

