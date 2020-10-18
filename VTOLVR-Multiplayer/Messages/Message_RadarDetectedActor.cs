﻿using System;
[Serializable]
public class Message_RadarDetectedActor : Message
{
    public ulong detectedUID;
    public ulong senderUID;
    public Message_RadarDetectedActor(ulong detectedUID, ulong senderUID)
    {
        this.detectedUID = detectedUID;
        this.senderUID = senderUID;
        type = MessageType.RadarDetectedActor;
    }
}