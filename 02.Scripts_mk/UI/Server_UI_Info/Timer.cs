using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : NetworkBehaviour
{
    [Networked] TickTimer timer { get; set; }

    public override void Spawned()
    {
        timer = TickTimer.CreateFromSeconds(Runner,5f);
        Debug.Log(timer.RemainingTime(Runner));
    }
    void FixedUpdateNetwork()
    {
        if (timer.Expired(Runner))
        {
            // Execute Logic

            // Reset timer
            timer = TickTimer.None;
        }
    }
}
