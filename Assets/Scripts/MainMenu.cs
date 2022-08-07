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

    public AudioClip coin;

    // Start is called before the first frame update
    void Start()
    {
        tapToPlay.transform.DOScale(1.25f, 1.0f).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayClicked()
    {
        audioSource.PlayOneShot(coin);
        SceneManager.LoadScene(1);
    }
}
