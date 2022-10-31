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
            string str = _serialPort.ReadLine();
            _data += str + " ";
            Message = str;
              
        }

        public void WriteToPort(string segment)
        {
            _serialPort.Open();
            Writing = true;
            if (_serialPort.IsOpen)
            {
                _serialPort.WriteLine("use flash\n");
                Thread.Sleep(1000);
                _serialPort.WriteLine(segment + "\n");
                Thread.Sleep(1000);
                while (_serialPort.IsOpen)
                {
                    _serialPort.WriteLine("d\n"); 
                    Thread.Sleep(500);
                    if (_message.Contains("FFF0:"))
                    {
                        break;
                    }
                }            
            }
            WriteToFile();
            _serialPort.Close();
        }

        public async Task ReadSegment(string segment) {
            Message = "Начало работы";
            await Task.Run(() => WriteToPort(segment));  
            Message = "Конец работы";
        }

        private void WriteToFile()
        {
            using (StreamWriter writer = new StreamWriter($"{FileName}.txt", false))
            {
                writer.WriteLineAsync(_data);
            }
            Writing = false;
        }

        public async void ClosePort()
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
