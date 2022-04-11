using UnityEngine;
using System.Collections;

namespace MainScriptt
{
    public class MainScript : MonoBehaviour
    {
        private Rigidbody2D rigidBody;
        private Animator animator;
        private SpriteRenderer chSprite;
        private BoxCollider2D boxCollider2D;
        private DamageScript dm;

        private Vector2 moveCoord; //������ ��� �������� ��������� �������� � - ����� ����������, � - ������

        private bool goRight = true; //��������� ����� �� �������� ���� �� �����
        private bool stop = false; //���� ��������� ����� ������������ ������� true

        [SerializeField] private float stopTime = 2f;
        [SerializeField] private int damage = 20; // ������ ��������
        [SerializeField] private float attackTime = 1f;
        [SerializeField] private float nextAttack = 0f;
        [SerializeField] private float attackRange;
        [SerializeField] private float speed = 10f;

        private IEnumerator StayTime() //��������� ��������� �� 2 �������
        {
            stop = true;
            yield return new WaitForSeconds(2f);
            goRight = !goRight;
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

        private void setCoord(Vector2 coord) //��������� ��������� �������� ��� coord �������� �������� ���������
        {
            float curentPos = transform.position.x;
            moveCoord = new Vector2(curentPos + coord.x, curentPos + coord.y);
        }

        private void setComp()
        {
            dm = GetComponent<DamageScript>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody2D>();
            chSprite = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            setComp();
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
        }

        private void Run() // ��� �� ������� ����������� 
        {
            float dirX = 0; //������� �������� ���������

            if (!goRight) { chSprite.flipX = true; } else if (goRight) { chSprite.flipX = false; } // �������� ������� � ����������� �� ������� ��������
            if (!stop) 
            {
                if (transform.position.x >= moveCoord.x && !goRight) { dirX = -1f; }  // ������� ��� �������� � ����� �������
                else if (transform.position.x <= moveCoord.y && goRight) { dirX = 1f; }  // ������� ��� �������� � ������ �������
                else { StartCoroutine(StayTime()); } //���� �������� ������ ������� ���������� ���������������� � �������������� ������� ������

                animator.SetBool("isRun", true);
            } 
            else { animator.SetBool("isRun", false); } // ���������� �������� ��������
            rigidBody.velocity = new Vector2(dirX * speed, rigidBody.velocity.y); // ����� � ��������� ��� ��������
        }

        private void Attack()
        {
            LayerMask PlayerMask = LayerMask.GetMask("Player");
            Vector2 side = Vector2.zero;

            if (goRight) { side = Vector2.right; }
            else if (!goRight) { side = Vector2.left; }

            RaycastHit2D hit = Physics2D.Raycast(boxCollider2D.bounds.center, side, attackRange, PlayerMask);

            if (hit.collider != null && nextAttack <= 0)
            {
                StartCoroutine(AttackTime(hit.collider)); //������� �� �����
                animator.SetTrigger("isAttack"); //������� 
                nextAttack = attackTime;
            }
            else if (nextAttack >= -1) { nextAttack -= Time.deltaTime; }
        }
    }
}