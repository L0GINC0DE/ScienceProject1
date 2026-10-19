using System;
using TMPro;
using UnityEngine;

public class QuizText : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI _textUI;
    [SerializeField]private TextMeshProUGUI _questionText;
    

   public void SetText(string text)
   {
       _textUI.text = text;
       _questionText.text = text;
   }
}
