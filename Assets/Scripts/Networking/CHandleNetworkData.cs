using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHandleNetworkData : MonoBehaviour
{
    private delegate void Packet_(byte[] data);
    private static Dictionary<int, Packet_> Packets;

    
    public static void InitializeNetworkPackages()
    {
        Debug.Log("Initialize Network Packages");
        Packets = new Dictionary<int, Packet_> { };
        Packets.Add((int)ServerPackets.SConnectionOK, HandleConnectionOK);
        Packets.Add((int)ServerPackets.StoC, HandleStoC);
    }

    public static void HandleNetworkInformation(byte[] data)
    {
        int packetnum; PacketBuffer buffer = new PacketBuffer();
        buffer.WriteBytes(data);
        packetnum = buffer.ReadInteger();
        buffer.Dispose();

        if (Packets.TryGetValue(packetnum, out Packet_ Packet))
        {
            Packet.Invoke(data);
        }
    }

    private static void HandleConnectionOK(byte[] data)
    {
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteBytes(data);
        buffer.ReadInteger();
        string msg = buffer.ReadString();
        buffer.Dispose();

        // Add Code to Ececute
        Debug.Log(msg);

        ClientTCP.ThankYouServer();
    }
    
    private static void HandleStoC(byte[] data)
    {
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteBytes(data);
        buffer.ReadInteger();
        string msg = buffer.ReadString();
        buffer.Dispose();

        // Add Code to Ececute
        Debug.Log(msg);

        
    }
}
