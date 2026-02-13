using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    private static MenuMusic instance;

    void Awake()
    {
        // Eðer sahnede zaten bir tane varsa, yenisini yok et
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Sahne deðiþince yok olma
    }
}


