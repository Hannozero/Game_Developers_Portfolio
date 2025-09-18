using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    public GameObject clearUI;
    public int score = 0;
    public int[] monsterCount;

    public GameObject[] scoreStamp;
    public TMP_Text[] countText;

    public int[] scoreCutLine;

    void Awake()
    {
        for (int i = 0; i < monsterCount.Length; i++) {
            countText[i].text = monsterCount[i].ToString();
        }

        if (score > scoreCutLine[0]) {
            scoreStamp[0].SetActive(true);
        }
        else if(score > scoreCutLine[1]) {
            scoreStamp[1].SetActive(true);
            scoreStamp[0].SetActive(false);
        }
        else if (score > scoreCutLine[2])
        {
            scoreStamp[2].SetActive(true);
            scoreStamp[1].SetActive(false);
        }
        else if (score > scoreCutLine[3])
        {
            scoreStamp[3].SetActive(true);
            scoreStamp[2].SetActive(false);
        }
        else if (score > scoreCutLine[4])
        {
            scoreStamp[4].SetActive(true);
            scoreStamp[3].SetActive(false);
        }

    }

    void Update()
    {
        for (int i = 0; i < monsterCount.Length; i++)
        {
            countText[i].text = monsterCount[i].ToString();
        }

        if (score > scoreCutLine[0])
        {
            scoreStamp[0].SetActive(true);
        }
        else if (score > scoreCutLine[1])
        {
            scoreStamp[1].SetActive(true);
            scoreStamp[0].SetActive(false);
        }
        else if (score > scoreCutLine[2])
        {
            scoreStamp[2].SetActive(true);
            scoreStamp[1].SetActive(false);
        }
        else if (score > scoreCutLine[3])
        {
            scoreStamp[3].SetActive(true);
            scoreStamp[2].SetActive(false);
        }
        else if (score > scoreCutLine[4])
        {
            scoreStamp[4].SetActive(true);
            scoreStamp[3].SetActive(false);
        }
    }
}
