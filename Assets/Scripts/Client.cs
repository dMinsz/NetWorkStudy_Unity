using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Client : MonoBehaviour
{
    public InputField nickname;
    public TMP_InputField IPInput;
    public TMP_InputField IPPort;

    private TcpClient tcpClient; // TCP 통신용 클라이언트 // 소켓으로 직접 안만들어도 C#에서 제공해준다.
    private NetworkStream stream; // 송수신 통신용 통로, 원래 패킷을 구성해서 하나하나 만들어줘야한다.

    private StreamWriter writer; // 송신용
    private StreamReader reader; // 수신용

    private string clientName;
   
    public bool isConnected { get; private set; } = false;

    private void OnDisable()
    {
        DisconnectedFromServer();
    }

    private void Start()
    {
        //ConnectedToServer();
    }

    private void Update()
    {
        if (isConnected && stream.DataAvailable)
        {
            ReceiveChat();
        }
    }


    public void ConnectedToServer()
    {
        if (isConnected)
            return;
        try
        {


            if (IPInput.text == "")
            {
                IPInput.text = "127.0.0.1";
            }

            if (IPPort.text == "")
            {
                IPPort.text = "7778";
            }


            tcpClient = new TcpClient(IPInput.text.ToString(), int.Parse(IPPort.text)); // 127.0.0.1 은 루프백 즉 자기자신이다.
            stream = tcpClient.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            //Connect Success

            Debug.Log("TCP Connect Success");
            isConnected = true;
        }
        catch (Exception e)
        {

            Debug.LogError("TCP Connect Fail : " + e.Message);
            DisconnectedFromServer();
        }


        SendChat(" 두둥등장!");


    }

    public void DisconnectedFromServer()
    {
        reader?.Close();
        reader = null;
        writer?.Close();
        writer = null;
        stream?.Close();
        stream = null;

        tcpClient?.Close();
        tcpClient = null;

        isConnected = false;
    }

    public void SendChat(string message) 
    {

        if (!isConnected)
        {
            Debug.LogError("Client is not connected");
            return;
        }

        try
        {
            clientName = nickname.text == "" ? "NoName" : nickname.text;

            writer.WriteLine(string.Format("{0} : {1}", clientName, message));
            writer.Flush();
        }
        catch (Exception e)
        {
            Debug.LogError("Send chat Fail " + e.Message);
        }
    }

    public void ReceiveChat() 
    {
        if (!isConnected)
        {
            Debug.LogError("ReceiveChat Error : Client is not connected");
            return;
        }

        string chat = reader.ReadLine();
        Chat.instance.AddMessage(chat);

    }
}
