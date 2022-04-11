using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer chSprite;
    private BoxCollider2D boxCollider2D;
    private DamageScript dm;

    private bool stop = false; //Если персонажу нужно остановиться указать true

    [SerializeField] private float stopTime = 2f;
    [SerializeField] private int damage = 20; // Базове значення
    [SerializeField] private float attackTime = 1f;
    [SerializeField] private float nextAttack = 0f;
    [SerializeField] private float attackRange;
    [SerializeField] private float speed = 10f;
    private GameObject platform;

    private void Start()
    {
        dm = GetComponent<DamageScript>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        chSprite = GetComponent<SpriteRenderer>();
    }

    private IEnumerator AttackTime() //Зупинка на атаку(анімацію) якщо не попадає у ворога
    {
        stop = true;
        yield return new WaitForSeconds(stopTime);
        stop = false;
    }

    private IEnumerator AttackTime(Collider2D hit) //Зупинка на атаку(анімацію)
    {
        stop = true;
        yield return new WaitForSeconds(stopTime / 2);
        hit.GetComponent<DamageScript>().TakeDamage(damage);
        yield return new WaitForSeconds(stopTime);
        stop = false;
    }

    //Проверка на нахождение персонажа на земле
    private bool isGround()
    {
        LayerMask ground = LayerMask.GetMask("Ground"); //Указание маски земли или платформы
                                                        //Создание квадрата под персонажем для проверки прикасания к земле
        RaycastHit2D isGround = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, 0.1f, ground);
        return isGround.collider != null; //возвращает true если персонаже не касается земли     
    }

    private void Jump()
    {
        // Указания для прыжка
        if (Input.GetButtonDown("Jump") && isGround())
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y + 25f);
            animator.SetBool("isJump", true);
        }
        else if (!isGround()) { animator.SetBool("isFall", true); } //если персонаж не на земле включает анимацию падения
        else { animator.SetBool("isJump", false); animator.SetBool("isFall", false); }  //выключение анимации падения
    }

    private void Run()
    {
        float dirX = Input.GetAxisRaw("Horizontal");

        if (dirX < 0) { chSprite.flipX = true; } else if (dirX > 0) { chSprite.flipX = false; } // Инверсия спрайта в зависимости от стороны движения
        if (dirX != 0) { animator.SetBool("isRun", true); } else { animator.SetBool("isRun", false); } // Включение анимации движения
        if (stop) { dirX = 0f; } 
        rigidBody.velocity = new Vector2(dirX * speed, rigidBody.velocity.y); // Смена х координат для движения
    }

    private void Update()
    {
        if (dm.isDeath)
        {
            animator.SetTrigger("isDeath");
            Destroy(boxCollider2D);
            Destroy(rigidBody);
            Destroy(this);
        }
        Attack();
        Run();
        Jump();

        if (Input.GetKey(KeyCode.S) && platform != null)
        {
            StartCoroutine(IgnoreCollision());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            platform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            platform = null;
        }
    }

    private void Attack()
    {
        if (nextAttack <= 0)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                animator.SetTrigger("isAttack"); //Анімація
                LayerMask enemyLayer = LayerMask.GetMask("Enemy");

                //Створення радіусу атаки
                Collider2D hitEnemy = Physics2D.OverlapCircle(new Vector2(boxCollider2D.bounds.center.x + 0.5f, boxCollider2D.bounds.center.y - 0.75f), attackRange, enemyLayer);

                // Якщо попав
                if (hitEnemy != null) { StartCoroutine(AttackTime(hitEnemy)); }
                else StartCoroutine(AttackTime()); //Зупинка на атаку

                nextAttack = attackTime;
            }
        }
        else { nextAttack -= Time.deltaTime; }
    }

    private IEnumerator IgnoreCollision()
    {
        CompositeCollider2D platformCollider = platform.GetComponent<CompositeCollider2D>();
        Physics2D.IgnoreCollision(boxCollider2D, platformCollider);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isFall", true);
        Physics2D.IgnoreCollision(boxCollider2D, platformCollider, false);
    }
}