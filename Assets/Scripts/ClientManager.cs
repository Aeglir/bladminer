using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.NetworkInformation;

public class ClientManager
{
    private Socket receiveSocket;
    private Socket sentSocket;
    private IPEndPoint remoteEndPoint;
    private IPEndPoint localEndPoint;
    private bool Connecting;
    public bool CONNECTING
    {
        get => Connecting;
    }
    private const int MaxConnectNum = 1;
    public async System.Threading.Tasks.Task EnableClient()
    {
        receiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //List<IPAddress> ipList;
        //List<IPAddress> broadcastlist;
        //(ipList, broadcastlist) = ServerManager.GetIPS();

        //clientEndPoint = new IPEndPoint(ipList[0],UDPPort);

        localEndPoint = new IPEndPoint(IPAddress.Any, GameManager.ReciveUDPport);

        receiveSocket.Bind(localEndPoint);

        sentSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        sentSocket.Bind(new IPEndPoint(IPAddress.Any,GameManager.SentUDPport));


        remoteEndPoint = await UDPListenning() as IPEndPoint;

        remoteEndPoint.Port = GameManager.ReciveUDPport;

        string messToSend = "success";
        Debug.Log($"send message to {remoteEndPoint}");
        sentSocket.SendTo(Encoding.ASCII.GetBytes(messToSend), remoteEndPoint);

        // _clientSocket = new Socket()
    }

    private async System.Threading.Tasks.Task<EndPoint> UDPListenning()
    {
        System.ArraySegment<byte> data = new System.ArraySegment<byte>(new byte[1024]);
        EndPoint endPoint = new IPEndPoint(IPAddress.Any, GameManager.ReciveUDPport);
        Connecting = true;
        while (Connecting)
        {
            SocketReceiveFromResult res = await receiveSocket.ReceiveFromAsync(data, SocketFlags.None, endPoint);
            string message = Encoding.ASCII.GetString(data.Array, 0, res.ReceivedBytes);
            if (message.Equals("master"))
            {
                Debug.Log($"客户端收到ַ服务端IP地址{message}");
                // Debug.Log($"客户端IP和端口号{_clientSocket.LocalEndPoint}");
                Debug.Log($"服务端IP和端口号{res.RemoteEndPoint}");
                Connecting = false;
                return res.RemoteEndPoint;
            }
        }
        return endPoint;
    }

    public async System.Threading.Tasks.Task<string> ReceiveMessage()
    {
        Connecting = true;
        System.ArraySegment<byte> data = new System.ArraySegment<byte>(new byte[1024]);
        Debug.Log($"服务端IP和端口号{localEndPoint}");
        SocketReceiveFromResult res = await receiveSocket.ReceiveFromAsync(data, SocketFlags.None, localEndPoint);
        string message = Encoding.ASCII.GetString(data.Array, 0, res.ReceivedBytes);
        Connecting = false;
        return message;
    }

    public void SendMessage(string message){
        System.ArraySegment<byte> data = new System.ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        sentSocket.SendToAsync(data,SocketFlags.None,remoteEndPoint);
    }
}
