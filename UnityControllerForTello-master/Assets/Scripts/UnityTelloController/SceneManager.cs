using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BetterTelloLib.Commander;

namespace UnityControllerForTello
{
    public class SceneManager : SingletonMonoBehaviour<SceneManager>
    {
        /// <summary>
        /// Different Scene Modes
        /// </summary>
        public enum SceneType {
            /// <summary>
            /// Control the Tello
            /// </summary>
            FlyOnly, 
            /// <summary>
            /// Run the simulator
            /// </summary>
            SimOnly 
        }
        /// <summary>
        /// Is the scene configured to control the Tello, or the simulator
        /// </summary>
        public SceneType sceneType;

        public BetterTelloManager telloManager { get; private set; }
        public DroneSimulator simulator { get; private set; }

        [HideInInspector]
        public Transform activeDrone;

        private Camera display2Cam;


        private float timeSinceLastUpdate;
        private float prevDeltaTime = 0;
        private System.TimeSpan telloDeltaTime;
        private float telloFrameCount = 0;

        public Quaternion finalInputs { get; private set; }
        public float elv;
        public float yaw;
        public float pitch;
        public float roll;

        public InputController inputController { get; private set; }

        private void Pause()
        {
        }

        override protected void Awake()
        {
            base.Awake();
            telloManager = transform.Find("Tello Manager").GetComponent<BetterTelloManager>();

            if (telloManager == null)
                Debug.LogError("No Tello Manager Found");

            inputController = FindObjectOfType<InputController>();

            //so we can roll/pitch tello model without the camera moving on those axis
            var trackingCamObject = transform.Find("Tracking Camera (Display 2)");
            if (trackingCamObject)
                display2Cam = trackingCamObject.GetComponent<Camera>();
            if (sceneType != SceneType.SimOnly)
            {
                //telloManager.CustomAwake(this, inputController);
                //if (display2Cam)
                //    display2Cam.transform.SetParent(telloManager.transform);
            }
            else
                telloManager.gameObject.SetActive(false);

          
            if (!inputController)
                Debug.LogError("Missing an input controller");
            else
                inputController.CustomAwake(this);

            //Simulator
            simulator = FindObjectOfType<DroneSimulator>();
            if (!simulator)
                Debug.Log("No tello simulator found");
            if (sceneType == SceneType.FlyOnly)
            {
                if (simulator)
                    simulator.gameObject.SetActive(false);
                activeDrone = telloManager.gameObject.transform;
            }
            else if (sceneType == SceneType.SimOnly)
            {
                Debug.Log("Begin Sim");
                activeDrone = simulator.gameObject.transform;
                display2Cam.transform.SetParent(simulator.transform);
            }
        }

        private void Start()
        {
            inputController.CustomStart();
            if (sceneType != SceneType.SimOnly)
            {
                telloManager.ConnectToTello();
            }
            if (sceneType != SceneType.FlyOnly)
                simulator.CustomStart(this);
        }
        private void Update()
        {
            inputController.GetFlightCommmands();
        }

        public void EmergencyStop()
        {
            telloManager.EmergencyStop();
        }

        public void RunFrame()
        {
            telloFrameCount++;
            timeSinceLastUpdate = Time.time - prevDeltaTime;
            prevDeltaTime = Time.time;
            var deltaTime1 = (int)(timeSinceLastUpdate * 1000);
            telloDeltaTime = new System.TimeSpan(0, 0, 0, 0, (deltaTime1));

        }

        public void TakeOff()
        {
            switch (sceneType)
            {
                case SceneType.FlyOnly:
                    telloManager.TakeOff();
                    break;
                case SceneType.SimOnly:
                    simulator.TakeOff();
                    break;
                default:
                    break;
            }
        }
        public void Land()
        {
            telloManager.BetterTello.Commands.Land();
        }

        void OnApplicationQuit()
        {
            if (sceneType != SceneType.SimOnly)
            {
                telloManager.CustomOnApplicationQuit();
            }
        }
    }
}