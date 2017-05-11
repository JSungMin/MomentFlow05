using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElasticityGaugeManager : MonoBehaviour
{
    private const int MAX_GAUGE = 100;
    private int nowGauge = 0;

    private Player player;
    private FollowUpCamera followUpCamera;

    private void Awake()
    {
        player = GameObject.FindObjectOfType<Player>();
        followUpCamera = GameObject.FindObjectOfType<FollowUpCamera>();
    }

    public void SubGauge(int gauge)
    {
        nowGauge -= gauge;
        if (nowGauge <= 0)
            nowGauge = 0;
    }

    public void AddGauge(int gauge, UndoDelegate undoDelegate)
    {
        nowGauge += gauge;
        if (nowGauge >= MAX_GAUGE)
        {
            nowGauge = MAX_GAUGE;
            StartCoroutine(ThrownOut(undoDelegate));
        }
    }

    private IEnumerator ThrownOut(UndoDelegate undoDelegate)
    {
        int toLayer = player.OppositeLayer(player.pTimeLayer.layerNum);
        // 다른 레이어로 갈 수 있다면 시간을 바꾸고
        // 갈 수 없다면 죽인다
        if (player.CanSwitchingTime(toLayer))
        {
            player.DoTimeSwitch();
            yield return StartCoroutine(followUpCamera.DoElasticEffect());
            undoDelegate();
        }
        else
            player.Dead(true);
    }
}