using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI tapToPlay;
    public AudioSource audioSource;
    public GameObject quitPanel;
    public GameObject helpPanel;

    public AudioClip coin;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.volume = PlayerPrefs.GetFloat("volume", 1f);
        tapToPlay.transform.DOScale(1.25f, 1.0f).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitPanel.SetActive(!quitPanel.activeInHierarchy);
        }
    }

    public void OnPlayClicked()
    {
        audioSource.PlayOneShot(coin);
        SceneManager.LoadScene(1);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickResume()
    {
        quitPanel.SetActive(false);
    }

    public void OnHelpButtonClick()
    {
        helpPanel.SetActive(true);
    }

    public void OnCloseHelp()
    {
        helpPanel.SetActive(false);
    }
}
