using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace ReadDataSegment
{
    internal class Data: INotifyPropertyChanged
    {
        private string _data = string.Empty;
        private string _message;
        private string _selectedSegment = string.Empty;
        private bool _writing;
        private SerialPort _serialPort = null;
        private string _fileName = "file";
        public event PropertyChangedEventHandler PropertyChanged;

        public Data(MySerialPort port)
        {
            _serialPort = new SerialPort(port.SelectedPort, port.Speed, Parity.None, 8, StopBits.One);
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
        }

        #region свойства
        public string SelectedSegment
        {
            get => _selectedSegment;
            set => _selectedSegment = value;
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public string FileName
        {
            get => _fileName;
            set => _fileName = value;
        }
        public bool Writing
        {
            get => _writing;
            set
            {
                _writing = value;
                OnPropertyChanged();
            }
        }

        public Visibility ProgressBarVisibility
        {
            get
            {
                return (Writing) ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        #endregion

        public void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Thread.Sleep(300);
            _data += _serialPort.ReadLine();
            Message = _serialPort.ReadLine();
              
        }

        public void WriteToPort()
        {
            _serialPort.Open();
            Writing = true;
            if (_serialPort.IsOpen)
            {
                _serialPort.Write("flash\n");
                _serialPort.Write(SelectedSegment + "\n");
                while (_serialPort.IsOpen)
                {
                    if (Message.Contains("FFF0"))
                    {
                        break;
                    }
                    _serialPort.Write("d\n");
                    Thread.Sleep(300);
                }
                
            }
            WriteToFile();
            _serialPort.Close();
        }

        private void WriteToFile()
        {
            using (StreamWriter writer = new StreamWriter($"{FileName}.txt", false))
            {
                writer.WriteLineAsync(_data);
            }
            Writing = false;
        }

        public void ClosePort()
        {
            _serialPort.Close();
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
