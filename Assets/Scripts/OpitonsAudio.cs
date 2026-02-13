using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsAudio : MonoBehaviour
{
    public Slider volumeSlider;

    private const string VolumeKey = "MasterVolume";

    void Start()
    {
        // Daha önce kayýtlý ses seviyesi varsa onu al, yoksa 1 (tam ses) kullan
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);

        // Slider ve AudioListener'a uygula
        if (volumeSlider != null)
            volumeSlider.value = savedVolume;

        AudioListener.volume = savedVolume;

        // Slider deðiþtiðinde OnVolumeChanged çaðrýlsýn
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    public void OnVolumeChanged(float value)
    {
        // Tüm oyunun sesini deðiþtir
        AudioListener.volume = value;

        // Ayarý kaydet
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }
}
