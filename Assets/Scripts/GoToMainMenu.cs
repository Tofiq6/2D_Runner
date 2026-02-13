using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Oyun durdurulmuþsa geri aç
        SceneManager.LoadScene("MainMenu");
    }
}
