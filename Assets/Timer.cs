using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text txtTempo;

    void Update()
    {
        if (GameController.gameOver) return;

        GameController.tempo -= Time.deltaTime;

        if (GameController.tempo <= 0f)
        {
            GameController.tempo = 0f;
            GameController.perdeu = true;
        }

        txtTempo.text = Mathf.CeilToInt(GameController.tempo).ToString();
    }
}