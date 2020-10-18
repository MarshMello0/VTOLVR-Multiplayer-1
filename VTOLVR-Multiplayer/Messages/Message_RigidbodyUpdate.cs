﻿using System;
using UnityEngine;
[Serializable]
public class Message_RigidbodyUpdate : Message
{
    public SerializableVector3 velocity, angularVelocity;
    public Vector3D position;
    public SerializableQuaternion rotation;
    public ulong sequenceNumber;
    public ulong networkUID;

    public Message_RigidbodyUpdate(Vector3D velocity, Vector3D angularVelocity, Vector3D position, Quaternion rotation, ulong sequenceNumber, ulong networkUID)
    {
        this.velocity = velocity;
        this.angularVelocity = angularVelocity;
        this.position = position;
        this.rotation = rotation;
        this.sequenceNumber = sequenceNumber;
        this.networkUID = networkUID;
        type = MessageType.RigidbodyUpdate;
    }
}