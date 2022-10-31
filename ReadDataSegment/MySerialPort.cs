using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;

namespace ReadDataSegment
{
    internal class MySerialPort
    {
        private string[] _portsName = null;
        private string _selectedPort = null;
        private int _speed = 115200;


        public MySerialPort()
        {
            PortsName = SerialPort.GetPortNames();
        }

        public string[] PortsName
        {
            get => _portsName;
            set => _portsName = value;
        }

        public string SelectedPort
        {
            get => _selectedPort;
            set => _selectedPort = value;
        }
        public int Speed
        {
            get => _speed;
            set => _speed = value;
        }
    }
}
