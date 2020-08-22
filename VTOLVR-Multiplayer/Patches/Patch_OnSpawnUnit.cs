﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Steamworks;
using UnityEngine;


[HarmonyPatch(typeof(UnitSpawner), "SpawnUnit")]
class Patch8
{
    static void Postfix(UnitSpawner __instance)
    {
        if (Networker.isHost)
        {
            Debug.Log("Setting up new AI.");
            AIManager.setupAIAircraft(__instance.spawnedUnit.actor);
            Debug.Log("Telling client about newly spawned AI.");
            Actor actor = __instance.spawnedUnit.actor;
            if (!actor.isPlayer)
            {
                if (actor.name.Contains("Client"))
                    return;
                bool Aggresion = false;
                if (actor.gameObject.GetComponent<UIDNetworker_Sender>() != null)
                {
                    Debug.Log("Try sending ai " + actor.name + " to client.");
                    HPInfo[] hPInfos2 = null;
                    int[] cmLoadout = null;
                    UIDNetworker_Sender uidSender = actor.gameObject.GetComponent<UIDNetworker_Sender>();
                    if (actor.role == Actor.Roles.Air)
                    {
                        WeaponManager wm = actor.gameObject.GetComponent<WeaponManager>();
                        if (wm != null)
                            hPInfos2 = PlaneEquippableManager.generateHpInfoListFromWeaponManager(actor.weaponManager, PlaneEquippableManager.HPInfoListGenerateNetworkType.sender, uidSender.networkUID).ToArray();
                    }
                    AIUnitSpawn aIUnitSpawn = actor.gameObject.GetComponent<AIUnitSpawn>();
                    if (aIUnitSpawn == null)
                    {
                        Debug.LogWarning("AI unit spawn is null on ai " + actor.name);
                    }
                    else
                    {
                        Aggresion = aIUnitSpawn.engageEnemies;
                    }
                    bool canBreak = false;
                    PhoneticLetters letters = new PhoneticLetters();
                    foreach (var Group in VTScenario.current.groups.GetExistingGroups(actor.team))
                    {
                        foreach (var ID in Group.unitIDs)
                        {
                            if (ID == actor.unitSpawn.unitID)
                            {
                                letters = Group.groupID;
                                canBreak = true;
                                break;
                            }
                        }
                        if (canBreak)
                            break;
                    }
                    List<ulong> ids = new List<ulong>();
                    ulong lastID;
                    SAMLauncher launcher = actor.gameObject.GetComponentInChildren<SAMLauncher>();
                    if (launcher != null)
                    {
                        foreach (var radar in launcher.lockingRadars)
                        {
                            if (radar.myActor == null)
                            {
                                Debug.LogError("Locking radar on one of the SAM's is literally null.");
                            }
                            if (AIDictionaries.reverseAllActors.TryGetValue(radar.myActor, out lastID))
                            {
                                ids.Add(lastID);
                                // Debug.Log("Aded a radar ID");
                            }
                            else
                                Debug.LogError("Couldn't get a locking radar on one of the SAM's, probably a dictionary problem.");
                        }
                    }
                    ulong[] irIDS = new ulong[0];
                    IRSAMNetworker_Sender irs = actor.gameObject.GetComponentInChildren<IRSAMNetworker_Sender>();
                    if (irs != null)
                    {
                        irIDS = irs.irIDs;
                    }
                    Debug.Log("Finally sending AI " + actor.name + " to client all clients.");
                    if (canBreak)
                    {
                        NetworkSenderThread.Instance.SendPacketAsHostToAllClients(new Message_SpawnAIVehicle(actor.name, AIManager.GetUnitNameFromCatalog(actor.unitSpawn.unitName),
                            VTMapManager.WorldToGlobalPoint(actor.gameObject.transform.position),
                            new Vector3D(actor.gameObject.transform.rotation.eulerAngles), uidSender.networkUID, hPInfos2, cmLoadout, 0.65f, Aggresion, actor.unitSpawn.unitSpawner.unitInstanceID, letters, ids.ToArray(), irIDS),
                            EP2PSend.k_EP2PSendReliable);
                    }
                    else
                    {
                        // Debug.Log("It seems that " + actor.name + " is not in a unit group, sending anyways.");
                        NetworkSenderThread.Instance.SendPacketAsHostToAllClients(new Message_SpawnAIVehicle(actor.name, AIManager.GetUnitNameFromCatalog(actor.unitSpawn.unitName),
                            VTMapManager.WorldToGlobalPoint(actor.gameObject.transform.position),
                            new Vector3D(actor.gameObject.transform.rotation.eulerAngles), uidSender.networkUID, hPInfos2, cmLoadout, 0.65f, Aggresion, actor.unitSpawn.unitSpawner.unitInstanceID, ids.ToArray(), irIDS),
                            EP2PSend.k_EP2PSendReliable);
                    }
                }
                else
                {
                    Debug.Log("Could not find the UIDNetworker_Sender");
                }
            }
        }
    }
}



