using BetterTelloLib.Commander;
using BetterTelloLib.Commander.Events.EventArgs;
using System.Collections;
using System.Collections.Generic;
using UnityControllerForTello;
using UnityEngine;
using UnityEngine.UIElements;

public class BetterTelloManager : MonoBehaviour
{
    public BetterTello BetterTello = new();

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

    public void ConnectToTello()
    {
        BetterTello.Events.OnStateRecieved += OnStateUpdate;
        BetterTello.Events.OnVideoDataRecieved += Tello_onVideoData;
        BetterTello.Connect();
        BetterTello.Commands.SetBitrate(0);
        Run();
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

    public void Run()
    {
        
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
    }

    public void UpdateTransform()
    {
        transform.position = Position;
        transform.rotation = PYR;
    }



    private void Tello_onVideoData(object? sender, VideoDataRecievedEventArgs data)
    {
        if (telloVideoTexture != null)
            telloVideoTexture.PutVideoData(data.Data);
    }
}
