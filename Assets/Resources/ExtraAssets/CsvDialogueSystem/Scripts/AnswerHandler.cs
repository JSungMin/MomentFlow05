using UnityEngine;
using System.Collections;

/// <summary>
/// Answer click handler for CSV dialog system
/// </summary>
public class AnswerHandler : MonoBehaviour
{
    public bool active = false;                                                     // Answer interactive flag: true - clickable, false - inactive

    /// <summary>
    /// Call when mouse click on answer text
    /// </summary>
    public void Clicked()
    {
        DialogueManager.Instance.OnAnswerClick(gameObject);                         // Call CSV dialog manager handler with current answer link
    }
}
