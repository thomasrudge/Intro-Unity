using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UIManager : MonoBehaviour
{

    public GameObject endGamePanel;
    public TMP_Text txtTitulo;
    public TMP_Text txtTempoFinal;
    public TMP_Text txtJoias;
    public TMP_Text txtVidas;
    public GameObject btnReiniciar;

    public AudioSource musica;
    public AudioClip somPreso;

    void Update()
    {
        txtJoias.text = "Joias: " + GameController.coletados + "/3";
        txtVidas.text = "Vidas: " + GameController.vidas;
    }

    void FixedUpdate()
    {

        if (GameController.gameOver && !endGamePanel.activeSelf){
            endGamePanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(btnReiniciar);

            musica.Stop();

            if (GameController.venceu){
                txtTitulo.text = "Você escapou!";
            } else {
                txtTitulo.text = "Você foi preso!";
                musica.PlayOneShot(somPreso);
            }

            float tempoJogado = 60f - GameController.tempo;
            txtTempoFinal.text = "Tempo: " + tempoJogado.ToString("F1") + "s";
        }
        
    }
}