using TelloSharp;

var drone = new Tello();
drone.Connect();
drone.Takeoff();
Thread.Sleep(5000);
drone.Land();
