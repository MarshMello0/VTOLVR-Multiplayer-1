﻿using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class LockingRadarNetworker_Receiver : MonoBehaviour
{
    public ulong networkUID;
    private Message_RadarUpdate lastRadarMessage;
    private Message_LockingRadarUpdate lastLockingMessage;
    private LockingRadar lockingRadar;
    private RadarLockData radarLockData;
    private void Awake()
    {
        lockingRadar = gameObject.GetComponentInChildren<LockingRadar>();
        if (lockingRadar == null)
        {
            Debug.Log($"Locking radar on networkUID {networkUID} is null.");
            return;
        }
        lastRadarMessage = new Message_RadarUpdate(false, 0, networkUID);
        Networker.RadarUpdate += RadarUpdate;
        Networker.LockingRadarUpdate += LockingRadarUpdate;
    }

    public void RadarUpdate(Packet packet)
    {
        lastRadarMessage = (Message_RadarUpdate)((PacketSingle)packet).message;
        Debug.Log("Got a new radar update intended for id " + lastRadarMessage.UID);
        if (lastRadarMessage.UID != networkUID)
            return;

        Debug.Log($"Doing radarupdate for uid {networkUID}");
        lockingRadar.radar.radarEnabled = lastRadarMessage.on;
        lockingRadar.radar.sweepFov = lastRadarMessage.fov;
    }
    public void LockingRadarUpdate(Packet packet)
    {
        lastLockingMessage = (Message_LockingRadarUpdate)((PacketSingle)packet).message;
        Debug.Log("Got a new locking radar update intended for id " + lastLockingMessage.senderUID);
        if (lastLockingMessage.senderUID != networkUID)
            return;

        Debug.Log($"Doing LockingRadarupdate for uid {networkUID} which is intended for uID {lastLockingMessage.senderUID}");
        if (!lastLockingMessage.isLocked && lockingRadar.IsLocked())
        {
            Debug.Log($"Unlocking radar {gameObject.name}");
            lockingRadar.Unlock();
            return;
        }
        else if (lastLockingMessage.isLocked && !lockingRadar.IsLocked())
        {
            Debug.Log("Trying to lock radar.");
            foreach (var AI in AIManager.AIVehicles)
            {
                if (AI.vehicleUID == lastLockingMessage.actorUID)
                {
                    lockingRadar.ForceLock(AI.actor, out radarLockData);
                    Debug.Log($"Radar {gameObject.name} found its lock {AI.vehicleName} with an id of {AI.vehicleUID} while trying to lock id {lastLockingMessage.actorUID}.");
                    Debug.Log($"The lock data is Locked: {radarLockData.locked}, Locked Actor: {radarLockData.actor.name}.");
                    break;
                }
            }
        }
    }
    public void OnDestroy()
    {
        Networker.RadarUpdate -= RadarUpdate;
        Debug.Log("Radar update");
        Debug.Log(gameObject.name);
    }
}
