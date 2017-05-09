using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElasticityGaugeManager : MonoBehaviour
{
    private const int MAX_GAUGE = 100;
    private int nowGauge = 0;

    private Player player;

    private void Awake()
    {
        player = GameObject.FindObjectOfType<Player>();
    }

    public void SubGauge(int gauge)
    {
        nowGauge -= gauge;
        if (nowGauge <= 0)
            nowGauge = 0;
    }

    public void AddGauge(int gauge)
    {
        nowGauge += gauge;
        if (nowGauge >= MAX_GAUGE)
        {
            nowGauge = MAX_GAUGE;
            ThrownOut();
        }
    }

    private void ThrownOut()
    {
        int toLayer = player.OppositeLayer(player.pTimeLayer.layerNum);
        if (player.CanSwitchingTime(toLayer))
            player.DoTimeSwitch();
        else
            player.Dead(true);

        // 튕겨나가는 effect
        // player 행동 undo
    }
}