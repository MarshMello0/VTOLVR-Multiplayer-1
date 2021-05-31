﻿using System;
using System.Collections.Generic;

[Serializable]
public class Message_JoinRequest : Message
{
    public string name,currentVehicle, vtolVrVersion, multiplayerBranch, multiplayerModVersion, scenarioId;
    public byte[] mapHash, scenarioHash, campaignHash;
    public bool builtInCampaign;
    public Dictionary<string, string> modsLoadedHashes;
    public byte[] modloaderHash;
    public long discordID;

    public Message_JoinRequest(String namepl, string currentVehicle, bool builtInCampaign, string scenarioId, byte[] mapHash, byte[] scenarioHash, byte[] campaignHash, Dictionary<string, string> mods, byte[] modloaderhash, long dID)
    {
        this.name = namepl;
        this.currentVehicle = currentVehicle;
        this.builtInCampaign = builtInCampaign;
        this.scenarioId = scenarioId;
        this.mapHash = mapHash;
        this.scenarioHash = scenarioHash;
        this.campaignHash = campaignHash;
        vtolVrVersion = GameStartup.versionString;
        modsLoadedHashes = mods;
        modloaderHash = modloaderhash;
        multiplayerBranch = ModVersionString.ReleaseBranch;
        multiplayerModVersion = ModVersionString.ModVersionNumber;
        discordID = dID;
        type = MessageType.JoinRequest;
    }
}
[Serializable]
public class Message_JoinRequestAccepted_Result : Message
{
    public long hostDiscordID;
    public long lobbyDiscordID;
    public string lobbySecret;
    public float thrust;
    public bool hiAlpha;
    public float fog;
    public string freqString;
    public string freqLabelString;
    public Message_JoinRequestAccepted_Result(long ihostDiscordID, long ilobbyDiscordID, string ilobbySecret, float fthrust, bool alpha, float Fog, String freq, string labels)
    {
        hostDiscordID = ihostDiscordID;
        lobbyDiscordID = ilobbyDiscordID;
        lobbySecret = ilobbySecret;
        thrust = fthrust;
        hiAlpha = alpha;
        fog = Fog;
        freqString = freq;
        freqLabelString = labels;
        type = MessageType.JoinRequestAccepted_Result;
    }
}
[Serializable]
public class Message_JoinRequestRejected_Result : Message
{
    public string reason;

    public Message_JoinRequestRejected_Result(string reason)
    {
        this.reason = reason;
        type = MessageType.JoinRequestRejected_Result;
    }
}
