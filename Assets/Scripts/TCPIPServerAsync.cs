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

    [SerializeField]
    private GameObject lightController;

    [Range(1, 60)]
    public int stepFrames;


    private int carPop = -1;
    private bool carInit = false;
    private string[] carsInitialPos;
    private List<GameObject> cars = new List<GameObject>();
    //private Queue<string[]> movementQueue = new Queue<string[]>();
    private Queue<Step> stepQueue = new Queue<Step>();

    private int frameCount = 0;

    struct Step
    {
        public List<string[]> carData;
        public List<string[]> lightData;
    }

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

        //if(carPop != -1)
        if(carInit)
        {
            for (int i = 0; i < carPop; i++)
            {
                string[] carPos = carsInitialPos[i].Split(' ');
                cars.Add((GameObject)Instantiate(carPrefab, new Vector3(Int32.Parse(carPos[0]), 0, Int32.Parse(carPos[1])), Quaternion.identity, transform));
                cars[i].GetComponent<Car>().nextX = Int32.Parse(carPos[0]);
                cars[i].GetComponent<Car>().nextZ = Int32.Parse(carPos[1]);
                cars[i].GetComponent<Car>().nextAngle = Quaternion.identity;

            }
            carInit = false;
        }

        if (frameCount >= stepFrames)
        {
            if (stepQueue.Count != 0)
            {
                Step currentStep = stepQueue.Dequeue();

                for (int i = 0; i < currentStep.carData.Count; i++)
                {
                    int id, x, z;
                    string dir, respawning;

                    id = Int32.Parse(currentStep.carData[i][0]);
                    x = Int32.Parse(currentStep.carData[i][1]);
                    z = Int32.Parse(currentStep.carData[i][2]);
                    dir = currentStep.carData[i][3];
                    respawning = currentStep.carData[i][4];

                    //Debug.Log("Car ID: " + id);
                    //Debug.Log("x: " + x);
                    //Debug.Log("z: " + z);

                    //transform.GetChild(id).position = new Vector3(x, 0, z);
                    //transform.GetChild(id).position = Vector3.MoveTowards(transform.GetChild(id).position, new Vector3(x, 0, z), 1);
                    transform.GetChild(id).GetComponent<Car>().nextX = x;
                    transform.GetChild(id).GetComponent<Car>().nextZ = z;
                    transform.GetChild(id).GetComponent<Car>().respawning = respawning;


                    switch (dir)
                    {
                        case "up":
                            //transform.GetChild(id).rotation = Quaternion.Euler(0, 90, 0);
                            transform.GetChild(id).GetComponent<Car>().nextAngle = Quaternion.Euler(0, 90, 0);
                            break;
                        case "down":
                            //transform.GetChild(id).rotation = Quaternion.Euler(0, -90, 0);
                            transform.GetChild(id).GetComponent<Car>().nextAngle = Quaternion.Euler(0, -90, 0);
                            break;
                        case "left":
                            //transform.GetChild(id).rotation = Quaternion.Euler(0, 0, 0);
                            transform.GetChild(id).GetComponent<Car>().nextAngle = Quaternion.Euler(0, 0, 0);
                            break;
                        case "right":
                            //transform.GetChild(id).rotation = Quaternion.Euler(0, 180, 0);
                            transform.GetChild(id).GetComponent<Car>().nextAngle = Quaternion.Euler(0, 180, 0);
                            break;
                    }
                }

                for (int i = 0; i < currentStep.lightData.Count; i++)
                {
                    string dir, color;
                    dir = currentStep.lightData[i][0];
                    color = currentStep.lightData[i][1];

                    lightController.GetComponent<StoplightController>().changeLights(dir, color);
                }
                frameCount = -1;
            }
        }

        for(int i = 0; i < carPop; i++)
        {
            int x = transform.GetChild(i).GetComponent<Car>().nextX;
            int z = transform.GetChild(i).GetComponent<Car>().nextZ;
            Quaternion angle = transform.GetChild(i).GetComponent<Car>().nextAngle;
            //Debug.Log("ID: " + i);
            //Debug.Log("X: " + x);
            //Debug.Log("Z: " + z);

            if (transform.GetChild(i).GetComponent<Car>().respawning == "false")
            {
                transform.GetChild(i).position = Vector3.MoveTowards(transform.GetChild(i).position, new Vector3(x, 0, z), 1f/stepFrames);
                transform.GetChild(i).rotation = Quaternion.Slerp(transform.GetChild(i).rotation, angle, 1f/stepFrames);
            }
            else
            {
                transform.GetChild(i).position = new Vector3(x, 0, z);
                transform.GetChild(i).rotation = angle;
            }
        }
        frameCount++;
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

                bytesRec = handler.Receive(bytes);
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRec);
                carsInitialPos = data.Split('$');
                carInit = true;

                // An incoming connection needs to be processed.
                while (keepReading)
                {

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
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        keepReading = false;
                        break;
                    }

                    string[] dataBuffer = (data.Split('%'));
                    string[] carDataBuffer = (dataBuffer[0].Split('$'));
                    string[] lightDataBuffer = (dataBuffer[1].Split('$'));

                    Step paso;
                    paso.carData = new List<string[]>();
                    paso.lightData = new List<string[]>();

                    for (int i = 0; i < carDataBuffer.Length; i++)
                    {
                        if(carDataBuffer[i] != "")
                        {
                            paso.carData.Add(carDataBuffer[i].Split(' '));
                        }
                    }

                    for (int i = 0; i < lightDataBuffer.Length; i++)
                    {
                        if(lightDataBuffer[i] != "")
                        {
                            paso.lightData.Add(lightDataBuffer[i].Split(' '));
                        }
                    }

                    stepQueue.Enqueue(paso);
                    handler.Send(System.Text.Encoding.Default.GetBytes("Wait"));

                    System.Threading.Thread.Sleep(1);
                }

                System.Threading.Thread.Sleep(1);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        Debug.Log("Simulation end");
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