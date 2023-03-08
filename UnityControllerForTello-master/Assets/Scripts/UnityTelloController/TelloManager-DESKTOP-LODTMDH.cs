using System.Collections.Generic;
using UnityEngine;
using TelloLib;
using System.Collections;

namespace UnityControllerForTello
{
    /// <summary>
    /// Displays information about the quadcopter from <see cref="TelloLib"/> library
    /// Takes inputs to send to the quadcopter
    /// </summary>
    public class TelloManager : MonoBehaviour
    {
        public int batteryPercent;
        public float posX = 0, posY, posZ;
        public Vector3 Offset = Vector3.zero;
        public float yaw, pitch, roll;
        public int height;
        public float posUncertainty;
        public int EXTTof;
        public int wifiStrength;
        public string state;



        private bool limitPathDistance = false;
        private TelloVideoTexture telloVideoTexture;
        private float quatW;
        private float quatX;
        private float quatY;
        private float quatZ;
        private Vector3 toEuler;
        private Quaternion onTrackStartRot;
        private Vector3 originPoint, originEuler;
        private bool flying = false, hovering = false;
        private float prevPosX, prevPosY, prevPosZ;
        private Transform ground, telloGround, telloModel, flightPointsParent;
        private bool updateReceived = false;

        //Tello api
        private bool batteryLow;
        private int cameraState;
        private bool downVisualState;
        private int telloBatteryLeft;
        private int telloFlyTimeLeft;
        private int flymode;
        private int flyspeed;
        private int flyTime;
        private bool gravityState;
        private int imuCalibrationState;
        private bool imuState;
        private int lightStrength;
        private bool onGround = true;
        private bool powerState;
        private bool pressureState;
        private int temperatureHeight;
        private int wifiDisturb;
        private bool windState;

        private bool startingProps = false;
        private int startUpCount = 0, startUpLimit = 300;
        private SceneManager sceneManager;
        private InputController inputController;
        private FlightPathController flightPathController;
        private int telloFrameCount = 0;

        public void CustomAwake(SceneManager sceneManager, InputController inputController)
        {
            this.sceneManager = sceneManager;
            this.inputController = inputController;
            this.flightPathController = GetComponent<FlightPathController>();
            try
            {
                telloModel = transform.Find("Tello Model");
                ground = transform.Find("Ground");
                telloGround = transform.Find("Tello Ground");
                flightPointsParent = GameObject.Find("Track Points").transform;
            }
            catch
            {
                Debug.Log("Missing a gameObject");
            }

            if (telloVideoTexture == null)
                telloVideoTexture = FindObjectOfType<TelloVideoTexture>();
        }

        /// <summary>
        /// Attempt to connect to the quadcopter via <see cref="Tello"/>
        /// Must be connected to quadcopter via wifi
        /// </summary>
        public void ConnectToTello()
        {
            Tello.onConnection += Tello_onConnection;
            Tello.onUpdate += Tello_onUpdate;
            Tello.onVideoData += Tello_onVideoData;
            Tello.startConnecting();
        }

        /// <summary>
        /// Send command to quadcopter to take off and elevate to quads predetermined height
        /// Cannot control the drone durring until <see cref="SceneManager.FlightStatus.Flying"/>
        /// </summary>
        public void AutoTakeOff()
        {
            Debug.Log("TakeOff!");
            var preFlightPanel = GameObject.Find("Pre Flight Panel");
            if (preFlightPanel)
                preFlightPanel.SetActive(false);
            Tello.takeOff();
            sceneManager.flightStatus = SceneManager.FlightStatus.Launching;
            flightPathController.TakeOff(this);
        }
        public void SetOffset()
        {
            Offset = new Vector3(posX / 5, posY / 2, posZ / 5);
        }
        public void PrimeProps()
        {
            Debug.Log("Start Prop");
            sceneManager.flightStatus = SceneManager.FlightStatus.PrimingProps;
            Tello.SuspendControllerUpdate();
            int i = 0;
            do
            {
                i++;
                Tello.StartMotors();
            } while (i < 900);
            // OnFlyBegin();
            Tello.ResumeControllerUpdate();
            var preFlightPanel = GameObject.Find("Pre Flight Panel");
            if (preFlightPanel)
                preFlightPanel.SetActive(false);
            sceneManager.flightStatus = SceneManager.FlightStatus.Launching;
            Debug.Log("props started");
        }

        public void OnLand()
        {
            Debug.Log("Land");
            flightPathController.Land();
            Tello.land();
            Offset = Vector3.zero;
            sceneManager.flightStatus = SceneManager.FlightStatus.Landing;
        }

        public void CheckForUpdate()
        {
            if (updateReceived)
            {
                UpdateLocalState();
                sceneManager.RunFrame();
                SendTelloInputs();
                updateReceived = false;
                
            }
        }

        public void SendTelloInputs()
        {
            if (sceneManager.flightStatus != SceneManager.FlightStatus.PreLaunch)
            {
                Tello.controllerState.setAxis(sceneManager.yaw, sceneManager.elv, sceneManager.roll, sceneManager.pitch);
            }
        }

        Vector3 GetCurrentPos()
        {
            state = Tello.state.ToString();
            var telloPosX = posX - originPoint.x;
            var telloPosY = posY - originPoint.y;
            var telloPosZ = posZ - originPoint.z;

            return new Vector3(telloPosX, telloPosY, telloPosZ);     
        }

        public void CheckForLaunchComplete()
        {
            if (flymode == 6)
            {
                originPoint = GetCurrentPos();
                Debug.Log("Y Offset " + originPoint + " tello frame count " + telloFrameCount);
                originEuler = new Vector3(pitch, yaw, roll);
                // onTrackStartRot = new Quaternion(quatW, quatX, quatY, quatZ);
                ground.position -= new Vector3(0, height * .1f, 0);
                Debug.Log("tello height set to " + height * .1f);
                telloGround.position = transform.position - new Vector3(0, height * .1f, 0);
                elevationOffset = height * .1f;
                sceneManager.SetHomePoint(new Vector3(0, height * .1f, 0));
                sceneManager.flightStatus = SceneManager.FlightStatus.Flying;
            }
        }

        public bool SetTelloPosition()
        {
            validTrackingFrame = true;
            var currentPos = GetCurrentPos();
            Vector3 dif = currentPos - transform.position;
            //transform.position = currentPos - Offset;

            var xDif = dif.x;
            var yDif = dif.y;
            var zDif = dif.z;

            //valid tello frame
            if (Mathf.Abs(xDif) < 2000 & Mathf.Abs(yDif) < 2000 & Mathf.Abs(zDif) < 2000)
            {
                transform.position += new Vector3(0, elevationOffset, 0);
                prevDeltaPos = dif;
                yaw = yaw * (180 / Mathf.PI);
                transform.eulerAngles = new Vector3(0, yaw, 0);
                pitch = pitch * (180 / Mathf.PI);
                roll = roll * (180 / Mathf.PI);
                telloModel.localEulerAngles = new Vector3(pitch - 90, 0, roll);
            }
            else
            {
                Debug.Log("Tracking lost " + Mathf.Abs(xDif) + "/" + Mathf.Abs(yDif) + "/" + Mathf.Abs(zDif));

                validTrackingFrame = false;
            }
            return validTrackingFrame;
        }

        bool validTrackingFrame;
        Vector3 prevDeltaPos;

        float elevationOffset;

        /// <summary>
        /// Create a point for tracking
        /// </summary>
        
        private void Tello_onConnection(Tello.ConnectionState newState)
        {
            if (newState == Tello.ConnectionState.Connected)
            {
                Debug.Log("Connected to Tello, please wait for camera feed");
                Tello.setPicVidMode(1); // 0: picture, 1: video
                Tello.setVideoBitRate((int)TelloController.VideoBitRate.VideoBitRateAuto);
                Tello.requestIframe();
            }
        }
        //Dealing with telloLib
        private void Tello_onUpdate(int cmdID)
        {
            updateReceived = true;
        }
        //This just saves all the tello variables locally for viewing in the inspector
        public void UpdateLocalState()
        {
            var state = Tello.state;

            posX = Tello.state.posY;
            posY = -Tello.state.posZ;
            posZ = Tello.state.posX;

            transform.position = new Vector3((posX / 5) - Offset.x, (posY / 2) - Offset.y, (posZ / 5) - Offset.z);

            quatW = state.quatW;
            quatX = state.quatW;
            quatY = state.quatW;
            quatZ = state.quatW;

            var eulerInfo = state.toEuler();

            pitch = (float)eulerInfo[0];
            roll = (float)eulerInfo[1];
            yaw = (float)eulerInfo[2];

            toEuler = new Vector3(pitch, roll, yaw);

            posUncertainty = state.posUncertainty;
            batteryLow = state.batteryLow;
            batteryPercent = state.batteryPercentage;
            cameraState = state.cameraState;
            downVisualState = state.downVisualState;
            telloBatteryLeft = state.droneBatteryLeft;
            telloFlyTimeLeft = state.droneFlyTimeLeft;
            flymode = state.flyMode;
            flyspeed = state.flySpeed;
            flyTime = state.flyTime;
            gravityState = state.gravityState;
            height = state.height;
            imuCalibrationState = state.imuCalibrationState;
            imuState = state.imuState;
            lightStrength = state.lightStrength;
            onGround = state.onGround;
            powerState = state.powerState;
            pressureState = state.pressureState;
            temperatureHeight = state.temperatureHeight;
            wifiDisturb = state.wifiDisturb;
            wifiStrength = state.wifiStrength;
            windState = state.windState;
        }
        public void CustomOnApplicationQuit()
        {
            Tello.onConnection -= Tello_onConnection;
            Tello.onUpdate -= Tello_onUpdate;
            Tello.onVideoData -= Tello_onVideoData;
            Tello.stopConnecting();
        }
        private void Tello_onVideoData(byte[] data)
        {
            if (telloVideoTexture != null)
                telloVideoTexture.PutVideoData(data);
        }
    }
}