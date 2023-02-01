using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    //他のクラスに渡す
    public bool trigger { get; set; } = false;　//カメラの移動トリガー
    GameObject ScriptGO;
    GameSystem gamesystem;
    Player player;
    void Start()
    {

        ScriptGO = GameObject.Find("Scripts");
        gamesystem = ScriptGO.GetComponent<GameSystem>();
        player = GameObject.Find("Player").GetComponent<Player>();
        //Item出現時の動き
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
        if (this.gameObject.name == "CameraTrigger")　　//このスクリプトがカメラトリガーにアタッチされている場合
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
        if (this.gameObject.CompareTag("Item"))      //このスクリプトがアイテムにアタッチされている場合
        {
            if (collision.gameObject.name == "Player")
            {
                SoundEffect.SETrigger[1] = true;  //効果音再生
                gamesystem.brightness += 0.1f;　　//画面の明るさ上昇
                gamesystem.score += 100;          //スコア上昇
                Destroy(this.gameObject);         //アイテムの削除
            }
        }
    }
}
