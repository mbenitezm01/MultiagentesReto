using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class TCPIPServerAsync : MonoBehaviour
{

    [SerializeField]
    private GameObject carPrefab;

    private int carPop = -1;
    private List<GameObject> cars = new List<GameObject>();
    private Queue<string[]> movementQueue = new Queue<string[]>();

    // Use this for initialization

    System.Threading.Thread SocketThread;
    volatile bool keepReading = false;

    void Start()
    {
        Application.runInBackground = true;
        startServer();
    }

    private void Update()
    {
        if(carPop != -1)
        {
            for (int i = 0; i < carPop; i++)
            {
                cars.Add((GameObject)Instantiate(carPrefab, new Vector3(9, 0, 0), Quaternion.identity, transform));
             }
            carPop = -1;
        }

        
        if(movementQueue.Count != 0)
        {
            int id, x, z;

            id = Int32.Parse(movementQueue.Peek()[0]);
            x = Int32.Parse(movementQueue.Peek()[1]);
            z = Int32.Parse(movementQueue.Peek()[2]);

            Debug.Log("Car ID: " + id);
            Debug.Log("x: " + x);
            Debug.Log("z: " + z);

            //transform.GetChild(id).GetComponent<Car>().moveCar(x,z);

            transform.GetChild(id).position = Vector3.MoveTowards(transform.position, new Vector3(x, 0, z), 1f * Time.deltaTime);
            //transform.GetChild(id).position = Vector3.MoveTowards(transform.position, new Vector3(0, 0, 0), 1f * Time.deltaTime);


            movementQueue.Dequeue();
        }
        
    }

    void startServer()
    {
        SocketThread = new System.Threading.Thread(networkCode);

        SocketThread.IsBackground = true;
        SocketThread.Start();
    }



    private string getIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
            }

        }
        return localIP;
    }


    Socket listener;
    Socket handler;

    void networkCode()
    {
        string data;

        // Data buffer for incoming data.
        byte[] bytes = new Byte[1024];

        // host running the application.
        //Create EndPoint
        IPAddress IPAdr = IPAddress.Parse("127.0.0.1"); // Dirección IP
        IPEndPoint localEndPoint = new IPEndPoint(IPAdr, 1101);

        // Create a TCP/IP socket.
        listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and 
        // listen for incoming connections.

        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            // Start listening for connections.
            while (true)
            {
                keepReading = true;

                // Program is suspended while waiting for an incoming connection.
                Debug.Log("Waiting for Connection");     //It works

                handler = listener.Accept();
                Debug.Log("Client Connected");     //It doesn't work
                data = null;

                byte[] SendBytes = System.Text.Encoding.Default.GetBytes("I will send key");
                handler.Send(SendBytes); // dar al cliente

                bytes = new byte[1024];
                int bytesRec = handler.Receive(bytes);
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRec);
                carPop = Int32.Parse(data);

                // An incoming connection needs to be processed.
                while (keepReading)
                {
                    string[] carData;

                    bytes = new byte[1024];
                    bytesRec = handler.Receive(bytes);
                    data = "";

                    if (bytesRec <= 0)
                    {
                        keepReading = false;
                        handler.Disconnect(true);
                        break;
                    }

                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    string[] dataBuffer = (data.Split('$'));
                    for(int i = 0; i < dataBuffer.Length; i++)
                    {
                        if(dataBuffer[i] != "")
                        {
                            carData = (dataBuffer[i].Split(' '));
                            movementQueue.Enqueue(carData);
                        }
                    }

                    //Debug.Log(data);
                    


                    /*
                    for(int i = 0; i < carData.Length; i++)
                    {
                        Debug.Log("Received from Server: " + carData[i]);
                    }

                    
                    Debug.Log("Received from Server: " + data);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                    */

                    System.Threading.Thread.Sleep(1);
                }

                System.Threading.Thread.Sleep(1);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    void stopServer()
    {
        keepReading = false;

        //stop thread
        if (SocketThread != null)
        {
            SocketThread.Abort();
        }

        if (handler != null && handler.Connected)
        {
            handler.Disconnect(false);
            Debug.Log("Disconnected!");
        }
    }

    void OnDisable()
    {
        stopServer();
    }
}