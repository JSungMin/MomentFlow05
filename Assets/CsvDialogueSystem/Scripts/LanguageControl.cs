using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Language switcher for CSV dialog system
/// </summary>
public class LanguageControl : MonoBehaviour
{
    public Image flag;                                                              // Current language flag

    private string spriteFolder = "Sprites/";                                       // Folder for sprite load

    void Start ()
    {
	    if (flag == null)
        {
            Debug.LogError("Wrong default settings");
            return;
        }
        DialogueManager.Instance.SetLanguage(DialogueManager.Languages.English);    // Set english on laod
        flag.sprite = Resources.Load<Sprite>(spriteFolder + "BritishFlag");         // Load british flag
    }

    /// <summary>
    /// Toggle between languages (called on button press)
    /// </summary>
    public void ToggleLanguage()
    {
        DialogueManager.Languages language = DialogueManager.Instance.GetLanguage();
        if (language == DialogueManager.Languages.English)                          // Switch between english and italian
        {
            DialogueManager.Instance.SetLanguage(DialogueManager.Languages.Italian);
            flag.sprite = Resources.Load<Sprite>(spriteFolder + "ItalianFlag");
        }
        else
        {
            DialogueManager.Instance.SetLanguage(DialogueManager.Languages.English);
            flag.sprite = Resources.Load<Sprite>(spriteFolder + "BritishFlag");
        }
    }
}
