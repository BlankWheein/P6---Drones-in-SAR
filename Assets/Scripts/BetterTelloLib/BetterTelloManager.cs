using BetterTelloLib.Commander;
using BetterTelloLib.Commander.Events.EventArgs;
using BetterTelloLib.Commander.Factories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class BetterTelloManager : MonoBehaviour
{
    [Header("Variables")]
    [Min(3)]
    public float DegreePrecision = 3f;
    public float TargetTransformPrecision = 2f;
    [Min(3)]
    public int ScanDegrees = 5;
    [Min(750)]
    public int ExtTofDistance = 800;
    [Min(10)]
    public int ForwardDistance = 70;

    [Header("State")]
    public float DistanceToTarget = 0;
    public TelloConnectionState ConnectionState = TelloConnectionState.Disconnected;
    public FlyingState FlyingState = FlyingState.Grounded;
    public float TempH = 0;
    public float TempL = 0;
    public float ExtTof = 0;
    public int Bat = -1;
    public int Tof;
    public Vector3 PositionVel = Vector3.zero;
    public Quaternion PYR = new();
    public Vector3 PositionMissionPad = Vector3.zero;
    private float Pitch = 0;
    private float Roll = 0;
    private float Yaw = 0;

    public BetterTello BetterTello = new();
    private Transform Transform;
    public ShowGoldenPath ShowGoldenPath;
    private float Height = 0;
    private RenderVideoStream telloVideoTexture;
    private FlightPathController flightPathController;

    private List<int> Timestamps = new();
    private List<Vector3> Vels = new();
    private bool waitingForOk = false;
    public bool IsPathfinding = false;



    private void Start()
    {
        gameObject.SetActive(true);
        ConnectToTello();
        Transform = GetComponent<Transform>();
    }
    public async void ConnectToTello()
    {
        flightPathController.drawFlightPath = false;
        ConnectionState = TelloConnectionState.Connecting;
        BetterTello.Events.OnStateRecieved += OnStateUpdate;
        BetterTello.Events.OnVideoDataRecieved += Tello_onVideoData;
        BetterTello.Events.OnOkRecieved += OkRecieved;
        BetterTello.Connect();
        BetterTello.Commands.SetBitrate(0);
        StartCoroutine(CheckConnected());

    }
    IEnumerator CheckConnected()
    {
        yield return new WaitForSeconds(10);
        if (ConnectionState != TelloConnectionState.Connected)
            ConnectionState = TelloConnectionState.CouldNotConnect;
    }

    private void OkRecieved(object? sender, TaskRecievedEventArgs e)
    {
        waitingForOk = false;
        ConnectionState = TelloConnectionState.Connected;
        BetterTello.Factories.ExtTofDelay = 100;
    }

    public void CustomOnApplicationQuit()
    {
        BetterTello.Events.OnStateRecieved -= OnStateUpdate;
        BetterTello.Events.OnVideoDataRecieved -= Tello_onVideoData;
        BetterTello.Factories.OnTaskRecieved -= OkRecieved;
        BetterTello.Events.OnOkRecieved -= OkRecieved;

        Timestamps.Clear();
        BetterTello.Dispose();
    }
    void Awake()
    {
        this.flightPathController = GetComponent<FlightPathController>();
        ShowGoldenPath = GetComponent<ShowGoldenPath>();
        if (telloVideoTexture == null)
            telloVideoTexture = FindObjectOfType<RenderVideoStream>();
    }
        
    public void EmergencyStop()
    {
        BetterTello.Commands.Stop();
        BetterTello.Commands.Land();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            BetterTello.Commands.Land();
        else if (Input.GetKeyDown(KeyCode.T))
            Task.Factory.StartNew(async () => await Takeoff());
        else if (Input.GetKeyDown(KeyCode.P))
            Task.Factory.StartNew(async () => await PathFind());
        else if (Input.GetKeyDown(KeyCode.W))
            Task.Factory.StartNew(async () => await Up(50));
        UpdateTransform();
        if (BetterTello?.State != null)
            FlyingState = BetterTello.State.FlyingState;
    }
    private void FixedUpdate()
    {
        flightPathController.CreateFlightPoint();
        DistanceToTarget = ShowGoldenPath.GetDistanceToTargetTransform();
    }
    public async Task<int> Takeoff()
    {
        Debug.Log("Takeoff!!!");
        var r = await RunCommand(BetterTello.Commands.Takeoff);
        flightPathController.drawFlightPath = true;
        return r;
    }
    public async Task<int> Land()
    {
        Debug.Log("Land!!!");
        IsPathfinding = false;
        flightPathController.drawFlightPath = false;
        var r = await RunCommand(BetterTello.Commands.Land);
        return r;
    }
    public async Task<int> Up(int x) => await RunCommand(BetterTello.Commands.Up, x);
    public async Task<int> Cw(int x)
    {
        BetterTello.Factories.ExtTofDelay = 10;
        var res = await RunCommand(BetterTello.Commands.Cw, x);
        return res;
    }
    public async Task<int> Ccw(int x)
    {
        BetterTello.Factories.ExtTofDelay = 10;
        var res = await RunCommand(BetterTello.Commands.Ccw, x);
        return res;
    }
    public async Task<int> Forward(int x) => await RunCommand(BetterTello.Commands.Forward, Math.Clamp(x, 1, 70));
    public async Task<int> Back(int x) => await RunCommand(BetterTello.Commands.Back, x);
    public async Task PathFind()
    {
        IsPathfinding = true;
        Debug.Log("Started Pathfinding");
        while(IsPathfinding)
        {
            await RotateToTarget(Scan: true);
            await Step();
            if (DistanceToTarget < TargetTransformPrecision)
                IsPathfinding = false;
        }
        Debug.Log("Target Found");
    }

    public async Task Step()
    {
        if (DistanceToTarget > TargetTransformPrecision 
                && ShowGoldenPath.IsTargetReachable
                && ShowGoldenPath.status != UnityEngine.AI.NavMeshPathStatus.PathInvalid 
                && ExtTof > ExtTofDistance
        )
        {
            Debug.Log("Stepping");
            await Forward(Math.Clamp(ForwardDistance, 10, 70));
            await ScanXDegrees();
        }
    }

    private async Task RotateToTarget(bool Scan)
    {
        Debug.Log($"Rotating... Scan:{Scan}");
        int rot = (int)ShowGoldenPath.targetY;
        if (rot < 0)
            await Ccw(Math.Abs(rot));
        else
            await Cw(rot);
        if (Math.Abs(ShowGoldenPath.targetY) > DegreePrecision)
            await RotateToTarget(false);
        if (Scan)
            await ScanXDegrees();
    }
    private async Task ScanXDegrees()
    {
        Debug.Log("Scanning");
        await Ccw(ScanDegrees);
        await Cw(ScanDegrees * 2);
        await RotateToTarget(false);
    }

    public async Task<int> RunCommand(Func<int, int> Function, int x)
    {
        var ret = Function(x);
        await WaitForOk();
        return ret;
    }
    public async Task<int> RunCommand(Func<int> Function)
    {
        var ret = Function();
        await WaitForOk();
        return ret;
    }

    public async Task WaitForOk()
    {
        waitingForOk = true;
        while (waitingForOk)
            await Task.Delay(5);
        await Task.Delay(500);
    }

    public void OnStateUpdate(object? sender, StateEventArgs e)
    {
        var state = e.State;
        var vel = new Vector3(state.Vgx, state.Vgy, state.Vgz);
        PositionMissionPad = new Vector3(state.X, state.Y, -state.Z);
        Pitch = state.Pitch;
        Roll = state.Roll;
        Yaw = state.Yaw;
        PYR = Quaternion.Euler(Pitch, Yaw, Roll);
        ExtTof = state.ExtTof;
        Bat = state.Bat;
        Tof = state.Tof;
        Height = state.H;
        TempH = state.Temph;
        TempL = state.Templ;
        Timestamps.Add(state.Time);
        Vels.Add(vel);
        if (Timestamps.Count > 1 && Vels.Count > 1)
        {
            List<int> localtime = new();
            int prevtime = Timestamps.First();
            foreach (var item in Timestamps.ToArray()[1..^0])
                localtime.Add(Math.Abs(prevtime - item));
            List<Vector3> localvel = Vels.ToArray()[1..^0].ToList();
            for (int i = 0; i < localvel.Count; i++)
            {
                localvel[i] = new Vector3(localvel[i].x * localtime[i], localvel[i].y * localtime[i], localvel[i].z * localtime[i]);
            }
            PositionVel = new Vector3()
            {
                x = -(localvel.Select(p => p.y).Sum() / 100),
                y = 1, //localvel.Select(p => p.z).Sum() / 100,
                z = -(localvel.Select(p => p.x).Sum() / 100),
            };
        }
    }

    public void UpdateTransform()
    {
        if (ConnectionState == TelloConnectionState.Connected)
        {
            Transform.position = PositionVel;
            Transform.rotation = PYR;
        }
    }



    private void Tello_onVideoData(object? sender, VideoDataRecievedEventArgs data)
    {
        if (telloVideoTexture != null)
        {
            //telloVideoTexture.PutVideoData(data.Data);
            telloVideoTexture.UpdateStream(data.Data);
        }
    }
}
