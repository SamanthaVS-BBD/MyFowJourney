using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Main Game UI")]
    [SerializeField] private TextMeshProUGUI scoreTxt;
   

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverScreen;

    [SerializeField] private TextMeshProUGUI gameOverScoreTxt;

    private GameObject _player;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        setScoreText();
    }

    public void setScoreText(){
        scoreTxt.text = _player.GetComponent<movement>().getScore().ToString();
    }

    public void setGameScore(bool state){
        scoreTxt.transform.gameObject.SetActive(state);
    }

    public void setGameOverScreen(bool state){
        gameOverScreen.SetActive(state);
    }

    public void setGameOverScore(string score){
        gameOverScoreTxt.text = score;
    }

    public void gameOverUI(string score){
        setGameOverScreen(true);
        setGameOverScore(score);
    }
}
