using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// CSV based dialogue manager
/// </summary>
public class DialogueManager : MonoBehaviour
{
    /// <summary>
    /// Support languages
    /// </summary>
    public enum Languages
    {
        English,
        Italian
    }
    /// <summary>
    /// How to display inactive answers
    /// </summary>
    public enum InactiveAnswersMode
    {
        Gray,                                                                       // Will have blured color
        Invisible                                                                   // Will not be displayed
    }

    public static DialogueManager Instance;                                         // Singleton

    public Text npcName;                                                            // Non-Player Character name
    public Text npcSay;                                                             // Non-Player Character text
    public GameObject answerPrefab;                                                 // Prefab for answer object
    public GameObject answerFolder;                                                 // Parent folder for answers
    public InactiveAnswersMode answersMode = InactiveAnswersMode.Gray;              // Inactive answers display mode
    public Color inactiveAnswerColor = Color.gray;                                  // Inactive answers color (only for InactiveAnswersMode.Gray)
    public float typingDelay = 0.02f;                                               // Delay between letters typing on screen

    private bool dialogInProgress = false;                                          // Is dialog active now
    private TextAsset dialogueCsv;                                                  // Comma-separated values dialogue descriptor file
    private IEnumerator npcTextRoutine;                                             // NPC text routine
    private bool npcIsTalking = false;                                              // Is NPC talking now
    private IEnumerator playerTextRoutine;                                          // Player text routine
    private bool playerIsTalking = false;                                           // Is player talking now
    private IEnumerator answersDisplayRoutine;                                      // Answers display routine
    private int answersCounter = 0;                                                 // Current answers counter
    private Languages language = Languages.English;                                 // Current language
    private string languageSign = "Eng";                                            // Current dialog language separator
    private string currentPage;                                                     // Current dialog page
    private string npcOnStartName, npcOnStartSay;                                   // NPC name and text before dialog start
    private string[] stuffLineName = { "NpcAnswer", "Requirements", "Effect" };     // Stuff lines will be excluded from answers
    private List<string> clickedAnswers = new List<string>();                       // List of answers were clicked during dialog
    private List<string> blockedAnswers = new List<string>();                       // List of answers manualy blocked during dialog

    void Awake()
    {
        Instance = this;                                                            // Make singleton Instance
    }

    void Start()
    {
        if (    npcName == null
            ||  npcSay == null
            ||  answerPrefab == null
            ||  answerFolder == null)
        {
            Debug.LogError("Wrong default settings");
            return;
        }
    }

    /// <summary>
    /// Start dialog from CSV descriptor
    /// </summary>
    /// <param name="csvFile"> CSV dialog descriptor </param>
    public void StartDialogue(TextAsset csvFile)
    {
        if (csvFile == null)
        {
            Debug.Log("Wrong input data");
            return;
        }
        dialogInProgress = true;
        npcOnStartName = npcName.text;                                              // Save NPC name before dialog start
        npcOnStartSay = npcSay.text;                                                // Save NPC text before dialog start
        dialogueCsv = csvFile;
        DisplayCommonInfo();                                                        // Display info from descriptor common page
        string page;
        page = CsvParser.Instance.GetPage("Welcome", dialogueCsv);                  // Find start page named "Welcome"
        if (page != null)
        {
            DisplayNpcAnswer(GetNpcAnswer(page));                                   // Display NPC answer from start page
            if (DisplayAnswers(page) == true)                                       // Display player answers from start page
            {
                currentPage = page;                                                 // Set current page
            }
            else
            {
                EndDialogue();                                                      // If no valid answers - end dialog
            }
        }
    }

    /// <summary>
    /// Check if dialog in progress
    /// </summary>
    /// <returns> true - in progress, false - not started </returns>
    public bool IsDialogInProgress()
    {
        return dialogInProgress;
    }

    /// <summary>
    /// Choose dialog language
    /// </summary>
    /// <param name="newLanguage"> Language from Languages enum </param>
    public void SetLanguage(Languages newLanguage)
    {
        language = newLanguage;
        switch (newLanguage)
        {
            case Languages.English:
                {
                    languageSign = "Eng";
                    break;
                }
            case Languages.Italian:
                {
                    languageSign = "Ital";
                    break;
                }
            default:
                {
                    Debug.Log("Unknown language");
                    languageSign = "Eng";
                    language = Languages.English;
                    break;
                }
        }
        if (IsDialogInProgress())                                                   // If dialog in progress update current screen
        {
            if (npcTextRoutine != null)                                             // Stop dialog coroutines
            {
                StopCoroutine(npcTextRoutine);
            }
            if (playerTextRoutine != null)
            {
                StopCoroutine(playerTextRoutine);
            }
            if (answersDisplayRoutine != null)
            {
                StopCoroutine(answersDisplayRoutine);
            }
            DisplayCommonInfo();                                                    // Update NPC name
            DisplayNpcAnswer(GetNpcAnswer(currentPage));                            // Update NPC answer
            CleanAnswers();                                                         // Remove current answers from screen
            DisplayAnswers(currentPage);                                            // Update answers from current page
        }
    }

    /// <summary>
    /// Get current language
    /// </summary>
    /// <returns> Current language </returns>
    public Languages GetLanguage()
    {
        return language;
    }

    /// <summary>
    /// End current dialog
    /// </summary>
    public void EndDialogue()
    {
        if (npcTextRoutine != null)                                                 // Stop dialog coroutines
        {
            StopCoroutine(npcTextRoutine);
        }
        if (playerTextRoutine != null)
        {
            StopCoroutine(playerTextRoutine);
        }
        if (answersDisplayRoutine != null)
        {
            StopCoroutine(answersDisplayRoutine);
        }
        CleanAnswers();                                                             // Remove answers from diplay
        npcName.text = npcOnStartName;                                              // Restore NPC name
        npcOnStartName = null;
        npcSay.text = npcOnStartSay;                                                // Restore NPC text
        npcOnStartSay = null;
        currentPage = null;
        clickedAnswers.Clear();                                                     // Clear list of clicked answers
        blockedAnswers.Clear();                                                     // Clear list of manualy blocked answers
        dialogInProgress = false;
    }

    /// <summary>
    /// Actions on user's answer click
    /// </summary>
    /// <param name="answer"> Clicked answer </param>
    public void OnAnswerClick(GameObject answer)
    {
        if (answer == null)
        {
            Debug.Log("Wrong input data");
            return;
        }
        string page = CsvParser.Instance.GetPage(answer.name, dialogueCsv);         // Find page with answer's name
        if (page != null)
        {
            if (clickedAnswers.Contains(answer.name) == false)
            {
                clickedAnswers.Add(answer.name);                                    // Add answer to clicked answers list
            }
            string npcAnswer = GetNpcAnswer(page);                                  // Get NPC answer from page
            if (npcAnswer != null)
            {
                DisplayNpcAnswer(npcAnswer);                                        // Display NPC answer
            }
            if (ApplyEffects(page) == true)                                         // Apply answer effects
            {
                return;                                                             // true - need to stop dialog
            }
            if (DisplayAnswers(page) == true)                                       // Try to display player answers from page
            {
                currentPage = page;                                                 // Save current page
            }
            else
            {
                if (DisplayAnswers(currentPage) == false)                           // If no active answers in new page - stay on current page
                {
                    Debug.Log("No active answers");
                    EndDialogue();                                                  // If no active answers on current page - end dialog
                }
            }
        }
    }

    /// <summary>
    /// Display data from dialog descriptor page
    /// </summary>
    private void DisplayCommonInfo()
    {
        string page;
        page = CsvParser.Instance.GetPage("Desc", dialogueCsv);                     // Get page named "Desc"
        if (page != null)
        {
            string name;
            List<string> lines = CsvParser.Instance.GetLines(page);                 // Get all lines from page
            if (lines != null)
            {
                foreach (string line in lines)
                {
                    ///
                    /// Dislay NPC name
                    ///
                    if (CsvParser.Instance.GetLineName(line) == "NpcName")          // Find line named "NpcName"
                    {
                        if (CsvParser.Instance.GetTextValue(out name, languageSign, line) == true)
                        {
                            if (name != null)                                       // Get text from line
                            {
                                npcName.text = name;                                // Display NPC name
                            }
                        }
                        continue;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Letter by letter NPC answer display
    /// </summary>
    /// <param name="text"> NPC anwer text </param>
    /// <returns></returns>
    private IEnumerator NpcAnswerCoroutine(string text)
    {
        npcIsTalking = true;
        if (typingDelay > 0)                                                        // If display delay setted
        {
            foreach (char letter in text)
            {
                npcSay.text += letter;                                              // Add letter to answer
                yield return new WaitForSeconds(typingDelay);                       // Wait for delay
            }
        }
        else
        {
            npcSay.text += text;                                                    // If no delay needed - display whole text
        }
        npcIsTalking = false;
    }

    /// <summary>
    /// Display NPC answer
    /// </summary>
    /// <param name="text"> NPC answer text </param>
    private void DisplayNpcAnswer(string text)
    {
        if (npcSay != null)
        {
            if (npcTextRoutine != null)
            {
                StopCoroutine(npcTextRoutine);                                      // Stop current coroutine if it is
            }
            npcSay.text = "";                                                       // Clear answer text field
            npcTextRoutine = NpcAnswerCoroutine(text);
            StartCoroutine(npcTextRoutine);                                         // Start coroutine
        }
    }

    /// <summary>
    /// Get NPC answer text from page
    /// </summary>
    /// <param name="page"> Page from CSV dialogue descriptor </param>
    /// <returns> NPC answer text </returns>
    private string GetNpcAnswer(string page)
    {
        string res = null;
        if (page == null)
        {
            Debug.Log("Wrong input data");
            return res;
        }
        List<string> lines = CsvParser.Instance.GetLines(page);                     // Get all lines from page
        if (lines != null)
        {
            foreach (string line in lines)
            {
                if (CsvParser.Instance.GetLineName(line) == "NpcAnswer")            // Find line named "NpcAnswer"
                {
                    if (CsvParser.Instance.GetTextValue(out res, languageSign, line) == true)
                    {
                        break;                                                      // Get text from line
                    }
                }
            }
        }
        return res;
    }

    /// <summary>
    /// Letter by letter player answer display
    /// </summary>
    /// <param name="text"> Answer text </param>
    /// <param name="answer"> Answer display text field </param>
    /// <returns></returns>
    private IEnumerator PlayerAnswerCoroutine(string text, Text answer)
    {
        if (answer != null)
        {
            playerIsTalking = true;
            if (typingDelay > 0)                                                    // If display delay setted
            {
                foreach (char letter in text)
                {
                    answer.text += letter;                                          // Add letter to answer
                    yield return new WaitForSeconds(typingDelay);                   // Wait for delay
                }
            }
            else
            {
                answer.text += text;                                                // If no delay needed - display whole text
            }
            playerIsTalking = false;
        }
    }

    /// <summary>
    /// Add answer into answer folder
    /// </summary>
    /// <param name="name"> Answer name </param>
    /// <param name="answer"> Answer text </param>
    /// <param name="isActive"> true - answer is interactive, false - answer is inactive </param>
    private void AddAnswer(string name, string answer, bool isActive)
    {
        if (name == null || answer == null)
        {
            Debug.Log("Wrong input data");
            return;
        }
        GameObject newAnswer = Instantiate(answerPrefab) as GameObject;             // Clone answer prefab
        if (newAnswer != null)
        {
            newAnswer.transform.SetParent(answerFolder.transform);                  // Place it into anwer folder
            newAnswer.name = name;                                                  // Set answer name
            Text text = newAnswer.GetComponent<Text>();
            if (text != null)
            {
                answersCounter++;                                                   // Increase answers counter (cleared by ClearAnswers)
                AnswerHandler answerHandler = newAnswer.GetComponent<AnswerHandler>();
                if (isActive == false)                                              // If answer inactive
                {
                    answerHandler.active = false;                                   // Make answer not clickable
                    text.color = inactiveAnswerColor;                               // Set inactive color
                }
                else
                {
                    answerHandler.active = true;                                    // Make answer clickable
                }
                if (playerTextRoutine != null)
                {
                    StopCoroutine(playerTextRoutine);                               // Stop current coroutine if it is
                }
                text.text = answersCounter.ToString() + ". ";                       // Display answer counter
                playerTextRoutine = PlayerAnswerCoroutine(answer, text);
                StartCoroutine(playerTextRoutine);                                  // Start coroutine
            }
        }
    }

    /// <summary>
    /// Remove current answers from screen and reset answers counter
    /// </summary>
    private void CleanAnswers()
    {
        answersCounter = 0;                                                         // Reset answers counter
        if (playerTextRoutine != null)                                              // Stop current coroutine if it is
        {
            StopCoroutine(playerTextRoutine);
        }
        if (answersDisplayRoutine != null)
        {
            StopCoroutine(answersDisplayRoutine);
        }
        List<GameObject> children = new List<GameObject>();                         // Get list of current answers
        foreach (Transform child in answerFolder.transform)
        {
            children.Add(child.gameObject);
        }
        answerFolder.transform.DetachChildren();
        foreach (GameObject child in children)
        {
            Destroy(child);                                                         // Destroy current answers
        }
    }

    /// <summary>
    /// Make active answers clickable
    /// </summary>
    private void EnableAnswersRaycast()
    {
        foreach (Transform child in answerFolder.transform)                         // Get current answers list from answers folder
        {
            AnswerHandler answerHandler = child.gameObject.GetComponent<AnswerHandler>();
            if (answerHandler != null)
            {
                if (answerHandler.active)                                           // If answer setted as clickable
                {
                    Text text = child.gameObject.GetComponent<Text>();
                    if (text != null)
                    {
                        text.raycastTarget = true;                                  // Enable text raycast
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get all answers line from page
    /// </summary>
    /// <param name="page"> Page from CSV dialog descriptor </param>
    /// <returns> List of player answers lines </returns>
    private List<string> GetAnswersLines(string page)
    {
        List<string> res = new List<string>();
        List <string> lines = CsvParser.Instance.GetLines(page);                    // Get all lines from page
        if (lines != null)
        {
            foreach (string line in lines)
            {
                string lineName = CsvParser.Instance.GetLineName(line);             // Get name of line
                if (lineName != null)
                {
                    bool stuff = false;
                    foreach (string stuffLine in stuffLineName)
                    {
                        if (lineName == stuffLine)                                  // Compare with stuff lines names
                        {
                            stuff = true;
                            break;
                        }
                    }
                    if (stuff == false)
                    {
                        res.Add(line);                                              // If line is not stuff - add it to list
                    }
                }
            }
        }
        return res;
    }

    /// <summary>
    /// Display player answers one by one followed
    /// </summary>
    /// <param name="answers"> List of answers line and their flags of clickable state </param>
    /// <returns></returns>
    private IEnumerator AnswersDisplayCoroutine(Dictionary<string, bool> answers)
    {
        while ((typingDelay > 0) && (npcIsTalking == true))                         // Wait while NPC stop talking
        {
            yield return new WaitForSeconds(typingDelay);
        }
        foreach (KeyValuePair<string, bool> answer in answers)
        {
            string text;
                                                                                    // Get answer text from line
            if (CsvParser.Instance.GetTextValue(out text, languageSign, answer.Key) == true)
            {
                                                                                    // Add answer to answer folder
                AddAnswer(CsvParser.Instance.GetLineName(answer.Key), text, answer.Value);
                while ((typingDelay > 0) && (playerIsTalking == true))              // Wait for previous answer stop display
                {
                    yield return new WaitForSeconds(typingDelay);
                }
            }
        }
        EnableAnswersRaycast();                                                     // Make active answers clickable
    }

    /// <summary>
    /// Display all player answers from page
    /// </summary>
    /// <param name="page"> Page from CSV dialog descriptor </param>
    /// <returns> true - have active answers, false - no active answers </returns>
    private bool DisplayAnswers(string page)
    {
        bool res = false;
        if (page == null)
        {
            Debug.Log("Wrong input data");
            return res;
        }
        List<string> lines = GetAnswersLines(page);                                 // Get list of answers from page
        if ((lines != null) && (lines.Count > 0))
        {
            bool hasActiveAnswers = false;
            Dictionary<string, bool> answersLines = new Dictionary<string, bool>();
            foreach (string line in lines)
            {
                string answerName = CsvParser.Instance.GetLineName(line);           // Get answer name
                                                                                    // Get page named "answerName"
                string answerPage = CsvParser.Instance.GetPage(answerName, dialogueCsv);
                                                                                    // Check for active answer requirements
                bool isActive = CheckAnswerRequirements(answerPage) && !IsAnswerBlocked(answerName);
                if ((answersMode == InactiveAnswersMode.Invisible) && isActive == false)
                {
                    continue;                                                       // If no need to display inactive answers - skip it
                }
                if (isActive)
                {
                    hasActiveAnswers = true;                                        // If at least one active answer - save it
                }
                answersLines.Add(line, isActive);                                   // Add answer to display list
            }
            if ((answersLines.Count > 0) && (hasActiveAnswers == true))             // If have active answers
            {
                res = true;
                if (answersDisplayRoutine != null)
                {
                    StopCoroutine(answersDisplayRoutine);                           // Stop current coroutine if it is
                }
                answersDisplayRoutine = AnswersDisplayCoroutine(answersLines);
                CleanAnswers();                                                     // Remove current answers
                StartCoroutine(answersDisplayRoutine);                              // Start coroutine
            }
        }
        return res;
    }

    /// <summary>
    /// Check if answer was clicked before in current dialog
    /// </summary>
    /// <param name="answerName"> Answer name </param>
    /// <returns> true - was clicked before, false - was not clicked </returns>
    private bool WasAnswerClickedBefore(string answerName)
    {
        return clickedAnswers.Contains(answerName);
    }

    /// <summary>
    /// Manualy block answer and make it inactive untill dialog end
    /// </summary>
    /// <param name="answerName"> Answer name </param>
    /// <param name="condition"> true - block answer, false - remove from blocked </param>
    private void BlockAnswer(string answerName, bool condition)
    {
        if (answerName != null)
        {
            if ((blockedAnswers.Contains(answerName) == false) && (condition == true))
            {
                blockedAnswers.Add(answerName);
            }
            else if ((blockedAnswers.Contains(answerName) == true) && (condition == false))
            {
                blockedAnswers.Remove(answerName);
            }
        }
    }

    /// <summary>
    /// Check if answer in blocked list
    /// </summary>
    /// <param name="answerName"> Answer name </param>
    /// <returns> true - answer blocked, false - answer not blocked manualy </returns>
    private bool IsAnswerBlocked(string answerName)
    {
        return blockedAnswers.Contains(answerName);
    }

    /// <summary>
    /// Check if answer meet requirements described in CSV dialog page
    /// </summary>
    /// <param name="page"> Page from CSV dialog descriptor with the same name as answer </param>
    /// <returns> true - meet requirements (active), false - fail requirements (inactive) </returns>
    private bool CheckAnswerRequirements(string page)
    {
        if (page == null)
        {
            Debug.Log("Wrong input data");
            return false;
        }
        bool res = true;
        List<string> reqLines = CsvParser.Instance.GetLines("Requirements", page);  // Get all lines named "Requirements" from page
        if (reqLines != null)
        {
            if (reqLines.Count > 0)
            {
                res = false;
            }
            ///
            /// Lines requirements will be united with logical OR
            ///
            foreach (string line in reqLines)
            {
                bool localRes = true;
                // Split line by value named "Req"
                List<string> reqs = CsvParser.Instance.SplitLineByValue("Req", line);
                if (reqs != null)
                {
                    ///
                    /// Field requirements (in one line) will be united with logical AND
                    ///
                    foreach (string req in reqs)
                    {
                        string data;
                        // Get requirement data from CSV
                        if (CsvParser.Instance.GetTextValue(out data, "Req", req) == true)
                        {
                            ///
                            /// Example: gold requirement
                            ///
                            if (data == "Gold")
                            {
                                int num;
                                if (CsvParser.Instance.GetNumValue(out num, data, line) == true)
                                {
                                    if (InventoryControl.Instance.GetGold() < num)
                                    {
                                        localRes = false;
                                        break;
                                    }
                                }
                                continue;
                            }
                            ///
                            /// Example: one timed answer
                            ///
                            else if (data == "OneOff")
                            {
                                if (WasAnswerClickedBefore(CsvParser.Instance.GetPageName(page)) == true)
                                {
                                    localRes = false;
                                    break;
                                }
                                continue;
                            }
                            ///
                            /// Example: free item slot in inventory
                            ///
                            else if (data == "FreeItemCell")
                            {
                                if (InventoryControl.Instance.IsItemCellFree() == false)
                                {
                                    localRes = false;
                                    break;
                                }
                                continue;
                            }
                            ///
                            /// Example: player has no money
                            ///
                            else if (data == "NoGold")
                            {
                                if (InventoryControl.Instance.GetGold() > 0)
                                {
                                    localRes = false;
                                    break;
                                }
                                continue;
                            }
                            ///
                            /// Template for new requirement
                            ///
                            else if (data == "MyOwnRequirement")
                            {
                                ///
                                /// Conditions
                                /// 
                                /// if conditions fails
                                /// localRes must be setted false
                                /// and break added
                                ///
                                continue;
                            }
                            else
                            {
                                Debug.Log("Unknown requirement");
                                continue;
                            }
                        }
                    }
                }
                res = res || localRes;                                              // Requirements lines united by OR and fields united by AND
            }
        }
        return res;
    }

    /// <summary>
    /// Aply effects described in clicked answer's page
    /// </summary>
    /// <param name="page"> Page from CSV dialog descriptor </param>
    /// <returns></returns>
    private bool ApplyEffects(string page)
    {
        if (page == null)
        {
            Debug.Log("Wrong input data");
            return false;
        }
        List<string> lines = CsvParser.Instance.GetLines(page);                     // Get all lines from page
        if (lines != null)
        {
            foreach (string line in lines)
            {
                string lineName = CsvParser.Instance.GetLineName(line);             // Get line name
                if (lineName == "Effect")                                           // Find lines named "Effect"
                {
                    string data;
                    // Get effect data
                    if (CsvParser.Instance.GetTextValue(out data, lineName, line) == true)
                    {
                        ///
                        /// Example: end dialog
                        ///
                        if (data == "Exit")
                        {
                            EndDialogue();
                            InventoryControl.Instance.ResetInventory();
                            return true;
                        }
                        ///
                        /// Example: add or remove gold
                        ///
                        else if (data == "Gold")
                        {
                            int num;
                            if (CsvParser.Instance.GetNumValue(out num, data, line) == true)
                            {
                                InventoryControl.Instance.AddGold(num);
                            }
                            continue;
                        }
                        ///
                        /// Example: add food
                        ///
                        else if (data == "Food")
                        {
                            int num;
                            if (CsvParser.Instance.GetNumValue(out num, data, line) == true)
                            {
                                InventoryControl.Instance.AddFood(num);
                            }
                            continue;
                        }
                        ///
                        /// Example: add item to inventory
                        ///
                        else if (data == "Item")
                        {
                            if (CsvParser.Instance.GetTextValue(out data, data, line) == true)
                            {
                                if (data == "Add")
                                {
                                    if (CsvParser.Instance.GetTextValue(out data, data, line) == true)
                                    {
                                        InventoryControl.Instance.AddItem(data);
                                    }
                                }
                            }
                            continue;
                        }
                        ///
                        /// Example: manualy block answer and make it inactive
                        ///
                        else if (data == "BlockIt")
                        {
                            BlockAnswer(CsvParser.Instance.GetPageName(page), true);
                            continue;
                        }
                        ///
                        /// Example: play sound
                        ///
                        else if (data == "Sound")
                        {
                            if (CsvParser.Instance.GetTextValue(out data, data, line) == true)
                            {
                                SoundLoader.Instance.PlaySound(data);
                            }
                            continue;
                        }
                        ///
                        /// Template for new effect
                        ///
                        else if (data == "MyOwnEffect")
                        {
                            ///
                            /// Effect handler
                            ///
                            continue;
                        }
                        else
                        {
                            Debug.Log("Unknown effect");
                            continue;
                        }
                    }
                }
            }
        }
        return false;
    }
}
