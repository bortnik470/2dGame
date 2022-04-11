using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer chSprite;
    private BoxCollider2D boxCollider2D;
    private DamageScript dm;

    private bool stop = false; //���� ��������� ����� ������������ ������� true

    [SerializeField] private float stopTime = 2f;
    [SerializeField] private int damage = 20; // ������ ��������
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

    private IEnumerator AttackTime() //������� �� �����(�������) ���� �� ������ � ������
    {
        stop = true;
        yield return new WaitForSeconds(stopTime);
        stop = false;
    }

    private IEnumerator AttackTime(Collider2D hit) //������� �� �����(�������)
    {
        stop = true;
        yield return new WaitForSeconds(stopTime / 2);
        hit.GetComponent<DamageScript>().TakeDamage(damage);
        yield return new WaitForSeconds(stopTime);
        stop = false;
    }

    //�������� �� ���������� ��������� �� �����
    private bool isGround()
    {
        LayerMask ground = LayerMask.GetMask("Ground"); //�������� ����� ����� ��� ���������
                                                        //�������� �������� ��� ���������� ��� �������� ���������� � �����
        RaycastHit2D isGround = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, 0.1f, ground);
        return isGround.collider != null; //���������� true ���� ��������� �� �������� �����     
    }

    private void Jump()
    {
        // �������� ��� ������
        if (Input.GetButtonDown("Jump") && isGround())
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y + 25f);
            animator.SetBool("isJump", true);
        }
        else if (!isGround()) { animator.SetBool("isFall", true); } //���� �������� �� �� ����� �������� �������� �������
        else { animator.SetBool("isJump", false); animator.SetBool("isFall", false); }  //���������� �������� �������
    }

    private void Run()
    {
        float dirX = Input.GetAxisRaw("Horizontal");

        if (dirX < 0) { chSprite.flipX = true; } else if (dirX > 0) { chSprite.flipX = false; } // �������� ������� � ����������� �� ������� ��������
        if (dirX != 0) { animator.SetBool("isRun", true); } else { animator.SetBool("isRun", false); } // ��������� �������� ��������
        if (stop) { dirX = 0f; } 
        rigidBody.velocity = new Vector2(dirX * speed, rigidBody.velocity.y); // ����� � ��������� ��� ��������
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
                animator.SetTrigger("isAttack"); //�������
                LayerMask enemyLayer = LayerMask.GetMask("Enemy");

                //��������� ������ �����
                Collider2D hitEnemy = Physics2D.OverlapCircle(new Vector2(boxCollider2D.bounds.center.x + 0.5f, boxCollider2D.bounds.center.y - 0.75f), attackRange, enemyLayer);

                // ���� �����
                if (hitEnemy != null) { StartCoroutine(AttackTime(hitEnemy)); }
                else StartCoroutine(AttackTime()); //������� �� �����

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