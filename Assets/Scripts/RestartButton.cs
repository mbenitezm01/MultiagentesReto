using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButton : MonoBehaviour
{
    public void restartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MapaScene");
    }
}
