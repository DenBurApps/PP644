using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    [SerializeField] private RemoutConfigLoader _loader;

    private void OnEnable()
    {
        _loader.ConfigLoadEnded += OnConfigLoadEnded;
    }

    private void OnDisable()
    {
        _loader.ConfigLoadEnded -= OnConfigLoadEnded;
    }

    private void OnConfigLoadEnded(bool showPrivacy)
    {
        if (showPrivacy)
        {
            if (!PlayerPrefs.HasKey("Onboarding"))
            {
                SceneManager.LoadScene("Onboarding");
            }
            else
            {
                SceneManager.LoadScene("MainScreen");
            }
        }
        else
        {
            SceneManager.LoadScene("TestScene");
        }
    }
}