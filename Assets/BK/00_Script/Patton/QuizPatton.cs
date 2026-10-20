using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Random = UnityEngine.Random;

public class QuizPatton : AbstractPatton
{
  [SerializeField] private GameObject _beforBoss;
  
  [SerializeField]private TextMeshProUGUI _textUI;
  
  [SerializeField] private GameObject _oBtton;
  [SerializeField]private GameObject _xBtton;
  
  private int questionCount = 0;
  
  public string[] questions;
  private void Start()
  {
    _oBtton.SetActive(false);
    
    _xBtton.SetActive(false);
    SetQuestion();
    Patton();
  }

  private void OnEnable()
  {
    TurnManager.Instance.OnEnemyTurn += Patton;
  }

  public override void Patton() //문제 위로 올라오고 버튼으로 OX결정하는거
  {
    
    base.Patton();
    
    _oBtton.SetActive(true);
   
    _xBtton.SetActive(true);
    _textUI.gameObject.SetActive(true);
    _beforBoss.SetActive(false);
    Sequence seq = DOTween.Sequence();
    seq.Append(gameObject.transform.DOMoveY(1.2f,0.5f));
    
  }

  public void GoodAnswer() //대충 정답 나오고 턴 넘어감 보스에 대미지 줘야함
  {
    //questionCount = Random.Range(0, questions.Length);
    
    _textUI.text = "정답!!";
    StartCoroutine(Co());
    Sequence seq = DOTween.Sequence();
   
    _oBtton.SetActive(false);
    
    _xBtton.SetActive(false);
    seq.Append(gameObject.transform.DOMoveY(-12f,0.5f));
    seq.AppendCallback(() => _textUI.gameObject.SetActive(false));
    seq.AppendCallback(()=>_beforBoss.SetActive(true));
  }
  public void BadAnswer() //대충 오답 나오고 턴 넘어감 플레이어 대미지 줘야함
  {
    //questionCount = Random.Range(0, questions.Length);
    
    _textUI.text = "오답!!!!!!!";
    StartCoroutine(Co());
    Sequence seq = DOTween.Sequence();
    
    _oBtton.SetActive(false);
  
    _xBtton.SetActive(false);
    seq.Append(gameObject.transform.DOMoveY(-12f,0.5f));
    seq.AppendCallback(() => _textUI.gameObject.SetActive(false));
    seq.AppendCallback(()=>_beforBoss.SetActive(true));
  }

  public IEnumerator Co() //그냥 기달렸다가 문제 바꾸는 거임.
  {
    yield return new WaitForSeconds(0.5f);
    SetQuestion();
  }

  public void SetQuestion() //그냥 문제 바꾸는 거임
  {
    questionCount = Random.Range(0, questions.Length);
    _textUI.text = questions[questionCount];
    
  }

}
