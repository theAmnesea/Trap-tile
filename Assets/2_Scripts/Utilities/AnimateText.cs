using System;
using System.Text;
using System.Collections;
using UnityEngine;
using TMPro;

[Serializable]
public class AnimateText : MonoBehaviour
{
    public bool loop = true;
    private bool On = true;
    private bool reset = true;
    private bool skip = false;

    private String FinalText;

    public float StartDelay = 0f;
    public float TotalTypeTime = -1f;
    public float TypeRate;
    public float RandomCharacterChangeRate = 0.1f;
    private float LastTime;
    private float RandomCharacterTime;

    private string RandomCharacter;

    private int i;

    private void Awake()
    {
        FinalText = gameObject.GetComponent<TMP_Text>().text;
    }

    private void OnEnable()
    {
        if (StartDelay >= 2)
        {
            StartDelay = 1;
        }

        StartCoroutine(UpdateTextAfterDelay());
    }

    public void RestartText(string newText)
    {
        FinalText = newText;
        StartDelay = 0.1f;

        On = true;
        reset = true;
        skip = false;

        StartCoroutine(UpdateTextAfterDelay());
    }

    private string RandomChar()
    {
        byte value = (byte)UnityEngine.Random.Range(41f, 128f);

        string c = Encoding.ASCII.GetString(new byte[] { value });

        return c;
    }

    public void Skip()
    {
        GetComponent<TMP_Text>().text = FinalText;
        StopCoroutine(UpdateText());
        skip = true;
    }

    private void ResetText()
    {
        if (reset == true)
        {
            GetComponent<TMP_Text>().text = "";
            i = 0;
            reset = false;
            On = true;
        }
    }

    private IEnumerator UpdateTextAfterDelay()
    {
        ResetText();
        yield return new WaitForSeconds(StartDelay);

        // Remove Delay on next function call
        StartDelay = 0;
        StartCoroutine(UpdateText());
    }

    private IEnumerator UpdateText()
    {
        if (skip == false)
        {
            if (TotalTypeTime != -1f)
            {
                TypeRate = TotalTypeTime / FinalText.Length;
            }

            if (On == true)
            {
                if (Time.time - RandomCharacterTime >= RandomCharacterChangeRate)
                {
                    RandomCharacter = RandomChar();
                    RandomCharacterTime = Time.time;
                }

                try
                {
                    GetComponent<TMP_Text>().text = FinalText.Substring(0, i) + RandomCharacter;
                }
                catch (ArgumentOutOfRangeException)
                {
                    On = false;
                }

                if (Time.time - LastTime >= TypeRate)
                {
                    i++;
                    LastTime = Time.time;
                }

                bool isChar = false;

                while (isChar == false)
                {
                    if ((i + 1) < FinalText.Length)
                    {
                        if (FinalText.Substring(i, 1) == " ")
                        {
                            i++;
                        }
                        else
                        {
                            isChar = true;
                        }
                    }
                    else
                    {
                        isChar = true;
                    }
                }

                if (GetComponent<TMP_Text>().text.Length == FinalText.Length + 1)
                {
                    RandomCharacter = RandomChar();
                    GetComponent<TMP_Text>().text = FinalText;
                    On = false;
                }
            }

            ResetText();

            if (On == false && loop == true)
            {
                yield return new WaitForSeconds(TotalTypeTime * 2);
                reset = true;
            }

            yield return null;
            StartCoroutine(UpdateText());
        }
    }
}
