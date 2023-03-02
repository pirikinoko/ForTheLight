using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //�v���C���[����֘A
    private Rigidbody2D playerrb;�@//Rigidbody2D�擾
    float jumpForce = 5;�@�@�@�@�@//�W�����v��
    float speed = 90, maxSpeed = 3;�@�@//�ړ������x,�ō����x
    float movement, stopper = 8;    //�ړ����x,�����x
    float attackSign = 1;             //scale.x �̕���
    Vector2 characterDirection; //�L�����N�^�[�̌���
    //�A�j���[�V�����֘A
    [SerializeField]
    private Animator playerAnim;
    bool isMoving = false;             //�ړ�����
    bool onFlore = false;              //�ڒn���Ă��邩
    public bool legOnFlore = false;
    bool colorChanging = false;
    bool idleAnim, walkAnim, whileAtack;
    [System.NonSerialized] public bool jumpAnim;
    float attackDuration = 1.2f, colorChangeDuration = 0.3f;
    //�Q�[���I�u�W�F�N�g
    private GameObject ScriptGO;   //GameSystem�X�N���v�g�̃A�^�b�`����Ă���I�u�W�F�N�g
    private GameObject attackCol;  //�U���̔���
    public Renderer attckColRend, legColRend;
     //���W
    [SerializeField]
    Vector2 defaultPos;

    GameSystem gamesystem;   //GameSystem�^

    [SerializeField] Text DebugText;
    // Start is called before the first frame update
    void Start()
    {
        colorChangeDuration = 0.3f;
        //�����ʒu�Ɉړ�
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


    //�����蔻��
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Flore"))�@�@//�ڒn�����Ƃ�
        {
            onFlore = true;
        }
        if (collision.gameObject.name == "Goal")�@�@�@�@//�S�[�������Ƃ�
        {
            gamesystem.inPlay = false;
            gamesystem.isOver = true;
        }
        if (collision.gameObject.CompareTag("Enemy"))�@�@�@�@//�G�ɓ��������Ƃ�
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            colorChanging = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Flore"))�@�@�@//�n�ʂ��痣�ꂽ�Ƃ�
        {
            onFlore = false;
        }
    }
    void GetHit()
    {
        if (colorChanging)�@�@�@//�G�ɓ������Ă��܂��ƃv���C���[���ԐF��
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
        //�v���C���[�̈ʒu���擾
        Vector2 position = transform.position;
        //movement�̕����擾
        float sign = Mathf.Sign(movement);

        //A�L�[�ŕ��̌����Ɉړ�
        if (Input.GetKey(KeyCode.A))
        {
            if (movement > -maxSpeed)
            {
                movement -= speed * Time.deltaTime;
            }

            if (onFlore && whileAtack == false)//�A�j���[�V����
            {
                walkAnim = true;
            }       
            isMoving = true;
            //�L�����N�^�[�̌��� 
            characterDirection = new Vector2(sign * 0.5f, 0.5f);
        }

        //D�L�[�Ő��̌����Ɉړ�
        else if (Input.GetKey(KeyCode.D))
        {
            if (movement < maxSpeed)
            {
                movement += speed * Time.deltaTime;
            }

            if (onFlore && whileAtack == false)//�A�j���[�V����
            {
                walkAnim = true;
            }
            isMoving = true;
            //�L�����N�^�[�̌��� 
            characterDirection = new Vector2(sign * 0.5f, 0.5f);
        }

        //�L�[�𗣂����Ƃ�
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            isMoving = false;
            walkAnim = false;  //�A�j���[�V����OFF
        }
        //�󒆂ɂ���Ƃ�
        if (onFlore == false)
        {
            walkAnim = false;  //�A�j���[�V����OFF
        }
       
       
        if (movement != 0 && !(whileAtack)) { gameObject.transform.localScale = characterDirection; } //�����Ă��Ȃ��Ƃ�(movement = 0)�͉E�����ɂȂ��Ă��܂��̂Ŕ��f���Ȃ�

        //���X�Ɍ���
        sign *= -1;�@//�����Ȃ̂�movement�̋t�̕�����������
        if(movement != 0)
        {
            if (isMoving) { movement += sign * (stopper * Time.deltaTime); }
            else { movement = 0; }�@//�ړ��L�[�𗣂��Ƃ���Ɍ���
        }
      

        //�ړ����f
        position.x += movement * Time.deltaTime;
        transform.position = position;

    }

    float FillDifference(float num1, float num2, float fillRate)�@�@//�����k�߂�֐�
    {
        float sign;
        if (Mathf.Abs(num2 - num1) > 0f)
        {
            sign = Mathf.Sign(num2 - num1);
            num1 += fillRate * sign * Time.deltaTime;

        }
        return num1;
    }

    void Attack()�@//�U������
    {
        Vector2 colPos = this.transform.position;�@�@
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
            maxSpeed = 1.5f;   //�U�����A�ړ����x����
            jumpAnim = false;
        }
        else { maxSpeed = 3f; }

        if(attackDuration < 0)
        {
            whileAtack = false;
            idleAnim = true;
            attackDuration = 1.2f;
        }

        if(0.15f < attackDuration && attackDuration < 0.43f)   //�U���̓����蔻��ON
        {
            attackCol.gameObject.SetActive(true);  
        }
        else { attackCol.gameObject.SetActive(false); }

        colPos.x += attackSign * 0.6f;
        attackCol.transform.position = colPos;

    }

    void Jump()�@�@//�W�����v
    {
        GameObject legCol = GameObject.Find("legCol");�@�@//���̓����蔻��
        Vector2 pos = this.transform.position;
        pos.y -= 0.8f;
        legCol.transform.position = pos;       
        if (Input.GetKeyDown(KeyCode.Space) && legOnFlore) //�L�[�������ꂽ�Ƃ�&&�n�ʂɂ���Ƃ�
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
