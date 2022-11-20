using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.NetworkInformation;

public class ServerManager
{
    private Socket receiveSocket;
    private Socket sentSocket;
    private IPEndPoint localEndPoint;
    private IPEndPoint remoteEndPoint;
    private bool Connecting = true;
    public static (List<IPAddress>, List<IPAddress>) GetIPS()
    {
        List<IPAddress> iplist = new List<IPAddress>();

        foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                iplist.Add(ip);
        }
        List<IPAddress> broadcastlist = new List<IPAddress>(iplist);

        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            var IPS = item.GetIPProperties().UnicastAddresses;
            for (int i = 0; i < IPS.Count; i++)
            {
                IPAddress iP = IPS[i].Address;
                for (int j = 0; j < iplist.Count; j++)
                {
                    if (!iplist[j].Equals(iP))
                        continue;
                    byte[] ipaddress = IPS[i].Address.GetAddressBytes();
                    byte[] ipMask = IPS[i].IPv4Mask.GetAddressBytes();
                    for (int k = 0; k < ipaddress.Length; k++)
                    {
                        ipaddress[k] = (byte)((~ipMask[k]) | ipaddress[k]);
                    }
                    broadcastlist[j] = new IPAddress(ipaddress);
                }
            }
        }
        return (iplist, broadcastlist);
    }
    public async System.Threading.Tasks.Task EnableServer()
    {
        List<IPAddress> ipList;
        List<IPAddress> broadcastlist;

        (ipList, broadcastlist) = GetIPS();

        receiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        localEndPoint = new IPEndPoint(IPAddress.Any, GameManager.ReciveUDPport);
        receiveSocket.Bind(localEndPoint);

        sentSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        sentSocket.Bind(new IPEndPoint(IPAddress.Any,GameManager.SentUDPport));


        for (int i = 0; i < ipList.Count; i++)
        {
            BroadCastIP(ipList[i], broadcastlist[i], i);
        }

        await UDPListen();
    }

    private async void BroadCastIP(IPAddress iPAddress, IPAddress broadCastAdress, int index)
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
        // socket.Bind(new IPEndPoint(iPAddress, GameManager.SentUDPport));
        string massage = "master";
        byte[] data = Encoding.ASCII.GetBytes(massage);
        IPEndPoint endPoint = new IPEndPoint(broadCastAdress, GameManager.ReciveUDPport);

        while (Connecting)
        {

            socket.SendTo(data, endPoint);

            await System.Threading.Tasks.Task.Delay(5000);
        }

    }

    private async System.Threading.Tasks.Task UDPListen()
    {
        System.ArraySegment<byte> data = new System.ArraySegment<byte>(new byte[1024]);
        EndPoint endPoint = new IPEndPoint(IPAddress.Any, GameManager.ReciveUDPport);
        while (Connecting)
        {
            SocketReceiveFromResult res = await receiveSocket.ReceiveFromAsync(data, SocketFlags.None, endPoint);
            string message = Encoding.ASCII.GetString(data.Array, 0, res.ReceivedBytes);
            if (message.Equals("success"))
            {
                Connecting = false;
                Debug.Log($"ַ服务端收到ַ客户端IP地址{message}");
                // Debug.Log($"客户端IP和端口号{_clientSocket.LocalEndPoint}");
                Debug.Log($"客户端IP和端口号{res.RemoteEndPoint}");
                remoteEndPoint = res.RemoteEndPoint as IPEndPoint;
                remoteEndPoint.Port=GameManager.ReciveUDPport;
            }
        }
    }

    public void SendMessage(string message){
        System.ArraySegment<byte> data = new System.ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        sentSocket.SendToAsync(data,SocketFlags.None,remoteEndPoint);
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
}
