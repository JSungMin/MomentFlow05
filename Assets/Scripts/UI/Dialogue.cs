using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 케릭터들 enum
// 및에 있는 string 배열과 sync가 맞아야 한다
// 더욱 core 쪽에 넣는 것도 좋을 거 같음
public enum Character
{
    Player = 0,
    Actor = 1,
    Enemy = 2
}

public enum Expression
{
    Normal = 0,
    Happy = 1,
    Angry = 2,
    Sad = 3
}

public class Dialogue : MonoBehaviour
{
    // enum이 코어 쪽이라면 여기는 코어를 따라가는 쪽이다
    // Resources들을 로드할 때 쓰일 이름
    public static string[] charactersName = { "Player", "Actor", "Enemy" };
    public static string[] expressions = { "Normal", "Happy", "Angry", "Sad" };

    // 이것도 core로 넘겨도 좋을 듯
    int characterNum = 3;
    // 이거는 여기에만 있는게 나을 듯
    int expressionNum = 3;

    public string[] contents;

    private void Awake()
    {
        for (int i = 0; i < characterNum; i++)
        {
            for (int j = 0; j < expressionNum; j++)
            {
                Resources.Load("Dialogue/" + charactersName[i] + "_" + expressions[j]);
            }
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //    ShowDialogue(Character.Actor, Expression.Normal);
    }

    public void ShowDialogue(Character character, Expression expre, string strs)
    {

    }
}
