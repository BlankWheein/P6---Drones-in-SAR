using BetterTelloLib.Commander;
using BetterTelloLib.Commander.Events.EventArgs;
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


    private void Start()
    {
        gameObject.SetActive(true);
        ConnectToTello();
    }
    public void ConnectToTello()
    {
        ConnectionState = TelloConnectionState.Connecting;
        BetterTello.Events.OnStateRecieved += OnStateUpdate;
        BetterTello.Events.OnVideoDataRecieved += Tello_onVideoData;
        BetterTello.Connect();
        ConnectionState = TelloConnectionState.Connected;
        BetterTello.Commands.SetBitrate(0);
        Task.Factory.StartNew(async ()=> await Run());
    }
    public void CustomOnApplicationQuit()
    {
        BetterTello.Events.OnStateRecieved -= OnStateUpdate;
        BetterTello.Events.OnVideoDataRecieved -= Tello_onVideoData;
        BetterTello.Dispose();
    }
    public void CustomAwake(SceneManager sceneManager, InputController inputController)
    {
        this.sceneManager = sceneManager;
        this.inputController = inputController;
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

    public async Task Run()
    {
        BetterTello.Commands.Takeoff();
        await Task.Delay(5000);
        BetterTello.Commands.Forward(100);
        await Task.Delay(1000);
        BetterTello.Commands.Forward(100);
        await Task.Delay(1000);
        BetterTello.Commands.Back(200);
        await Task.Delay(2000);
        BetterTello.Commands.Land();
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
        Debug.Log(state.ToString());
        Position += new Vector3(state.Vgx, state.Vgy, state.Vgz);
        Pitch = state.Pitch;
        Roll = state.Roll;
        Yaw = state.Yaw;
        PYR = Quaternion.Euler(Pitch, Yaw, Roll);
        ExtTof = state.ExtTof;
        Bat = state.Bat;
        UpdateTransform();
    }

    public void UpdateTransform()
    {
        transform.position.Set(Position.x, Position.y, Position.z);
        transform.rotation.Set(PYR.x, PYR.y, PYR.z, PYR.w);
    }



    private void Tello_onVideoData(object? sender, VideoDataRecievedEventArgs data)
    {
        if (telloVideoTexture != null)
            telloVideoTexture.PutVideoData(data.Data);
    }
}
