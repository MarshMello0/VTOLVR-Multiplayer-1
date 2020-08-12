﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;


[HarmonyPatch(typeof(VTEventTarget), "Invoke")]
class Patch2
{
    static void Postfix(VTEventTarget __instance)
    {
        String actionIdentifier = __instance.eventName + __instance.methodName + __instance.targetID + __instance.targetType.ToString();
        int hash = actionIdentifier.GetHashCode();

        Message_ScenarioAction ScanarioActionOutMessage = new Message_ScenarioAction(PlayerManager.localUID, hash);
        if (Networker.isHost)
        {
            Debug.Log("Host sent Event action" + __instance.eventName + " of type " + __instance.methodName + " for target " + __instance.targetID);

            NetworkSenderThread.Instance.SendPacketAsHostToAllClients(ScanarioActionOutMessage, Steamworks.EP2PSend.k_EP2PSendUnreliableNoDelay);
        }
        else
        {

            Debug.Log("Client sent  Event action" + __instance.eventName + " of type " + __instance.methodName + " for target " + __instance.targetID);
            NetworkSenderThread.Instance.SendPacketToSpecificPlayer(Networker.hostID, ScanarioActionOutMessage, Steamworks.EP2PSend.k_EP2PSendUnreliableNoDelay);
        }
    }
}

// patch to grab all the events being loaded on creation this replaces original method
[HarmonyPatch(typeof(VTEventInfo), "LoadFromInfoNode")]
class Patch3
{
    static bool Prefix(VTEventInfo __instance, ConfigNode infoNode)
    {

        Debug.Log("bahacode scenario dictionary");
        __instance.eventName = infoNode.GetValue("eventName");
        __instance.actions = new List<VTEventTarget>();
        foreach (ConfigNode node in infoNode.GetNodes("EventTarget"))
        {
            VTEventTarget vTEventTarget = new VTEventTarget();
            vTEventTarget.LoadFromNode(node);
            __instance.actions.Add(vTEventTarget);
            Debug.Log("Compiling scenario dictonary my codd2");
            String actionIdentifier = vTEventTarget.eventName + vTEventTarget.methodName + vTEventTarget.targetID + vTEventTarget.targetType.ToString();
            Debug.Log(actionIdentifier);
            int hash = actionIdentifier.GetHashCode();
            Debug.Log("Compiling scenario dictonary  adding to my dictionary");

            if (!ObjectiveNetworker_Reciever.scenarioActionsList.ContainsKey(hash))
                ObjectiveNetworker_Reciever.scenarioActionsList.Add(hash, vTEventTarget);
            else
                Debug.Log("Duplicate VT scenario actions found, we should probably rewrite the dictionary code");

        }
        return false;//dont run bahas code
    }
}






//patch to grab all the events being loaded on creation this replaces original method
[HarmonyPatch(typeof(MissionObjective), "CompleteObjective")]
class Patch4
{
    static bool Prefix(MissionObjective __instance)
    {
        //prevents infinite client host pings
        //if (__instance.completed)
        //   return false;
        Debug.Log("A mission got completed we need to send it");


        // Debug.Log("sending __instance.objectiveName + __instance.objectiveID");
        String actionIdentifier = __instance.objectiveName + __instance.objectiveID;

        Debug.Log(actionIdentifier);

        //dont run corrupt objectives
        if (MissionManager.instance.IndexOfObjective(__instance) == -1)
            return false;
        Message_ObjectiveSync objOutMessage = new Message_ObjectiveSync(PlayerManager.localUID, MissionManager.instance.IndexOfObjective(__instance), ObjSyncType.EMissionCompleted);
        if (Networker.isHost && objOutMessage.objID != -1)
        {
            Debug.Log("Host sent objective complete " + __instance.objectiveID);
            ObjectiveNetworker_Reciever.completeNext = false;
            NetworkSenderThread.Instance.SendPacketAsHostToAllClients(objOutMessage, Steamworks.EP2PSend.k_EP2PSendUnreliableNoDelay);
        }
        else
        {
            if (VTScenario.current.objectives.GetObjective(__instance.objectiveID).objectiveType == VTObjective.ObjectiveTypes.Destroy)
            {
                Debug.Log("Making client not send kill objective packet.");
                bool shouldComplete = ObjectiveNetworker_Reciever.completeNext;
                ObjectiveNetworker_Reciever.completeNext = false;
                return shouldComplete;// clients should not send kill obj packets or have them complete
            }
            ObjectiveNetworker_Reciever.completeNext = false;
            Debug.Log("Client sent objective complete " + __instance.objectiveID);
            NetworkSenderThread.Instance.SendPacketToSpecificPlayer(Networker.hostID, objOutMessage, Steamworks.EP2PSend.k_EP2PSendUnreliableNoDelay);
        }
        return true;
    }
}

//patch to grab all the events being loaded on creation this replaces original method
[HarmonyPatch(typeof(MissionObjective), "FailObjective")]
class Patch5
{
    static bool Prefix(MissionObjective __instance)
    {
        //prevents infinite client host pings
        //if (__instance.failed)
        //    return true;
        Debug.Log("A mission got failed we need to send it");


        //Debug.Log("sending __instance.objectiveName + __instance.objectiveID");
        String actionIdentifier = __instance.objectiveName + __instance.objectiveID;

        Debug.Log(actionIdentifier);

        //dont run corrupt objectives
        if (MissionManager.instance.IndexOfObjective(__instance) == -1)
            return false;
        Message_ObjectiveSync objOutMessage = new Message_ObjectiveSync(PlayerManager.localUID, MissionManager.instance.IndexOfObjective(__instance), ObjSyncType.EMissionFailed);
        if (Networker.isHost && objOutMessage.objID != -1)
        {
            Debug.Log("Host sent objective fail " + __instance.objectiveID);
            NetworkSenderThread.Instance.SendPacketAsHostToAllClients(objOutMessage, Steamworks.EP2PSend.k_EP2PSendUnreliableNoDelay);
        }
        else
        {
            if (VTScenario.current.objectives.GetObjective(__instance.objectiveID).objectiveType == VTObjective.ObjectiveTypes.Destroy)
            {
                Debug.Log("Making client not send kill objective packet.");
                return true;// clients should not send kill obj packets
            }
            NetworkSenderThread.Instance.SendPacketToSpecificPlayer(Networker.hostID, objOutMessage, Steamworks.EP2PSend.k_EP2PSendUnreliableNoDelay);
        }
        return true;
    }
}

//patch to grab all the events being loaded on creation this replaces original method
[HarmonyPatch(typeof(MissionObjective), "BeginMission")]
class Patch6
{
    static bool Prefix(MissionObjective __instance)
    {
        //prevents infinite client host pings
        //if (__instance.started)
        //   return true;
        Debug.Log("A mission got BeginMission we need to send it");


        //Debug.Log("sending __instance.objectiveName + __instance.objectiveID");
        String actionIdentifier = __instance.objectiveName + __instance.objectiveID;

        Debug.Log(actionIdentifier);
        //dont run corrupt objectives
        if (MissionManager.instance.IndexOfObjective(__instance) == -1)
            return false;
        Message_ObjectiveSync objOutMessage = new Message_ObjectiveSync(PlayerManager.localUID, MissionManager.instance.IndexOfObjective(__instance), ObjSyncType.EMissionBegin);
        if (Networker.isHost && objOutMessage.objID != -1)
        {

            Debug.Log("Host sent objective BeginMission " + __instance.objectiveID);
            NetworkSenderThread.Instance.SendPacketAsHostToAllClients(objOutMessage, Steamworks.EP2PSend.k_EP2PSendUnreliableNoDelay);
        }
        else
        {
            if (VTScenario.current.objectives.GetObjective(__instance.objectiveID).objectiveType == VTObjective.ObjectiveTypes.Destroy)
            {
                Debug.Log("Making client not send kill objective packet.");
                return true;// clients should not send kill obj packets
            }
            Debug.Log("Client sent objective BeginMission " + __instance.objectiveID);
            NetworkSenderThread.Instance.SendPacketToSpecificPlayer(Networker.hostID, objOutMessage, Steamworks.EP2PSend.k_EP2PSendUnreliableNoDelay);
        }
        return true;
    }
}

//patch to grab all the events being loaded on creation this replaces original method
[HarmonyPatch(typeof(MissionObjective), "CancelObjective")]
class Patch7
{
    static bool Prefix(MissionObjective __instance)
    {
        //prevents infinite client host pings
        //if (__instance.cancelled)
        //   return true;

        Debug.Log("A mission got CancelObjective we need to send it");


        ///Debug.Log("sending __instance.objectiveName + __instance.objectiveID");
        String actionIdentifier = __instance.objectiveName + __instance.objectiveID;

        Debug.Log(actionIdentifier);

        //dont run corrupt objectives
        if (MissionManager.instance.IndexOfObjective(__instance) == -1)
            return false;
        Message_ObjectiveSync objOutMessage = new Message_ObjectiveSync(PlayerManager.localUID, MissionManager.instance.IndexOfObjective(__instance), ObjSyncType.EMissionCanceled);
        if (Networker.isHost && objOutMessage.objID != -1)
        {

            Debug.Log("Host sent objective CancelObjective " + __instance.objectiveID);
            NetworkSenderThread.Instance.SendPacketAsHostToAllClients(objOutMessage, Steamworks.EP2PSend.k_EP2PSendUnreliableNoDelay);
        }
        else
        {
            if (VTScenario.current.objectives.GetObjective(__instance.objectiveID).objectiveType == VTObjective.ObjectiveTypes.Destroy)
            {
                Debug.Log("Making client not send kill objective packet.");
                return true;// clients should not send kill obj packets
            }
            Debug.Log("Client sent objective CancelObjective " + __instance.objectiveID);
            NetworkSenderThread.Instance.SendPacketToSpecificPlayer(Networker.hostID, objOutMessage, Steamworks.EP2PSend.k_EP2PSendUnreliableNoDelay);
        }
        return true;
    }
}