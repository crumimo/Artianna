using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public AudioSource footstepsAudio; // Ссылка на AudioSource для шагов
    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 movement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Ищем аниматор на текущем объекте

        if (footstepsAudio == null)
        {
            Debug.LogWarning("Footsteps AudioSource is not assigned!");
        }
    }

    private void Update()
    {
        // Получаем ввод от пользователя
        movement.x = Input.GetAxisRaw("Horizontal");

        // Задаём скорость для анимации
        float speedValue = movement.sqrMagnitude;
        animator.SetFloat("Speed", speedValue);

        // Отражаем спрайт при движении влево/вправо
        if (movement.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(movement.x), 1, 1);
        }

        // Управляем звуком шагов
        HandleFootstepSound(speedValue);
    }

    private void FixedUpdate()
    {
        // Движение персонажа
        rb.velocity = movement.normalized * speed;
    }

    private void HandleFootstepSound(float speedValue)
    {
        if (footstepsAudio == null) return;

        // Если персонаж движется и звук ещё не играет
        if (speedValue > 0 && !footstepsAudio.isPlaying)
        {
            footstepsAudio.Play();
        }
        // Если персонаж стоит и звук играет
        else if (speedValue == 0 && footstepsAudio.isPlaying)
        {
            footstepsAudio.Pause();
        }
    }
}