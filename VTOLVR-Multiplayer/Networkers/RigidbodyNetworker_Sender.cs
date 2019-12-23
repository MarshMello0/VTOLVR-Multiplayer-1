﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class RigidbodyNetworker_Sender : MonoBehaviour
{
    public ulong networkUID;
    private Rigidbody rb;
    private Message_RigidbodyUpdate lastMessage;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log("Added RB");
        Debug.Log("Setting last message");
        lastMessage = new Message_RigidbodyUpdate(new Vector3D(rb.velocity), new Vector3D(rb.angularVelocity), new Vector3D(transform.position), networkUID);
    }

    private void LateUpdate()
    {
        lastMessage.position = VTMapManager.WorldToGlobalPoint(transform.position);
        lastMessage.velocity = new Vector3D(rb.velocity);
        lastMessage.angularVelocity = new Vector3D(rb.angularVelocity);
        if (Networker.isHost)
            Networker.SendGlobalP2P(lastMessage, Steamworks.EP2PSend.k_EP2PSendUnreliableNoDelay);
        else
            Networker.SendP2P(Networker.hostID, lastMessage, Steamworks.EP2PSend.k_EP2PSendUnreliableNoDelay);
    }
}