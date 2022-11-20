using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class EndGame : MonoBehaviour
{
    public GameObject EndGameCanvas;
    public GameObject StartGameCanvas;
    public GameObject MulitEndPanel;
    public Text ResultLabel;
    public Text TitilLabel;
    public Countdown countdown;
    public StopBtn stopBtn;
    public SoundSetting soundSetting;
    public Player player;
    public Score score;
    public Text EndScore;
    public GameManager gameManager;
    public ScoreSlider scoreSlider;
    public PlayerInput inputManager;
    private bool hasResult = false;
    private bool hasEnd = false;

    public void Start()
    {
        EndGameCanvas.SetActive(false);
    }
    public void HomeBtnOnClick()
    {
        stopBtn.Player.SetActive(true);
        EndGameCanvas.SetActive(false);
        StartGameCanvas.SetActive(true);
        EndGameCanvas.SetActive(false);
        player.gameObject.SetActive(false);
        //stopBtn.Player.SetActive(false);
        stopBtn.Generater.SetActive(false);
    }

    public void RestartBtnOnClick()
    {
        EndGameCanvas.SetActive(false);
        StartGameCanvas.SetActive(false);

        stopBtn.Player.SetActive(true);
        stopBtn.Generater.SetActive(true);
        player.gameObject.SetActive(true);

        player.reStart();
        score.setScore(0);
        countdown.time = 120;
        stopBtn.StopWindow.SetActive(false);

        if (soundSetting.bgmtoggle.isOn)
            soundSetting.audioSource.gameObject.SetActive(true);

        scoreSlider.Start();
        //scoreSlider.GoldSlider.value = 0;
        gameManager.RestartGame();
        player.gameObject.SetActive(true);
        inputManager.enabled = true;
    }

    public void Update()
    {
        if (score.score <= 99999999)
            EndScore.text = score.score.ToString();
        else
            EndScore.text = (99999999).ToString();
    }

    public void EndMutilGame()
    {
        countdown.time=0;
        MulitEndPanel.SetActive(true);
        if (hasResult)
            ResultLabel.gameObject.SetActive(true);
        hasEnd = true;
    }

    public void SetResult(string localScore, string remoteScore)
    {
        if (localScore.CompareTo(remoteScore) >= 0)
        {
            ResultLabel.text = "胜利";
        }
        else
        {
            ResultLabel.text = "失败";
        }
        TitilLabel.text = "你的分数： " + localScore + "   对手分数: " + remoteScore;
        if (hasEnd)
            ResultLabel.gameObject.SetActive(true);
        hasResult = true;
    }
}
