using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    public GameObject EndGameCanvas;
    public GameManager manager;
    public Text timeLeft;
    public StopBtn stopBtn;
    private bool hasEnd=false;

    public float time = 120;
    public void TimeLeft()
    {
        if (stopBtn.StopWindow.activeSelf == false)
            time -= Time.deltaTime;

        if (time <= 0)
        {
            if(hasEnd)
                return;
            // EndGameCanvas.SetActive(true);
            time = 0;
            Pause();
            if (manager.isMult)
            {
                // Debug.Log("MulitGame End");
                manager.GameEnd();
            }
            else
            {
                EndGameCanvas.SetActive(true);
            }
            hasEnd=true;
        }
        else
        {
            // time = 60;
            // Time.timeScale = 1;
            EndGameCanvas.SetActive(false);
        }
    }

    public void Pause()
    {
        stopBtn.Generater.SetActive(false);
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        TimeLeft();
        timeLeft.text = time.ToString("0.00");
    }
}
