using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class Server : MonoBehaviour
{
    public bool IsOpened { get; private set; } = false;

    //public string serverIP = "127.0.0.1";
    //public int PortNum = 7778;


    public TMP_InputField IPInput;
    public TMP_InputField IPPort;

    TcpListener listener;
    List<TcpClient> connectedClients;
    List<TcpClient> disConnectedClients;

    private void Start()
    {
        //Open();
    }

    private void OnDisable()
    {
        Close();
    }

    private void Update()
    {
        if (!IsOpened)
            return;

        foreach (TcpClient client in connectedClients)
        {
            if (!ClientConnectCheck(client))
            {
                client.Close();
                disConnectedClients.Add(client);
                continue;
            }
            NetworkStream stream = client.GetStream();
            if (stream.DataAvailable)
            {
                StreamReader reader = new StreamReader(stream);
                string chat = reader.ReadLine();
                if (chat != null)
                    SendAll(chat);
            }
        }

        for (int i = 0; i < disConnectedClients.Count; i++)
        {
            connectedClients.Remove(disConnectedClients[i]);
        }
        disConnectedClients.Clear();
    }



    public void Open()
    {
        if (IsOpened)
            return;

        connectedClients = new List<TcpClient>();
        disConnectedClients = new List<TcpClient>();
        try
        {

            if (IPPort.text == "")
            {
                IPPort.text = "7778";
            }

            listener = new TcpListener(IPAddress.Any, int.Parse(IPPort.text));
            listener.Start();

            Debug.Log("SERVER: Server Opened");
            IsOpened = true;
        }
        catch (Exception e)
        {
            Debug.LogError("SERVER: Server Open Fail : " + e.Message);
            Close();
        }

        listener.BeginAcceptTcpClient(AcceptCallback, listener); // Ŭ���̾�Ʈ �����غ� ����
    }

    private void AcceptCallback(IAsyncResult ar)
    {
        Debug.Log("SEVER: AcceptCallback");

        if (IsOpened == false)
        {
            Debug.Log("SERVER: AcceptCall But Server Not Open ");
            return;
        }

        if (listener == null)
        {
            Debug.Log("SERVER: AcceptCall But Listener is null ");
            return;
        }

        TcpClient client = listener.EndAcceptTcpClient(ar); // �ϴ� ���߰�
        connectedClients.Add(client); // ���� �� Ŭ���̾�Ʈ �߰�
        Debug.Log("SERVER: Client connected");
        listener.BeginAcceptTcpClient(AcceptCallback, null); //�ٽ� ����
    }


    public void Close()
    {
        listener?.Stop();
        listener = null;
        IsOpened = false;

        Debug.Log("SERVER: Server Closed");
    }


    public void SendAll(string chat)
    {
        Debug.Log("SERVER: SendAll "+ chat);

        foreach (TcpClient client in connectedClients)
        {
            StreamWriter writer = new StreamWriter(client.GetStream());
            writer.WriteLine(chat);
            writer.Flush();
        }
    }

    private bool ClientConnectCheck(TcpClient client)
    {
        try
        {
            if (client != null && client.Client != null && client.Connected)
            {
                if (client.Client.Poll(0, SelectMode.SelectRead))
                    return !(client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true;
            }
            else
                return false;
        }
        catch (Exception e)
        {
            Debug.LogError("SEVER: Connect Check Error :" + e.Message);
            return false;
        }
    }


}
