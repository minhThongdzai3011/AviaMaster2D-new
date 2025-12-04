using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextIntro : MonoBehaviour
{
    public static TextIntro instance;
    // Start is called before the first frame update
    public TextMeshProUGUI TextWin;
    void Start()
    {
        instance = this;
        GetRandomWelcomeMessage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    string[] shortMessagesHome = {
        "Ready to fly high!",
        "Clear skies ahead!",
        "Take off now!",
        "Sky high adventure!",
        "Pilot mode engaged!",
        "Wings up strong!",
        "Adventure awaits you!"
    };

    string[] shortMessagesTextWinBad = {
        "Crash!",
        "Boom!",
        "Failed!"
    };

    string [] shortMessagesTextWinGood = {
        "Safe!",
        "Good!",
        "Nice!"
 
    };

    string [] shortMessagesTextWinPerfect = {
        "Perfect!",
        "Excellent!",
        "Flawless!"
    };



    public void GetRandomWelcomeMessage()
    {
        string randomMessage = shortMessagesHome[UnityEngine.Random.Range(0, shortMessagesHome.Length)];
        GManager.instance.newMapText.text = randomMessage;
    }

    public void GetRandomTextWinMessage(int winType)
    {
        string randomMessage = "";
        if (winType == 0) // bad
        {
            randomMessage = shortMessagesTextWinBad[UnityEngine.Random.Range(0, shortMessagesTextWinBad.Length)];
        }
        else if (winType == 1) // good
        {
            randomMessage = shortMessagesTextWinGood[UnityEngine.Random.Range(0, shortMessagesTextWinGood.Length)];
        }
        else if (winType == 2) // perfect
        {
            randomMessage = shortMessagesTextWinPerfect[UnityEngine.Random.Range(0, shortMessagesTextWinPerfect.Length)];
        }

        TextWin.text = randomMessage;
    }
}
