using UnityEngine;

public class Patrulha : MonoBehaviour
{
    public Vector2 distancia = new Vector2(4f, 0f);
    public float speed = 3f;

    private Rigidbody2D rb;
    private Vector2 pontoA;
    private Vector2 pontoB;
    private Vector2 alvo;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pontoA = rb.position;
        pontoB = pontoA + distancia;
        alvo = pontoB;
    }

    void FixedUpdate()
    {
        if (GameController.gameOver) return;

        Vector2 novaPos = Vector2.MoveTowards(rb.position, alvo, speed * Time.fixedDeltaTime);
        rb.MovePosition(novaPos);

        if (Vector2.Distance(novaPos, alvo) < 0.05f){
            alvo = (alvo == pontoA) ? pontoB : pontoA;
        }
    }
}