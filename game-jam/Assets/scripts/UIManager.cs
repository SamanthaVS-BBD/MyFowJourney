using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI scoreTxt;

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
        scoreTxt.text = _player.GetComponent<characterScript>().getScore().ToString();
    }

    public void setGameScore(bool state){
        scoreTxt.transform.gameObject.SetActive(state);
    }
}
