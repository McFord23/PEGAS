﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    Player player;
    DevOps devOps;
    GUIStyle style;

    void Start()
    {
        player = GetComponentInParent<Player>();
        devOps = GetComponent<DevOps>();
        style = new GUIStyle();
    }

    void Update()
    {
        style.normal.textColor = (devOps.pauseMenu.activeSelf || devOps.deadMenu.activeSelf || devOps.victoryMenu.activeSelf) ? Color.white : style.normal.textColor = Color.black;
    }

    void OnGUI()
    {
        GUILayout.Label("Developer Mode: " + devOps.mode, style);
        GUILayout.Label("Godness mode: " + player.godnessMode, style);
        GUILayout.Label("Speed: " + (int)player.speed, style);
        GUILayout.Label("Acceleration: " + (int)player.acceleration, style);
    }
}
