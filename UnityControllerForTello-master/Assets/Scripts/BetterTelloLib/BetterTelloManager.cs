using BetterTelloLib.Commander;
using BetterTelloLib.Commander.Events.EventArgs;
using BetterTelloLib.Commander.Factories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityControllerForTello;
using UnityEngine;
using UnityEngine.UIElements;

public class BetterTelloManager : MonoBehaviour
{
    public BetterTello BetterTello = new();
    public TelloConnectionState ConnectionState = TelloConnectionState.Disconnected;
    public float Height = 0;
    public Vector3 PositionVel = Vector3.zero;
    public Vector3 PositionAcc = Vector3.zero;
    public Vector3 Position = Vector3.zero;
    public float ExtTof = 0;
    public int Bat = 0;
    public Quaternion PYR = new();
    private float Pitch = 0;
    private float Roll = 0;
    private float Yaw = 0;
    private TelloVideoTexture telloVideoTexture;
    private SceneManager sceneManager;
    private InputController inputController;
    private FlightPathController flightPathController;
    public Transform Transform;

    private bool waitingForOk = false;


    private void Start()
    {
        gameObject.SetActive(true);
        ConnectToTello();
        Transform = GetComponent<Transform>();
    }
    public void ConnectToTello()
    {
        ConnectionState = TelloConnectionState.Connecting;
        BetterTello.Events.OnStateRecieved += OnStateUpdate;
        BetterTello.Events.OnVideoDataRecieved += Tello_onVideoData;
        BetterTello.Connect();
        BetterTello.Factories.OnTaskRecieved += TaskRecieved;
        ConnectionState = TelloConnectionState.Connected;
        BetterTello.Commands.SetBitrate(0);
        Task.Factory.StartNew(async ()=> await Run());
    }

    private void TaskRecieved(object? sender, TaskRecievedEventArgs e)
    {
        if (e.Received.Contains("ok"))
            waitingForOk = false;
    }

    public void CustomOnApplicationQuit()
    {
        BetterTello.Events.OnStateRecieved -= OnStateUpdate;
        BetterTello.Events.OnVideoDataRecieved -= Tello_onVideoData;
        BetterTello.Factories.OnTaskRecieved -= TaskRecieved;

        BetterTello.Dispose();
    }
    void Awake()
    {
        this.flightPathController = GetComponent<FlightPathController>();
        if (telloVideoTexture == null)
            telloVideoTexture = FindObjectOfType<TelloVideoTexture>();
    }

    public void EmergencyStop()
    {
        BetterTello.Commands.Stop();
        BetterTello.Commands.Land();
        //CustomOnApplicationQuit();
    }

    private void Update()
    {
        UpdateTransform();
    }
    private void FixedUpdate()
    {
        flightPathController.CreateFlightPoint();
    }
    public async Task<int> Takeoff() => await RunCommand(BetterTello.Commands.Takeoff);
    public async Task<int> Land() => await RunCommand(BetterTello.Commands.Land);
    public async Task<int> Up(int x) => await RunCommand(BetterTello.Commands.Up, x);
    public async Task<int> Forward(int x) => await RunCommand(BetterTello.Commands.Forward, x);
    public async Task<int> Back(int x) => await RunCommand(BetterTello.Commands.Back, x);
    public async Task Run()
    {
        await Takeoff();
        await Up(50);
        await Forward(100);
        await Forward(100);
        await Back(100);
        await Back(100);
        await Land();
    }
    public async Task<int> RunCommand(Func<int, int> Function, int x)
    {
        Debug.Log("Sending command: " + Function.Method.Name + $"({x})");
        var ret = Function(x);
        await WaitForOk();
        return ret;
    }
    public async Task<int> RunCommand(Func<int> Function)
    {
        Debug.Log("Sending command: " + Function.Method.Name + "()");
        var ret = Function();
        await WaitForOk();
        return ret;
    }

    public async Task WaitForOk()
    {
        waitingForOk = true;
        while (waitingForOk)
            await Task.Delay(10);
    }


    public void TakeOff()
    {
        Debug.Log("TakeOff!");
        var preFlightPanel = GameObject.Find("Pre Flight Panel");
        if (preFlightPanel)
            preFlightPanel.SetActive(false);
        BetterTello.Commands.Takeoff();
        flightPathController.TakeOff(this);
    }

    public void OnStateUpdate(object? sender, StateEventArgs e)
    {
        var state = e.State;
        Position += new Vector3(state.Vgx, state.Vgy, state.Vgz);
        Pitch = state.Pitch;
        Roll = state.Roll;
        Yaw = state.Yaw;
        PYR = Quaternion.Euler(Pitch, Yaw, Roll);
        ExtTof = state.ExtTof;
        Bat = state.Bat;
        Height = state.H;
    }

    public void UpdateTransform()
    {
        if (ConnectionState == TelloConnectionState.Connected)
        {
            Transform.position = Position;
            Transform.rotation = PYR;
        }
    }



    private void Tello_onVideoData(object? sender, VideoDataRecievedEventArgs data)
    {
        if (telloVideoTexture != null)
            telloVideoTexture.PutVideoData(data.Data);
    }
}
