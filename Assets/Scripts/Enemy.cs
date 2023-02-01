using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    float speed, jumpForce = 5f, elapsedTime1 = 0, elapsedTime2 = 0, turnSpan = 1, jumpSpan = 2, rndFloat = 0;

    Vector2 characterDirection, monsterPos, scale;
    [SerializeField] Rigidbody2D rb2D;
    GameObject item;
    bool onSurface  = false;
    // Start is called before the first frame update
    void Start()
    {
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        rndFloat = 0;
        speed = 1.3f;
        elapsedTime1 = 0;
        elapsedTime2 = 0;
        Vector2 characterDirection = new Vector2(0.01f, 0.01f);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {

        monsterPos = this.transform.position;
        monsterPos.x += speed * Time.deltaTime;
        this.transform.position = monsterPos;
        //��b���ƂɊm���ŃL�������]�����Ԍo�߂Ŕ��]��UP
        elapsedTime1 += Time.deltaTime;   //���Ԍv��
        elapsedTime2 += Time.deltaTime;
        float prob1 = 10 + elapsedTime1 * 5; //���Ԍo�߂Ŋm���㏸
        float prob2 = 10 + elapsedTime2 * 5;�@//���Ԍo�߂Ŋm���㏸
        if (elapsedTime1 > turnSpan)
        {
            rndFloat = Random.Range(-5, 10);
            turnSpan++;
            if (prob1 * rndFloat > 100)
            {
                Turn();
                turnSpan = 1;
                elapsedTime1 = 0;
            }
        }
        if (elapsedTime2 > jumpSpan)
        {
            rndFloat = Random.Range(-5, 10);
            jumpSpan++;
            if (prob2 * rndFloat > 100)
            {
                Jump();
                jumpSpan = 1;
                elapsedTime2 = 0;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "attackCol")�@�@//���ƏՓ�(�v���C���[�̏���)
        {
            SoundEffect.SETrigger[2] = true;
            //Item�h���b�v
            Vector2 pos = this.transform.position;
            for (int i = 0; i < 3; i++)
            {
                item = (GameObject)Resources.Load("Item");
                item.gameObject.name = "item" + (i + 1).ToString();
                pos.x += 0.1f;
                Instantiate(item, pos, Quaternion.identity);
            }

            //�X���C��������
            Destroy(this.gameObject);
        }

    }
    void OnCollisionStay2D(Collision2D other)
    {
        onSurface = true;
        if (other.gameObject.name == "Player")  //�v���C���[�ƏՓ�(�G�̏���)
        {
            SoundEffect.SETrigger[6] = true;
            //�X���C��������
            Destroy(this.gameObject);
        }
    }
    void Jump()
    {
        if (onSurface)
        {
            rb2D.velocity = new Vector2(0, jumpForce);
            onSurface = false;
        }       
    }
    void Turn()
    {
        speed *= -1;
        Vector2 characterDirection = gameObject.transform.localScale;
        characterDirection.x *= -1;
        gameObject.transform.localScale = characterDirection;
    }
}
