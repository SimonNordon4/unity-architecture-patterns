using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialMessage
    {
        Wasd,
        Dash,
        OnDamage,
        Chest,
        Enemy,
        Buy,
        Pause
    }
    
    private static TutorialManager _instance;

    public static TutorialManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<TutorialManager>();
            return _instance;
        }
        private set => _instance = value;
    }

    public GameObject popup;
    public TextMeshProUGUI popupText;
    
    private Dictionary<TutorialMessage, string> Tips = new()
    {
        { TutorialMessage.Wasd, "Use WASD to Move."},
        { TutorialMessage.Dash , "Press Space to Dash."},
        {TutorialMessage.OnDamage,"When your health reaches 0, you lose"},
        {TutorialMessage.Chest, "Pickup Chests to improve your strength"},
        {TutorialMessage.Buy,"Buy items in the store to get permanently stronger."},
        {TutorialMessage.Pause,"Press F to pause the game."}
    };
    
    public void ShowTip(TutorialMessage message, float delay = 0f)
    {
        // Show tips for the first game only.
        if(AccountManager.instance.statistics.gamesPlayed > 1) return;
        StartCoroutine(ShowPopup(Tips[message], delay));
    }

    private IEnumerator ShowPopup(string message, float delay)
    {
        yield return new WaitForSeconds(delay);
        popup.SetActive(true);
        popupText.text = message;
        yield return new WaitForSeconds(3f);
        popup.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
