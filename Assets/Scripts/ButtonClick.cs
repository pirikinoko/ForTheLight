using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonClick : MonoBehaviour
{
    GameSystem gamesystem;
    // Start is called before the first frame update
    void Start()
    {
        gamesystem = GameObject.Find("Scripts").GetComponent<GameSystem>();
    }
    public void StartGame() //�X�^�[�g�{�^��
    {
        gamesystem.startFlag = true;
    }
    public void ResetGame()
    {
        SceneManager.LoadScene("Main");�@//���X�^�[�g�{�^��
    }
}
