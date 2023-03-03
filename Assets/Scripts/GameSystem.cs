using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Experimental.Rendering.Universal;

public class GameSystem : MonoBehaviour
{
    //���l
    public float score { get; set; } = 0;
    public float brightness { get; set; } = 1;   //��ʂ̖��邳
    int flored;
    float finalScore = 0, bonusScore = 0, bonusRate, highScore;
    float time = 0, startTime = 3, soundSpan = 1, finalTime, finalBrightness;
    float decreaseRate = 0.04f;�@�@�@//1�t���[�����Ƃ̖��邳�����l
    int num = 2;
    int numberOfEnemyLimit;
    [SerializeField] Text scoreText, finalScoreText, finalBrightnessText, highScoreText;�@//�X�R�A���^�C���e�L�X�g
    //�Q�[���I�u�W�F�N�g,�e�L�X�g
    private GameObject pointLight, spriteLight, inPlayUI;             //�Ɩ�,�X�R�A,�^�C���̃Q�[���I�u�W�F�N�g,�J�E���g�_�E���e�L�X�g,���̖��邳�Q�[�W
    public GameObject[] moonGage;
    public GameObject startPanel, resultPanel;                  //�p�l��
    GameObject[] allObjects;
    Text coundDownText;
    //�R���C�_�[
    Collider2D player;
    Collider2D[] enemyLimits;
    //����
    [System.NonSerialized] public bool inPlay = false, isOver = false , startFlag = false;
    bool ignore = true;
    // Start is called before the first frame update
    void Start()
    {

        num = 2;
        startFlag = false;
        inPlay = false;
        isOver = false;  
        score = 0;
        finalScore = 0;
        time = 0;
        startTime = 3;
        soundSpan = 1;
        finalTime = 0;
        inPlayUI = GameObject.Find("inPlay");
        coundDownText = GameObject.Find("CountDownText").GetComponent<Text>();
        pointLight = GameObject.Find("Point Light 2D");
        spriteLight = GameObject.Find("Moon");
        inPlayUI.gameObject.SetActive(false);
        startPanel.gameObject.SetActive(true);
        resultPanel.gameObject.SetActive(false);
        brightness = 1;

        //�X���C���̈ړ������p�̕ǁA�v���C���[�݂̂Ƃ�������
        allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject target in allObjects)        //�ǂ̐��𐔂���
        {
            if (target.name.Contains("EnemyLimit"))
            {
                numberOfEnemyLimit++;
            }
        }
        enemyLimits = new Collider2D[numberOfEnemyLimit];
        player = GameObject.Find("Player").GetComponent<BoxCollider2D>();
        player.enabled = false;
        for (int i = 0; i < numberOfEnemyLimit; i++)
        {
            enemyLimits[i] = GameObject.Find("EnemyLimit" + (i + 1).ToString()).GetComponent<BoxCollider2D>();
            Physics2D.IgnoreCollision(player, enemyLimits[i], ignore);
        }

        //�n�C�X�R�A���[�h
         highScore = PlayerPrefs.GetFloat("highScore", 0);
        int highScoreCut = (int)highScore;
        highScoreText.text = String.Format("HighScore: " + highScoreCut);
        Debug.Log(highScore);   
    }

    // Update is called once per frame
    void Update()
    {
        TimeControl();
        ReflectIntensity();
    }
    void FixedUpdate()
    {
        StartCoroutine(ShowResult());
    }

    void TimeControl()
    {
        //�J�E���g�_�E��
        if (startFlag)
        {
            startPanel.gameObject.SetActive(false);
            inPlayUI.gameObject.SetActive(true);
            if (soundSpan == 1) { SoundEffect.SETrigger[0] = true; }
            startTime -= Time.deltaTime;
            soundSpan -= Time.deltaTime;
            //3,2,1�J�E���g�_�E��
            if (startTime > num)
            {
                coundDownText.text = ((num + 1).ToString());
            }
            if (startTime < num) { num--; };
            //�Q�[���J�n
            if (startTime < 0 && startTime > -0.5f)
            {
                coundDownText.text = ("Start!");
                inPlay = true;
            }
            //�e�L�X�g��\��
            else if (startTime < -0.5f)
            {
                coundDownText.text = ("");
                startFlag = false;
            }
            //���ʉ��Đ���b����          
            if (soundSpan < 0)
            {
                SoundEffect.SETrigger[0] = true;
                soundSpan = 1;
            }
        }
        //�Q�[���X�^�[�g
        if (startTime < 0)//�X�R�A���^�C���\��
        {
            player.enabled = true;
            if (inPlay)//
            {
                time += Time.deltaTime;
            }
            scoreText.text = "Score: " + score;
        }
    }
    void ReflectIntensity()�@//���邳�ύX
    {
        if (inPlay)
        {
            brightness -= decreaseRate * Time.deltaTime;   //���邳����
            float MaxBrightness = 1.19f;  //���邳�ő�l
            brightness = System.Math.Min(brightness, MaxBrightness); //�������ق���n��                   
            var pointlight2d = pointLight.gameObject.GetComponent<Light2D>();
            var spritelight2d = spriteLight.gameObject.GetComponent<Light2D>();
            pointlight2d.intensity = brightness;
            spritelight2d.intensity = brightness * 5;

            if (brightness < 0)�@�@//���邳0�ŃQ�[���I�[�o�[
            {
                brightness = 0;
                inPlay = false;
                isOver = true;
            }
            flored = (int)Math.Floor(brightness * 10);
            int maxGage = 10;
            flored = System.Math.Min(flored, maxGage);

            for (int i = 0; i <= flored; i++)�@�@�@//�Q�[�W����
            {
                moonGage[i].gameObject.SetActive(true);
            }

            for (int i = 10; i > flored; i--)
            {
                moonGage[i].gameObject.SetActive(false);
            }

        }
    }

    IEnumerator ShowResult()  //���U���g�\��
    {
        if (!(startFlag) && !(inPlay) && isOver)
        {
            finalBrightnessText.text = String.Format("BrightNess: " + "{0:####}" + "%", finalBrightness * 100);  //�����̂ݕ\��
            finalScoreText.text = String.Format("Score: " + "{0:####}", finalScore);
            resultPanel.gameObject.SetActive(true);
            if (finalBrightness < brightness)
            {
                GainResultBrightNess();
            }
            else if (finalScore < score)
            {
                GainResultScore();
                bonusRate = Mathf.Floor((1 + finalBrightness) * 10) / 10;
                bonusScore = finalScore * bonusRate;

            }
            else 
            {
                finalScoreText.text = String.Format("Score: " + "{0:####}" + "�~" + "{1:#.##}", finalScore, bonusRate);
                yield return new WaitForSeconds(2.0f);                  // �҂�����           
                while (isOver)
                {
                    BonusScore();                                   
                }
                
                finalScoreText.text = String.Format("Score: " + "{0:####}", finalScore);
            }           
        }
    }

    void GainResultBrightNess()
    {
        finalBrightness += brightness / 500;
        if (finalBrightness >= brightness)
        {
            SoundEffect.SETrigger[0] = true;
        }
    }
    void GainResultScore()
    {
       
        finalScore += score / 500;
        if (finalScore > score)
        {
            finalScore = score;
            SoundEffect.SETrigger[0] = true;
        }
    }
�@   void GainNumber(float target, float goal)
    {
        target += goal / 500;
        if (target > goal)
        {
            target = score;
            SoundEffect.SETrigger[0] = true;
        }
    }
    void BonusScore()
    {
        finalScoreText.text = String.Format("Score: " + "{0:####}", finalScore);
        finalScore += (finalScore * bonusRate) / 500;
        if (finalScore > bonusScore)
        {
            finalScore = bonusScore;
            SoundEffect.SETrigger[5] = true;
            isOver = false;
            if(finalScore >= highScore)
            {
                highScore = finalScore;
                PlayerPrefs.SetFloat("highScore", highScore);
            }
        }
    }
}
