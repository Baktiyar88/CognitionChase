
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{

    public GameManager manager;
    [SyncVar(hook = nameof(OnMelonChanged))]
    public int melon;

    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;
    private Camera mainCam;
    private bool frozen = false;

    [SerializeField] private LayerMask jumpableGround;
    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private AudioSource jumpSoundEffect;

    private enum MovementState { idle, running, jumping, falling }

    // Синхронизация анимационного состояния с использованием команды
    [SyncVar(hook = nameof(OnAnimStateChanged))]
    private int animState;

    // Синхронизированное значение для флипа спрайта
    [SyncVar(hook = nameof(OnFlipChanged))]
    private bool isFlipped;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
       


    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        SetFrozen(false); // включить управление при старте уровня
    }


    private void Update()
    {
        if (!isLocalPlayer || frozen) return;


        manager.globalMelonText.text = "Melonss:" + manager.globalMelons;
        manager.melonText.text = "you point: " + melon;


        dirX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            jumpSoundEffect.Play();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        UpdateAnimationState();
        CameraMovement();
    }

    private void OnMelonChanged(int oldValue, int newValue)
    {
        if (!isLocalPlayer) return;
        // Обновляем локальный UI-элемент с личным счётом
        manager.melonText.text = "Собрано: " + newValue;
    }

    public void SetFrozen(bool state)
    {
        frozen = state;
        if (frozen)
        {
            rb.linearVelocity = Vector2.zero;   // сразу обнуляем скорость  
            anim.SetInteger("state", 0);  // ставим idle  
        }
    }

    private void CameraMovement()
    {
        mainCam.transform.localPosition = new Vector3(transform.position.x, transform.position.y, -1f);
        transform.position = Vector2.MoveTowards(transform.position, mainCam.transform.localPosition, Time.deltaTime);
    }

    // Команда для обновления флипа на сервере
    [Command]
    private void CmdSetFlip(bool flip)
    {
        isFlipped = flip;
    }

    // Команда для обновления анимационного состояния на сервере
    [Command]
    private void CmdUpdateAnimState(int state)
    {
        animState = state;
    }

    private void UpdateAnimationState()
    {
        if (!isLocalPlayer) return;

        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            CmdSetFlip(false);
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            CmdSetFlip(true);
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.linearVelocity.y > 0.001f)
        {
            state = MovementState.jumping;
        }
        else if (rb.linearVelocity.y < -0.001f)
        {
            state = MovementState.falling;
        }

        // Отправляем серверу новое состояние только если оно изменилось (для снижения количества вызовов)
        int intState = (int)state;
        if (intState != animState)
        {
            CmdUpdateAnimState(intState);
        }

        // Локально для мгновенного фидбэка обновляем анимацию и спрайт
        anim.SetInteger("state", intState);
        sprite.flipX = isFlipped;
    }

    private void OnFlipChanged(bool oldValue, bool newValue)
    {
        sprite.flipX = newValue; // Обновляем на клиентах
    }

    private void OnAnimStateChanged(int oldState, int newState)
    {
        anim.SetInteger("state", newState); // Обновляем анимацию на клиентах
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer)
        {
            anim.SetInteger("state", animState);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }


}

