using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDP_Reciever : MonoBehaviour {

    // receiving Thread
    Thread receiveThread;

    // udpclient object
    UdpClient client;

    public int port; // define > init

    public float[] dataArray;

    public Vector3 pointerFingerPos;
    public bool hasPointerFinger;
    public Vector3 paletteCenterPos;
    public bool hasPalette;
    public float clicked;
    public float numFingers;
    public GameObject board;

    public GameObject pointerFinger;
    public GameObject palette;
    public GameObject drawing;

    private char[] seperatingChars = { '%' };
    private bool palette_grabbed = true;

    public bool drawState = true;
    public bool fiveFingerState = false;

    // infos
    public string lastReceivedUDPPacket = "";

    // Start from Unity3D
    void Start () {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.init()");

        // define port
        port = 8051;

        // status
        print("Sending to 127.0.0.1 : " + port);
        print("Test-Sending to this Port: nc -u 127.0.0.1  " + port + "");

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        palette_grabbed = false;
        palette.SetActive(true);
    }

    // OnGUI
    void OnGUI()
    {
        
        Rect rectObj = new Rect(40, 10, 200, 400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj, "# UDPReceive\n127.0.0.1 " + port + " #\n"
                + "shell> nc -u 127.0.0.1 : " + port + " \n"
                + "\nLast Packet: \n" + lastReceivedUDPPacket
                // + "\n\nAll Messages: \n"+allReceivedUDPPackets
                , style);
        
        string[] strs = lastReceivedUDPPacket.Split(seperatingChars);
        clicked = float.Parse(strs[6]);
        numFingers = float.Parse(strs[7]);
        if (numFingers > 1)
        {
            fiveFingerState = true;
        }
        if(numFingers <= 1 && fiveFingerState)
        {
            drawState = !drawState;
            fiveFingerState = false;
        }

        // has finger
        if (strs[0] != "-" && numFingers == 1)
        {
            pointerFinger.SetActive(true);
            if (clicked == 1)
            {
                pointerFingerPos = new Vector3(float.Parse(strs[0]) * 100 - 5.5f, float.Parse(strs[1]) * 100 + 4.5f, float.Parse(strs[2]) * 100);
            }
            else
            {
                pointerFingerPos = new Vector3(float.Parse(strs[0]) * 100 - 3.5f, float.Parse(strs[1]) * 100 + 1f, float.Parse(strs[2]) * 100);
            }
            hasPointerFinger = true;
            pointerFinger.transform.position = pointerFingerPos;
        } else {
            hasPointerFinger = false;
            pointerFinger.SetActive(false);
        }
        // No palette
        if (palette_grabbed == true)
        {
            if (strs[3] != "-")
            {
                palette.SetActive(true);
                drawing.SetActive(false);
                paletteCenterPos = new Vector3(float.Parse(strs[3]) * 100 - 5f, float.Parse(strs[4]) * 100 + 3, float.Parse(strs[5]) * 100);
                hasPalette = true;
                palette.transform.position = paletteCenterPos;
            }
            else
            {
                palette.SetActive(false);
                drawing.SetActive(true);
                hasPalette = false;
            }
        }
    }


    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            palette.SetActive(false);
        }

        if(Input.GetKeyUp("space"))
        {
            palette.transform.parent = null;
            palette_grabbed = true;
        }
    }

    // receive thread
    private void ReceiveData()
    {

        client = new UdpClient(port);
        while (true)
        {

            try
            {
                // Bytes empfangen.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
                string text = Encoding.UTF8.GetString(data);

                // Den abgerufenen Text anzeigen.
                //print(">> " + text);

                // latest UDPpacket
                lastReceivedUDPPacket = text;
                // ....
                // allReceivedUDPPackets=allReceivedUDPPackets+text;

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    // getLatestUDPPacket and cleans up the rest
    public string getLatestUDPPacket()
    {
        // allReceivedUDPPackets="";
        string tempPacket = lastReceivedUDPPacket;
        lastReceivedUDPPacket = "";
        return tempPacket;
    }
}
