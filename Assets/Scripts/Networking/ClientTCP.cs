using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;

public class ClientTCP : MonoBehaviour
{
    public string IP_ADDRESS;
    public int PORT;
    
    private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private byte[] _asyncbuffer = new byte[1024];

    void Awake()
    {
        Application.runInBackground = true;
    }

    private void OnApplicationQuit()
    {
        _clientSocket.Disconnect(false);
        _clientSocket.Close();
        
        close = true;
    }

    public void ConnectToServer()
    {
        Debug.Log("Connecting to Server...");
        CHandleNetworkData.InitializeNetworkPackages();
        _clientSocket.BeginConnect(IP_ADDRESS, PORT, new AsyncCallback(ConnectCallback), _clientSocket);
    }

    public void LeaveServer()
    {
        _clientSocket.Disconnect(true);

        close = true;
    }

    
    
    
    private bool close = false;
    private void ConnectCallback(IAsyncResult ar)
    {
        _clientSocket.EndConnect(ar);
        while (close == false)
        {
            OnReceive();
        }
        
        Debug.Log("While true 1 breaked");
    }

    private static void OnReceive()
    {
        byte[] _sizeinfo = new byte[4];
        byte[] _receivedbuffer = new byte[1024];

        int totalread = 0, currentread = 0;

        try
        {
            currentread = totalread = _clientSocket.Receive(_sizeinfo);
            if(totalread <= 0)
            {
                Console.WriteLine("You are not connected to the server.");
            } else
            {
                while (totalread < _sizeinfo.Length && currentread > 0)
                {
                    currentread = _clientSocket.Receive(_sizeinfo, totalread, _sizeinfo.Length - totalread, SocketFlags.None);
                    totalread += currentread;
                }

                int messagesize = 0;
                messagesize |= _sizeinfo[0];
                messagesize |= (_sizeinfo[1] << 8);
                messagesize |= (_sizeinfo[2] << 16);
                messagesize |= (_sizeinfo[3] << 24);

                byte[] data = new byte[messagesize];

                totalread = 0;
                currentread = totalread = _clientSocket.Receive(data, totalread, data.Length - totalread, SocketFlags.None);
                while (totalread < messagesize && currentread > 0)
                {
                    currentread = _clientSocket.Receive(data, totalread, data.Length - totalread, SocketFlags.None);
                    totalread += currentread;
                }

                CHandleNetworkData.HandleNetworkInformation(data);
            }
        }
        catch
        {
            Debug.Log("You are not connected to the server.");
            throw;
        }
    }

    public static void SendData(byte[]data)
    {
        _clientSocket.Send(data);
    }

    public static void ThankYouServer()
    {
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteInteger((int)ClientPackets.CThankYou);
        buffer.WriteString("Connection Confirmation.");
        SendData(buffer.ToArray());
        buffer.Dispose();
    }

    public static void ClientToServer(string msg)
    {
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteInteger((int)ClientPackets.CtoS);
        buffer.WriteString(msg);
        SendData(buffer.ToArray());
        buffer.Dispose();

    }

    public static void AssignInfo(string msg)
    {
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteInteger((int)ClientPackets.CAssignInfo);
        buffer.WriteString(msg);
        SendData(buffer.ToArray());
        buffer.Dispose();

    }




}
