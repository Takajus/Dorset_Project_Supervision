using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonT : MonoBehaviour
{
    public void TryAgain()
    {
        SceneManager.LoadScene(0);
    }
}
