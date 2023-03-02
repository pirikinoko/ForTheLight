using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //プレイヤー操作関連
    private Rigidbody2D playerrb;　//Rigidbody2D取得
    float jumpForce = 5;　　　　　//ジャンプ力
    float speed = 90, maxSpeed = 3;　　//移動加速度,最高速度
    float movement, stopper = 8;    //移動速度,減速度
    float attackSign = 1;             //scale.x の符号
    Vector2 characterDirection; //キャラクターの向き
    //アニメーション関連
    [SerializeField]
    private Animator playerAnim;
    bool isMoving = false;             //移動中か
    bool onFlore = false;              //接地しているか
    public bool legOnFlore = false;
    bool colorChanging = false;
    bool idleAnim, walkAnim, whileAtack;
    [System.NonSerialized] public bool jumpAnim;
    float attackDuration = 1.2f, colorChangeDuration = 0.3f;
    //ゲームオブジェクト
    private GameObject ScriptGO;   //GameSystemスクリプトのアタッチされているオブジェクト
    private GameObject attackCol;  //攻撃の判定
    public Renderer attckColRend, legColRend;
     //座標
    [SerializeField]
    Vector2 defaultPos;

    GameSystem gamesystem;   //GameSystem型

    [SerializeField] Text DebugText;
    // Start is called before the first frame update
    void Start()
    {
        colorChangeDuration = 0.3f;
        //初期位置に移動
        this.gameObject.transform.position = defaultPos;     
        movement = 0;
        isMoving = false;
        attackCol = GameObject.Find("attackCol");
        attackCol.gameObject.SetActive(false);
        ScriptGO = GameObject.Find("Scripts");
        attckColRend.enabled = false;
        legColRend.enabled = false; 
        colorChanging = false;
        gamesystem = ScriptGO.GetComponent<GameSystem>();
        playerrb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gamesystem.inPlay)
        {
            Move();
            Jump();
            Attack();
            GetHit();
            PlayAnimation();
        }
        else
        {
            this.gameObject.transform.position = defaultPos;
            playerrb.velocity = new Vector2(0, 0);
        }
    }


    //当たり判定
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Flore"))　　//接地したとき
        {
            onFlore = true;
        }
        if (collision.gameObject.name == "Goal")　　　　//ゴールしたとき
        {
            gamesystem.inPlay = false;
            gamesystem.isOver = true;
        }
        if (collision.gameObject.CompareTag("Enemy"))　　　　//敵に当たったとき
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            colorChanging = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Flore"))　　　//地面から離れたとき
        {
            onFlore = false;
        }
    }
    void GetHit()
    {
        if (colorChanging)　　　//敵に当たってしまうとプレイヤーが赤色に
        {
            colorChangeDuration -= Time.deltaTime;
            if(colorChangeDuration < 0)
            {
                GetComponent<SpriteRenderer>().color = Color.white;
                colorChanging = false;
                colorChangeDuration = 0.3f;
            }
        }
    }
    void Move()
    {
        //プレイヤーの位置を取得
        Vector2 position = transform.position;
        //movementの符号取得
        float sign = Mathf.Sign(movement);

        //Aキーで負の向きに移動
        if (Input.GetKey(KeyCode.A))
        {
            if (movement > -maxSpeed)
            {
                movement -= speed * Time.deltaTime;
            }

            if (onFlore && whileAtack == false)//アニメーション
            {
                walkAnim = true;
            }       
            isMoving = true;
            //キャラクターの向き 
            characterDirection = new Vector2(sign * 0.5f, 0.5f);
        }

        //Dキーで正の向きに移動
        else if (Input.GetKey(KeyCode.D))
        {
            if (movement < maxSpeed)
            {
                movement += speed * Time.deltaTime;
            }

            if (onFlore && whileAtack == false)//アニメーション
            {
                walkAnim = true;
            }
            isMoving = true;
            //キャラクターの向き 
            characterDirection = new Vector2(sign * 0.5f, 0.5f);
        }

        //キーを離したとき
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            isMoving = false;
            walkAnim = false;  //アニメーションOFF
        }
        //空中にいるとき
        if (onFlore == false)
        {
            walkAnim = false;  //アニメーションOFF
        }
       
       
        if (movement != 0 && !(whileAtack)) { gameObject.transform.localScale = characterDirection; } //動いていないとき(movement = 0)は右向きになってしまうので反映しない

        //徐々に減速
        sign *= -1;　//減速なのでmovementの逆の符号を加える
        if(movement != 0)
        {
            if (isMoving) { movement += sign * (stopper * Time.deltaTime); }
            else { movement = 0; }　//移動キーを離すとさらに減速
        }
      

        //移動反映
        position.x += movement * Time.deltaTime;
        transform.position = position;

    }

    float FillDifference(float num1, float num2, float fillRate)　　//差を縮める関数
    {
        float sign;
        if (Mathf.Abs(num2 - num1) > 0f)
        {
            sign = Mathf.Sign(num2 - num1);
            num1 += fillRate * sign * Time.deltaTime;

        }
        return num1;
    }

    void Attack()　//攻撃処理
    {
        Vector2 colPos = this.transform.position;　　
        if (Input.GetMouseButtonDown(0) && !(whileAtack))
        {          
            walkAnim = false;
            idleAnim = true;
            whileAtack = true;
            playerAnim.SetTrigger("attack");
            attackSign = Mathf.Sign(gameObject.transform.localScale.x);
        }

        if (whileAtack)
        {
            attackDuration -= Time.deltaTime;
            maxSpeed = 1.5f;   //攻撃時、移動速度減少
            jumpAnim = false;
        }
        else { maxSpeed = 3f; }

        if(attackDuration < 0)
        {
            whileAtack = false;
            idleAnim = true;
            attackDuration = 1.2f;
        }

        if(0.15f < attackDuration && attackDuration < 0.43f)   //攻撃の当たり判定ON
        {
            attackCol.gameObject.SetActive(true);  
        }
        else { attackCol.gameObject.SetActive(false); }

        colPos.x += attackSign * 0.6f;
        attackCol.transform.position = colPos;

    }

    void Jump()　　//ジャンプ
    {
        GameObject legCol = GameObject.Find("legCol");　　//足の当たり判定
        Vector2 pos = this.transform.position;
        pos.y -= 0.8f;
        legCol.transform.position = pos;       
        if (Input.GetKeyDown(KeyCode.Space) && legOnFlore) //キーが押されたとき&&地面にいるとき
        {
            playerrb.velocity = new Vector2(0f, jumpForce);
            walkAnim = false;
            idleAnim = true;
            jumpAnim = true;
            legOnFlore = false;
        }

        if(onFlore && legOnFlore)
        {
            jumpAnim = false;
        }
    }
    void PlayAnimation()
    {
        playerAnim.SetBool("idle", idleAnim);
        playerAnim.SetBool("walk", walkAnim);
        playerAnim.SetBool("jump", jumpAnim);
    }
       

}
