using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public AudioSource musicSource;   // Main menu müziði buraya atanacak

    private static bool musicExists = false;
    private static AudioSource savedMusicSource;

    void Awake()
    {
        // Müzik henüz baþlamadýysa baþlat ve koru
        if (!musicExists)
        {
            musicExists = true;
            savedMusicSource = musicSource;
            DontDestroyOnLoad(savedMusicSource.gameObject);
        }
        else
        {
            // Sahne tekrar açýldýysa yeni AudioSource'u yok et
            Destroy(musicSource.gameObject);
        }
    }

    public void StartGame()
    {
        StopMenuMusic();
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenOptions()
    {
        // Options'a geçerken müzik DURMASIN
        SceneManager.LoadScene("Options");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void StopMenuMusic()
    {
        if (savedMusicSource != null)
        {
            savedMusicSource.Stop();
            Destroy(savedMusicSource.gameObject);
            musicExists = false;
        }
    }
}

