﻿using System;
[Serializable]
public class Message_MissileUpdate : Message
{
    public ulong networkUID;
    public Vector3D targetPosition;
    public SerializableQuaternion seekerRotation;
    public bool hasExploded;
    public Missile.GuidanceModes guidanceMode;
    public ulong MissileLauncher;
    public int idx;
    public ulong radarLock;
    public Message_MissileUpdate(ulong uid)
    {
        networkUID = uid;
        type = MessageType.MissileUpdate;
    }
}

[Serializable]
public class Message_MissileDamage : Message
{
    public ulong networkUID;
    public Vector3D targetPosition;
    public ulong actorTobeDamaged;
    public float damage;
    public ulong damageSourceActor;

    public Message_MissileDamage(ulong uid)
    {
        networkUID = uid;
        type = MessageType.MissileDamage;
    }
}
