﻿using System;
[Serializable]
public class Message_PlaneUpdate : Message
{
    public bool landingGear;
    public float flaps; //0 = 0, 0.5 = 1, 1 = 1
    public float pitch, roll, yaw;
    public float brakes, throttle;
    public float doorState; // 0 = closed, 1 = open, in between is the reallm of quantum or something
    public bool tailHook, fuelPort, launchBar;
    public ulong networkUID;
    public ulong sequenceNumber;
    public Message_PlaneUpdate(bool landingGear, float flaps, float pitch, float roll, float yaw, float breaks, float throttle, bool tailHook, bool launchBar, bool fuelPort, ulong networkUID, ulong sequenceNumber)
    {
        this.landingGear = landingGear;
        this.flaps = flaps;
        this.pitch = pitch;
        this.roll = roll;
        this.yaw = yaw;
        brakes = breaks;
        this.throttle = throttle;
        this.tailHook = tailHook;
        this.fuelPort = fuelPort;
        this.networkUID = networkUID;
        this.launchBar = launchBar;
        this.sequenceNumber = sequenceNumber;
        type = MessageType.PlaneUpdate;
    }

    public override string ToString()
    {
        return $"Landing Gear = {landingGear} Flaps = {flaps} Pitch = {pitch} Roll = {roll} Yaw = {yaw} Breaks = {brakes} " +
            $"Throttle = {throttle} tailHook = {tailHook} Fuel Port = {fuelPort} NetworkID = {networkUID}";
    }
}

[Serializable]
public class Message_IKPuppet : Message
{
    public Vector3D puppetRhand;
    public Vector3D puppetLhand;
    public Vector3D puppetHead;
    public Vector3D puppetHeadLook;
    public ulong networkUID;
    public Message_IKPuppet(ulong networkUIDD)
    {

        networkUID = networkUIDD;

        type = MessageType.IKPuppet;
    }
}