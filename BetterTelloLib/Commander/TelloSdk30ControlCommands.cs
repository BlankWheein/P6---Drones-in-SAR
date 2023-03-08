using BetterTelloLib.Udp;
using Microsoft.VisualBasic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Threading.Channels;
using static System.Net.Mime.MediaTypeNames;

namespace BetterTelloLib.Commander
{
    public class TelloSdk30ControlCommands
    {
        private readonly TelloUdpClient _client;
        private readonly BetterTello bt;

        public TelloSdk30ControlCommands(TelloUdpClient client, BetterTello bt)
        {
            _client = client;
            this.bt = bt;
        }

        /// <summary>
        /// Enter SDK command mode.
        /// </summary>
        /// <returns></returns>
        public int Command()
        { 
            return _client.Send("command");
        }

        /// <summary>
        /// Auto take off
        /// </summary>
        /// <returns></returns>
        public int Takeoff()
        {
            return _client.Send("takeoff");
        }

        /// <summary>
        /// Auto landing
        /// </summary>
        /// <returns></returns>
        public int Land()
        {
            return _client.Send("land");
        }

        /// <summary>
        /// Turn on the video stream.
        /// </summary>
        /// <returns></returns>
        public int Streamon()
        {
            return _client.Send("streamon");
        }

        /// <summary>
        /// Turn off the video stream
        /// </summary>
        /// <returns></returns>
        public int Streamoff()
        {
            return _client.Send("streamoff");
        }

        /// <summary>
        /// Stop the motor from running.
        /// </summary>
        /// <returns></returns>
        public int Emergency()
        {
            return _client.Send("emergency");
        }

        /// <summary>
        /// Fly upward by x cm. x = 20-500
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int Up(int x)
        {
            return _client.Send($"up {x}");
        }

        /// <summary>
        /// Fly downward by x cm. x = 20-500
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int Dowm(int x)
        {
            return _client.Send($"down {x}");
        }

        /// <summary>
        /// Fly leftward by x cm. x = 20-500
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int Left(int x)
        {
            return _client.Send($"left {x}");
        }

        /// <summary>
        /// Fly rightward by x cm. x = 20-500
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int Right(int x)
        {
            return _client.Send($"right {x}");
        }

        /// <summary>
        /// Fly forward by x cm. x = 20-500
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int Forward(int x)
        {
            if (x > 70) { x = 70; }
            if (!bt.State.ObstacleTooCloseInFront)
                return _client.Send($"forward {x}");
            return -1;
        }

        /// <summary>
        /// Fly backward by x cm. x = 20-500
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int Back(int x)
        {
            return _client.Send($"back {x}");
        }

        /// <summary>
        /// Rotate clockwise by x°. x = 1-360
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int Cw(int x)
        {
            return _client.Send($"cw {x}");
        }

        /// <summary>
        /// Rotate counterclockwise by x°. x = 1-360
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int Ccw(int x)
        {
            return _client.Send($"ccw {x}");
        }

        /// <summary>
        /// Enter Motor-On mode
        /// </summary>
        /// <returns></returns>
        public int MotorOn()
        {
            return _client.Send("motoron");
        }

        /// <summary>
        /// Exit Motor-On mode.
        /// </summary>
        /// <returns></returns>
        public int MotorOff()
        {
            return _client.Send("motoroff");
        }

        /// <summary>
        /// Throw to launch. Throw the drone horizontally within 5s of sending the command.
        /// </summary>
        /// <returns></returns>
        public int ThrowFly()
        {
            return _client.Send("throwfly");
        }

        /// <summary>
        /// Roll in the x direction. l = (left), r = (right), f = (forward), b = (back).
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int Flip(string x)
        {
            return _client.Send($"throwfly {x}");
        }

        /// <summary>
        /// Fly to the coordinates (x,y,z) at the set speed (cm/s).
        /// x: -500 - 500
        /// y: -500 - 500
        /// z: -500 - 500
        /// speed: 10-100 (cm/s)
        /// x, y, and z cannot be between -20 and 20 at the same
        /// time
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public int GoSpeed(int x, int y, int z, int speed)
        {
            return _client.Send($"go {x} {y} {z} {speed}");
        }

        /// <summary>
        /// Stop moving and hover immediately.
        /// </summary>
        /// <returns></returns>
        public int Stop()
        {
            return _client.Send("stop");
        }

        /// <summary>
        /// Fly to the coordinate point (x, y, z) in the coordinate
        /// system of the mission pad with the specified ID at the
        /// set speed(m/s)(* Note 2).
        /// x: -500 - 500
        /// y: -500 - 500
        /// z: 0 - 500
        /// speed: 10-100 (cm/s)
        /// x, y, and z cannot be between -20 and 20 at the same
        /// time
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="speed"></param>
        /// <param name="mid"></param>
        /// <returns></returns>
        public int GoMid(int x, int y, int z, int speed, int mid)
        {
            return _client.Send($"curve {x} {y} {z} {speed} {mid}");
        }

        /// <summary>
        /// Fly in a curve from point (x1,y1,z1) to point (x2,y2,z2)
        /// in the coordinate system of the mission pad with the
        /// set mid at the set speed(cm/s).
        /// If the radius of the curve is not within 0.5-10 meters,
        /// the corresponding reminder will be returned.
        /// x1, x2: -500 - 500
        /// y1, y2: -500 - 500
        /// z1, z2: 0 - 500
        /// speed: 10-60
        /// x, y, and z cannot be between -20 and 20 at the same
        /// time
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="z1"></param>
        /// <param name="z2"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public int Curve(int x1, int x2, int y1, int y2, int z1, int z2, int speed, int mid)
        {
            return _client.Send($"curve {x1} {y1} {z1} {x2} {y2} {z2} {speed} {mid}");
        }

        /// <summary>
        /// Tello flies to the point (x,y,z) in the mid1 coordinate
        /// system and hovers.Then, it identifies the mission pad
        /// of mid2 and rotates to the position(0,0, z) in the mid2
        /// coordinate system to set the yaw value(z>0).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="speed"></param>
        /// <param name="yaw"></param>
        /// <param name="mid1"></param>
        /// <param name="mid2"></param>
        /// <returns></returns>
        public int Jump(int x, int y, int z, int speed, int yaw, int mid1, int mid2)
        {
            return _client.Send($"jump {x} {y} {z} {speed} {yaw} {mid1} {mid2}");
        }

        /// <summary>
        /// Reboot the drone.
        /// </summary>
        /// <returns></returns>
        public int Reboot()
        {
            return _client.Send("reboot");
        }

        /// <summary>
        /// Set the current speed to x cm/s.
        /// x = 10-100
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int SetSpeed(int x)
        {
            return _client.Send($"speed {x}");
        }

        /// <summary>
        /// Set the lever force values for the four channels of the
        /// remote control.
        /// a: roll (-100 to 100)
        /// b: pitch(-100 to 100)
        /// c: throttle(-100 to 100)
        /// d: yaw(-100 to 100)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public int SetRc(int a, int b, int c, int d)
        {
            return _client.Send($"rc {a} {b} {c} {d}");
        }

        /// <summary>
        /// Change the Tello Wi-Fi password.
        /// ssid: The new Wi-Fi account
        /// pass: The new Wi-Fi password
        /// If an open-source controller is connected, ssid adds
        /// the RMTT- prefix by default. Otherwise, it adds the
        /// TELLO- prefix
        /// </summary>
        /// <param name="ssid"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public int WifiChange(string ssid, string pass)
        {
            return _client.Send($"wifi {ssid} {pass}");
        }

        /// <summary>
        /// Enables mission pad.
        /// By default, downward detection is enabled.
        /// </summary>
        /// <returns></returns>
        public int Mon()
        {
            return _client.Send("mon");
        }

        /// <summary>
        /// Disables mission pad detection.
        /// </summary>
        /// <returns></returns>
        public int Moff()
        {
            return _client.Send("moff");
        }

        /// <summary>
        /// X=0/1/2
        /// 0: downward detection enabled.
        /// 1: forward detection enabled.
        /// 2: both forward and downward detection enabled.
        /// *Before use, you must use the mon command to
        /// enable the detection function.Downward detection is
        /// enabled by default.
        /// *When either forward-looking or downward-looking
        /// detection is enabled alone, the detection frequency
        /// is 20Hz.If both enabled, detection will be performed
        /// alternatively, with a frequency of 10Hz in each
        /// direction
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int SetMdirection(int x)
        {
            return _client.Send($"mdirection {x}");
        }

        /// <summary>
        /// Switch Tello to Station mode and connect to the AP.
        /// ssid: the Wi-Fi account to connect to
        /// pass: the Wi-Fi password
        /// </summary>
        /// <param name="ssid"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public int SetAp(string ssid, string pass)
        {
            return _client.Send($"ap {ssid} {pass}");
        }

        /// <summary>
        /// Set the -WiFi channel of the open-source controller.
        /// xxx indicates the channel to be set.Note: To clear
        /// the channel settings, you need to clear the Wi-Fi
        /// information.Then, set a channel that complies with
        /// local policies and regulations.
        /// (Only applies to the open-source controller)
        /// </summary>
        /// <param name="xxx"></param>
        /// <returns></returns>
        public int WifiSetChannel(string xxx)
        {
            return _client.Send($"wifisetchannel {xxx}");
        }

        /// <summary>
        /// Set the ports for pushing status information and video
        /// streams.Here, info is the port for pushing status
        /// information, and vedio is the port for pushing video
        /// information.The range of ports is 1025 to 65535.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Port()
        {
            throw new NotImplementedException("do not need this command");
        }

        /// <summary>
        /// Set the video stream frame rate. The fps parameter
        /// specifies the frame rate, whose value can be "high",
        /// "middle", or "low", indicating 30fps, 15fps, and 5fps,
        /// respectively.
        /// </summary>
        /// <param name="fps"></param>
        /// <returns></returns>
        public int SetFps(string fps)
        {
            return _client.Send($"setfps {fps}");
        }

        /// <summary>
        /// Set the video stream bit rate. The bitrate parameter
        /// specifies the bit rate, with a value range is 0 to 5,
        /// indicating auto, 1Mbps, 2MBps, 3Mbps, 4Mbps, and
        /// 5Mbps, respectively.
        /// </summary>
        /// <param name="bitrate"></param>
        /// <returns></returns>
        public int SetBitrate(int bitrate)
        {
            return _client.Send($"setbitrate {bitrate}");
        }

        /// <summary>
        /// Set the video stream resolution. The resolution
        /// parameter specifies the resolution, whose value
        /// can be "high" or "low", indicating 720P and 480P,
        /// respectively.
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public int SetResolution(string resolution)
        {
            return _client.Send($"setresolution {resolution}");
        }

        /// <summary>
        /// Get the current set speed (cm/s).
        /// </summary>
        /// <returns></returns>
        public int GetSpeed()
        {
            return _client.Send("speed?");
        }

        /// <summary>
        /// Get the percentage (%) indicating the current battery
        /// level.
        /// </summary>
        /// <returns></returns>
        public int GetBattery()
        {
            return _client.Send("battery?");
        }

        /// <summary>
        /// Get the motor running time (s)
        /// </summary>
        /// <returns></returns>
        public int GetTime()
        {
            return _client.Send("time?");
        }

        /// <summary>
        /// Get the Wi-Fi SNR
        /// </summary>
        /// <returns></returns>
        public int GetWifi()
        {
            return _client.Send("wifi?");
        }

        /// <summary>
        /// Get the Tello SDK version number.
        /// </summary>
        /// <returns></returns>
        public int GetSdk()
        {
            return _client.Send("sdk?");
        }

        /// <summary>
        /// Get the Tello SN.
        /// </summary>
        /// <returns></returns>
        public int GetSn()
        {
            return _client.Send("sn?");
        }

        /// <summary>
        /// Get whether TT is connected to an open-source
        /// controller.If yes, it returns RMTT; if not, it returns
        /// TELLO.
        /// </summary>
        /// <returns></returns>
        public int GetHardware()
        {
            return _client.Send("hardware?");
        }

        /// <summary>
        /// Query the -WiFi version of the open-source controller.
        /// (Only applies to the open-source controller)
        /// </summary>
        /// <returns></returns>
        public int GetWifiversion()
        {
            return _client.Send("wifiversion?");
        }

        /// <summary>
        /// Get the name and password of the current router
        /// to be connected. (Only applies to the open-source
        /// controller)
        /// </summary>
        /// <returns></returns>
        public int GetAp()
        {
            return _client.Send("ap?");
        }

        /// <summary>
        /// Get the current SSID of the drone. (Only applies to the
        /// open-source controller)
        /// </summary>
        /// <returns></returns>
        public int GetSsid()
        {
            return _client.Send("ssid?");
        }

        /// <summary>
        /// Set the SSID and password of the open-source
        /// controller.This feature supports connection to multiple
        /// devices as a router.
        /// </summary>
        /// <param name="ssid"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public int SetMultiwif(string ssid, string pass)
        {
            return _client.Send($"multiwifi {ssid} {pass}");
        }

        /// <summary>
        /// Light up the top LED in the specified color. The r,
        /// g, and b variables indicate the red, green, and blue
        /// channels, respectively.
        /// r: 0-255
        /// g: 0-255
        /// b: 0-255
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int EXTLed(int r, int g, int b)
        {
            return _client.Send($"EXT led {r} {g} {b}");
        }

        /// <summary>
        /// The top LED displays the pulse effect according to the
        /// max pulse brightness(r, g, b) and pulse frequency t.
        /// The cycle from dimmest to brightest to dimmest again
        /// is counted as one pulse.
        /// r, g, b: 0~255
        /// t: 0.1-2.5Hz
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int EXTLedBr(float t, int r, int g, int b)
        {
            return _client.Send($"EXT led br {t} {r} {g} {b}");
        }

        /// <summary>
        /// The top LED flashes alternately between color 1 (r1,
        /// g1, b1) and color(r2, g2, b2) according to the flash
        /// frequency t.
        /// a1 b1 c1 a2 b2 c2: 0~255
        /// t: 0.1-10Hz
        /// </summary>
        /// <param name="t"></param>
        /// <param name="a1"></param>
        /// <param name="b1"></param>
        /// <param name="c1"></param>
        /// <param name="a2"></param>
        /// <param name="b2"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public int EXTLedBl(float t, int a1, int b1, int c1, int a2, int b2, int c2)
        {
            return _client.Send($"EXT led bl {t} {a1} {b1} {c1} {a2} {b2} {c2}");
        }

        /// <summary>
        /// Light up the dot-matrix display with the specified
        /// pattern. 
        /// xxxx indicates a string consisting only of 'r', 'b', 'p', and '0'. 'r', 'b', 'p', and '0' indicate red, blue,
        /// purple, and off, respectively. The max string length is 64.
        /// </summary>
        /// <param name="xxxx"></param>
        /// <returns></returns>
        public int EXTMled(string xxxx)
        {
            return _client.Send($"EXT mled g {xxxx}");
        }

        /// <summary>
        /// The dot-matrix display indicates the direction of
        /// movement as a string.
        /// l/r/u/d (movement) indicates left/right/up/down movement.
        /// r/b/p (color) indicates the display color the string.
        /// t: 0.1-2.5Hz, indicating the frame rate of the image.
        /// xxxx indicates the string to be displayed, which
        /// cannot exceed 70 characters.
        /// </summary>
        /// <param name="movemonet"></param>
        /// <param name="color"></param>
        /// <param name="t"></param>
        /// <param name="xxxx"></param>
        /// <returns></returns>
        public int EXTMledString(string movemonet, string color, float t, string xxxx)
        {
            return _client.Send($"EXT mled {movemonet} {color} {t} {xxxx}");
        }

        /// <summary>
        /// The dot-matrix display indicates the direction of
        /// movement as an image.
        /// l/r/u/d indicates left/right/up/down movement.
        /// r/b/p (g) indicates the display color the string.
        /// t: 0.1-2.5Hz, indicating the frame rate of the image.
        /// </summary>
        /// <param name="movemonet"></param>
        /// <param name="g"></param>
        /// <param name="t"></param>
        /// <param name="xxxx"></param>
        /// <returns></returns>
        public int EXTMledImage(string movemonet, string g, float t, string xxxx)
        {
            return _client.Send($"EXT mled {movemonet} {g} {t} {xxxx}");
        }

        /// <summary>
        /// Display static ASCII character or preset pattern.
        /// r/b/p (color) indicates the display color of the string.
        /// xxxxx can only be "heart" or an ASCII character.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="xxxx"></param>
        /// <returns></returns>
        public int EXTMledASCII(string color, string xxxx)
        {
            return _client.Send($"EXT mled s {color} {xxxx}");
        }

        /// <summary>
        /// Set the dot-matrix display boot animation. The pattern
        /// set will be displayed after every boot.
        /// </summary>
        /// <param name="xxxx"></param>
        /// <returns></returns>
        public int EXTMledAnimation(string xxxx)
        {
            return _client.Send($"EXT mled sg {xxxx}");
        }

        /// <summary>
        /// Clear the set boot animation.
        /// </summary>
        /// <returns></returns>
        public int EXTMledClear()
        {
            return _client.Send($"EXT mled sc");
        }

        /// <summary>
        /// Set the dot-matrix display brightness.
        /// n: 0~255
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int EXTMledBrightness(int n)
        {
            return _client.Send($"EXT mled sl {n}");
        }

        /// <summary>
        /// Read the tof value
        /// </summary>
        /// <returns></returns>
        public int EXTTof()
        {
            return _client.Send($"EXT tof?");
        }

        /// <summary>
        /// Read the firmware version of the open-source
        /// controller ESP32.
        /// </summary>
        /// <returns></returns>
        public int EXTVersion()
        {
            return _client.Send($"EXT version?");
        }
    }
}