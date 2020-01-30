﻿using System;

[Serializable]
public class Message_WeaponSet_Result : Message
{
    public string[] hpLoadout;
    public int[] cmLoadout;
    public float normalizedFuel;
    public ulong UID;

    public Message_WeaponSet_Result(string[] hpLoadout, int[] cmLoadout, float normalizedFuel, ulong uID)
    {
        this.hpLoadout = hpLoadout;
        this.cmLoadout = cmLoadout;
        this.normalizedFuel = normalizedFuel;
        UID = uID;
        type = MessageType.WeaponsSet_Result;
    }
}