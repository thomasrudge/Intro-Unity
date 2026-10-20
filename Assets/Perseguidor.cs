using UnityEngine;
using System.Collections.Generic;

public class Perseguidor : MonoBehaviour
{
    public float speed = 2.5f;
    public float tamanhoCelula = 0.5f;
    public Vector2 arenaMin = new Vector2(-7.5f, -3.8f);
    public Vector2 arenaMax = new Vector2(7.5f, 3.8f);
    public float raioAgente = 0.45f;
    public float intervaloRecalculo = 0.25f;

    private Rigidbody2D rb;
    private Transform player;
    private int largura, altura;
    private bool[,] bloqueado;
    private List<Vector2> caminho = new List<Vector2>();
    private float timerRecalculo = 0f;
    private RaycastHit2D[] hits = new RaycastHit2D[8];

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        largura = Mathf.CeilToInt((arenaMax.x - arenaMin.x) / tamanhoCelula);
        altura  = Mathf.CeilToInt((arenaMax.y - arenaMin.y) / tamanhoCelula);
        bloqueado = new bool[largura, altura];

        // mapeia as paredes
        for (int x = 0; x < largura; x++){
            for (int y = 0; y < altura; y++){
                Collider2D[] cols = Physics2D.OverlapCircleAll(Centro(x, y), raioAgente);
                foreach (Collider2D c in cols){
                    if (c.isTrigger) continue;
                    if (c.attachedRigidbody == rb) continue;
                    if (c.CompareTag("Player") || c.CompareTag("Inimigo")) continue;
                    bloqueado[x, y] = true;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (GameController.gameOver || player == null){
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 paraPlayer = (Vector2)player.position - rb.position;

        // visão livre: vai reto
        if (Livre(paraPlayer.normalized, paraPlayer.magnitude)){
            caminho.Clear();
            rb.linearVelocity = paraPlayer.normalized * speed;
            return;
        }

        // recalcula o caminho de tempos em tempos
        timerRecalculo -= Time.fixedDeltaTime;
        if (timerRecalculo <= 0f){
            timerRecalculo = intervaloRecalculo;
            CalcularCaminho();
        }

        if (caminho.Count == 0){
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 alvo = caminho[0];
        if (Vector2.Distance(rb.position, alvo) < 0.15f){
            caminho.RemoveAt(0);
            if (caminho.Count == 0) return;
            alvo = caminho[0];
        }

        rb.linearVelocity = (alvo - rb.position).normalized * speed;
    }

    void CalcularCaminho()
    {
        caminho.Clear();
        Vector2Int inicio = Celula(rb.position);
        Vector2Int fim = Celula(player.position);

        Vector2Int[,] pai = new Vector2Int[largura, altura];
        bool[,] visitado = new bool[largura, altura];
        Queue<Vector2Int> fila = new Queue<Vector2Int>();

        fila.Enqueue(inicio);
        visitado[inicio.x, inicio.y] = true;

        Vector2Int[] dirs = {
            new Vector2Int(1,0), new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(0,-1),
            new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
        };

        bool achou = false;
        while (fila.Count > 0){
            Vector2Int atual = fila.Dequeue();
            if (atual == fim){ achou = true; break; }

            foreach (Vector2Int d in dirs){
                Vector2Int n = atual + d;
                if (n.x < 0 || n.y < 0 || n.x >= largura || n.y >= altura) continue;
                if (visitado[n.x, n.y]) continue;
                if (bloqueado[n.x, n.y] && n != fim) continue;

                // diagonal: não corta quina de parede
                if (d.x != 0 && d.y != 0){
                    if (bloqueado[atual.x + d.x, atual.y] || bloqueado[atual.x, atual.y + d.y]) continue;
                }

                visitado[n.x, n.y] = true;
                pai[n.x, n.y] = atual;
                fila.Enqueue(n);
            }
        }

        if (!achou) return;

        // reconstrói o caminho
        Vector2Int c = fim;
        while (c != inicio){
            caminho.Insert(0, Centro(c.x, c.y));
            c = pai[c.x, c.y];
        }
    }

    bool Livre(Vector2 dir, float dist)
    {
        int n = rb.Cast(dir, hits, dist);
        for (int i = 0; i < n; i++){
            if (hits[i].collider.isTrigger) continue;
            if (hits[i].collider.CompareTag("Player")) continue;
            return false;
        }
        return true;
    }

    Vector2 Centro(int x, int y)
    {
        return arenaMin + new Vector2((x + 0.5f) * tamanhoCelula, (y + 0.5f) * tamanhoCelula);
    }

    Vector2Int Celula(Vector2 p)
    {
        int x = Mathf.Clamp(Mathf.FloorToInt((p.x - arenaMin.x) / tamanhoCelula), 0, largura - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt((p.y - arenaMin.y) / tamanhoCelula), 0, altura - 1);
        return new Vector2Int(x, y);
    }
}