using System.Net;
using System.Net.Sockets;
using System.Text;
var tello = new TelloLib.Tello();
TelloLib.Tello.startConnecting();
tello.SendToDrone("EXT tof?", true);
