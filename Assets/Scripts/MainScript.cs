using UnityEngine;
using System.Collections;

namespace MainScriptt
{
    public class MainScript : MonoBehaviour
    {
        protected Rigidbody2D rigidBody;
        protected Animator animator;
        protected SpriteRenderer chSprite;
        protected BoxCollider2D boxCollider2D;
        private DamageScript dm;

        private Vector2 moveCoord; //Вектор для хранение координат движения х - левая координата, у - правая

        protected bool goRight = true; //Указывает может ли персонаж идти на право
        protected bool stop = false; //Если персонажу нужно остановиться указать true
        
        public float stopTime = 2f;
        public int damage = 20; // Базове значення
        public float attackTime = 1f;
        public float nextAttack = 0f;
        public float attackRange;
        public float speed = 10f;

        private IEnumerator StayTime() //Остановка персонажа на 2 секунды
        {
            stop = true;
            yield return new WaitForSeconds(2f);
            goRight = !goRight;
            stop = false;
        }

        public IEnumerator AttackTime() //Зупинка на атаку(анімацію)
        {
            stop = true;
            yield return new WaitForSeconds(stopTime);
            stop = false;
        }

        public IEnumerator AttackTime(Collider2D hit) //Зупинка на атаку(анімацію)
        {
            stop = true;
            yield return new WaitForSeconds(stopTime / 2);
            hit.GetComponent<DamageScript>().TakeDamage(damage);
            yield return new WaitForSeconds(stopTime);
            stop = false;
        }

        public void setCoord(Vector2 coord) //Установка координат движения где coord указывет смещение персонажа
        {
            float curentPos = transform.position.x;
            moveCoord = new Vector2(curentPos + coord.x, curentPos + coord.y);
        }

        public void setComp()
        {
            dm = GetComponent<DamageScript>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody2D>();
            chSprite = GetComponent<SpriteRenderer>();
        }

        virtual public void Start()
        {
            setComp();
        }

        virtual public void Update()
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

        virtual public void Run() // Бег по заданым координатам 
        {
            float dirX = 0; //Сторона смещения персонажа

            if (!goRight) { chSprite.flipX = true; } else if (goRight) { chSprite.flipX = false; } // Инверсия спрайта в зависимости от стороны движения
            if (!stop) 
            {
                if (transform.position.x >= moveCoord.x && !goRight) { dirX = -1f; }  // Условия для движения в левую сторону
                else if (transform.position.x <= moveCoord.y && goRight) { dirX = 1f; }  // Условия для движения в правую сторону
                else { StartCoroutine(StayTime()); } //если персонаж достиг крайней координаты останавливаеться и переключаеться сторона ходьбы

                animator.SetBool("isRun", true);
            } 
            else { animator.SetBool("isRun", false); } // Выключение анимации движения
            rigidBody.velocity = new Vector2(dirX * speed, rigidBody.velocity.y); // Смена х координат для движения
        }

        virtual public void Attack()
        {
            LayerMask PlayerMask = LayerMask.GetMask("Player");
            Vector2 side = Vector2.zero;

            if (goRight) { side = Vector2.right; }
            else if (!goRight) { side = Vector2.left; }

            RaycastHit2D hit = Physics2D.Raycast(boxCollider2D.bounds.center, side, attackRange, PlayerMask);

            if (hit.collider != null && nextAttack <= 0)
            {
                StartCoroutine(AttackTime(hit.collider)); //Зупинка на атаку
                animator.SetTrigger("isAttack"); //Анімація 
                nextAttack = attackTime;
            }
            else if (nextAttack >= -1) { nextAttack -= Time.deltaTime; }
        }
    }
}