using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb;
    AudioSource audio;
    public float speed;

    public AudioClip somDano;

    public float tempoInvencivel = 1.5f;
    private float timerInvencivel = 0f;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (timerInvencivel > 0f){
            timerInvencivel -= Time.deltaTime;
            // pisca enquanto invencível
            sr.enabled = Mathf.FloorToInt(timerInvencivel * 10f) % 2 == 0;
        } else {
            sr.enabled = true;
        }
    }

    void FixedUpdate()
    {
        if (GameController.gameOver) return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        rb.MovePosition(rb.position + movement.normalized * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other){
        if (other.tag == "Coletavel"){
            audio.Play();
            Destroy(other.gameObject);
            GameController.Collect();
        }
    }

    private void OnCollisionStay2D(Collision2D other){
        if (other.gameObject.tag == "Inimigo" && timerInvencivel <= 0f && !GameController.gameOver){
            GameController.LevarDano();
            timerInvencivel = tempoInvencivel;
            audio.PlayOneShot(somDano);
        }
    }
}