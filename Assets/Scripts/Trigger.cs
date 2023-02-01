using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    //���̃N���X�ɓn��
    public bool trigger { get; set; } = false;�@//�J�����̈ړ��g���K�[
    GameObject ScriptGO;
    GameSystem gamesystem;
    Player player;
    void Start()
    {

        ScriptGO = GameObject.Find("Scripts");
        gamesystem = ScriptGO.GetComponent<GameSystem>();
        player = GameObject.Find("Player").GetComponent<Player>();
        //Item�o�����̓���
        for (int i = 0; i < 3; i++)
        {
            if (this.gameObject.name.Contains("item"))
            {
                if(this.gameObject.name.Contains((i + 1).ToString()))
                {
                    Rigidbody2D itemrb = this.GetComponent<Rigidbody2D>();
                    Vector2 force = new Vector2((i - 1) * 1.5f, 3);
                    itemrb.AddForce(force);
                }              
            }
        }
    }



    void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.gameObject.name == "CameraTrigger")�@�@//���̃X�N���v�g���J�����g���K�[�ɃA�^�b�`����Ă���ꍇ
        {
            if (collision.gameObject.name == "Player")
            {
                trigger = true;
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (this.gameObject.name == "legCol")
        {
            if (collision.gameObject.CompareTag("Flore"))
            {
                player.legOnFlore = true;
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (this.gameObject.name == "legCol")
        {
            if (collision.gameObject.CompareTag("Flore"))
            {
                player.legOnFlore = false;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.gameObject.CompareTag("Item"))      //���̃X�N���v�g���A�C�e���ɃA�^�b�`����Ă���ꍇ
        {
            if (collision.gameObject.name == "Player")
            {
                SoundEffect.SETrigger[1] = true;  //���ʉ��Đ�
                gamesystem.brightness += 0.1f;�@�@//��ʂ̖��邳�㏸
                gamesystem.score += 100;          //�X�R�A�㏸
                Destroy(this.gameObject);         //�A�C�e���̍폜
            }
        }
    }
}
