using MainScriptt;
using UnityEngine;
using System.Collections;

public class Character : MainScript
{
    private GameObject platform;

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

    public override void Run()
    {
        float dirX = Input.GetAxisRaw("Horizontal");

        if (dirX < 0) { chSprite.flipX = true; } else if (dirX > 0) { chSprite.flipX = false; } // �������� ������� � ����������� �� ������� ��������
        if (dirX != 0) { animator.SetBool("isRun", true); } else { animator.SetBool("isRun", false); } // ��������� �������� ��������
        if (stop) { dirX = 0f; } 
        rigidBody.velocity = new Vector2(dirX * speed, rigidBody.velocity.y); // ����� � ��������� ��� ��������
    }

    public override void Update()
    {
        base.Update();
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

    public override void Attack()
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
