using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Vector3 defaultPos;
    Vector3 cameraPos;
    Camera camera;
    Trigger cameraTrigger;
    GameSystem gamesystem;
    private GameObject cameraTriggerGO;
    int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        count = 0;  
        cameraPos = this.transform.position;
        defaultPos = new Vector3(0, 0, -10);
        camera = this.GetComponent<Camera>();
        cameraTriggerGO = GameObject.Find("CameraTrigger");
        gamesystem = GameObject.Find("Scripts").GetComponent<GameSystem>();
        cameraTrigger = cameraTriggerGO.GetComponent<Trigger>();
        
    }

    // Update is called once per frame
    void Update()
    {
        CameraPosition(); 
    }

    void CameraPosition()
    {
        if (gamesystem.startFlag && count == 0)　//スタート時にプレイヤーの位置をアップで強調
        {
            cameraPos = GameObject.Find("Player").transform.position;　
            camera.orthographicSize = 3;
            count++;
        }
        if (!(gamesystem.inPlay))　　　//プレイ時はカメラを引いて全体を写す
        {　
            camera.orthographicSize = FillDifference(camera.orthographicSize, 6, 3f);
            cameraPos.x = FillDifference(cameraPos.x, defaultPos.x, 4f);
            cameraPos.y = FillDifference(cameraPos.y, defaultPos.y, 3f);
        }     

        if (cameraTrigger.trigger == true)    //プレイヤーが画面右端オブジェクトに触れたらカメラが右に移動
        {
            cameraPos.x += 20;         
            cameraTrigger.trigger = false;
        }
        cameraPos.z = -10;
        this.transform.position = cameraPos;
    }

    float FillDifference(float num1, float num2, float fillRate)　//二つの数値を近づける関数
    {
        float sign;
        if(Mathf.Abs(num2 - num1) > 0.01)
        {
            sign = Mathf.Sign(num2 - num1);
            num1 += fillRate * sign * Time.deltaTime;

        }
        return num1;
    }
}
