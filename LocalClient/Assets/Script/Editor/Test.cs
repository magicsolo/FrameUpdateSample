using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int port;
    private Socket nSocket;
    // Start is called before the first frame update
    void Start()
    {
        nSocket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
        IPAddress ipAdress = IPAddress.Parse("192.168.50.22");
        IPEndPoint udpIPPoint = new IPEndPoint(ipAdress, port);
        nSocket.Connect(udpIPPoint);
    }
    int wait = 0;

    // Update is called once per frame
    void Update()
    {
        //if (wait>30)
        {
            nSocket.Send(new byte[] { 1 }, SocketFlags.None);
            wait = 0;
        
        }
        
        ++wait;

    }
}
