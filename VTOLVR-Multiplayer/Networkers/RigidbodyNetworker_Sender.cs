﻿using System;
using System.Collections;
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
    public Vector3 originOffset;
    private Vector3D globalLastPosition;
    private Vector3 localLastPosition;
    private Vector3 lastVelocity;
    private Vector3 lastUp;
    private Vector3 lastForward;
    private Quaternion lastRotation;
    private Vector3 lastAngularVelocity;
    private float threshold = 0.5f;
    private float angleThreshold = 5f;
    
    private ulong updateNumber;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lastMessage = new Message_RigidbodyUpdate(new Vector3D(), new Vector3D(), new Vector3D(), Quaternion.identity, 0, networkUID);
    }

    private void FixedUpdate()
    {
        globalLastPosition += new Vector3D(lastVelocity * Time.fixedDeltaTime);
        localLastPosition = VTMapManager.GlobalToWorldPoint(globalLastPosition);
        Quaternion quatVel = Quaternion.Euler(lastAngularVelocity * Time.fixedDeltaTime);
        lastRotation *= quatVel;

        lastUp = lastRotation * Vector3.up;
        lastForward = lastRotation * Vector3.forward;

        if (Vector3.Distance(localLastPosition, transform.TransformPoint(originOffset)) > threshold || Vector3.Angle(lastUp, transform.up) > angleThreshold || Vector3.Angle(lastForward, transform.forward) > angleThreshold)
        {
            lastUp = transform.up;
            lastForward = transform.forward;

            globalLastPosition = VTMapManager.WorldToGlobalPoint(transform.TransformPoint(originOffset));
            lastVelocity = rb.velocity;

            lastRotation = transform.rotation;
            lastAngularVelocity = rb.angularVelocity * Mathf.Rad2Deg;

            lastMessage.position = VTMapManager.WorldToGlobalPoint(transform.TransformPoint(originOffset));
            lastMessage.rotation = transform.rotation;
            if (Multiplayer.SoloTesting)
                lastMessage.position += new Vector3D(-30, 0, 0);
            lastMessage.velocity = new Vector3D(rb.velocity);
            lastMessage.angularVelocity = new Vector3D(rb.angularVelocity * Mathf.Rad2Deg);
            lastMessage.networkUID = networkUID;
            lastMessage.sequenceNumber = ++updateNumber;
            if (Networker.isHost)
                NetworkSenderThread.Instance.SendPacketAsHostToAllClients(lastMessage, Steamworks.EP2PSend.k_EP2PSendUnreliableNoDelay);
            else
                NetworkSenderThread.Instance.SendPacketToSpecificPlayer(Networker.hostID, lastMessage, Steamworks.EP2PSend.k_EP2PSendUnreliableNoDelay);
        }
            // Temperz STOP KILLING PERFORMANCE AND HARD DRIVES!
            //Debug.Log($"{actor.name} is not outside of the threshold {Threshold}, the distance is {Vector3.Distance(lastPos, gameObject.transform.position)} not updating it.");
    }

    public void SetSpawn(Vector3 spawnPos, Quaternion spawnRot)
    {
        StartCoroutine(SetSpawnEnumerator(spawnPos, spawnRot));
    }

    private IEnumerator SetSpawnEnumerator(Vector3 spawnPos, Quaternion spawnRot)
    {
        yield return new WaitForSeconds(0.5f);
        rb.position = spawnPos;
        rb.rotation = spawnRot;
        Debug.Log($"Our position is now {rb.position}");
    }
}
