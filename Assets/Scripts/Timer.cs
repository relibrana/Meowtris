using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float TimeLeft;
    public bool TimerOn = false;

    public TextMeshProUGUI  TimerTxt;
    public Canvas timer;

    public Canvas globalCanvas;
     void OnEnable()
    {
        // Inicia el temporizador cuando el script se habilita (Canvas activado)
        TimerOn = true;
    }

    void Update()
    {
        if(TimerOn)
        {
            if(TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                updateTimer(TimeLeft);
            }
            else
            {
                Debug.Log("Time is UP!");
                globalCanvas.gameObject.SetActive(false);
                TimeLeft = 0;
                TimerOn = false;
            }
        }
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        int seconds = Mathf.FloorToInt(currentTime);

    TimerTxt.text = string.Format("{0:D1}", seconds);
    }
}
