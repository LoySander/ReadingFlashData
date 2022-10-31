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
        CancellationTokenSource cancelTokenSource;
        private Visibility _visibility;




        public Data()
        {
           // _serialPort = new SerialPort(port.SelectedPort, port.Speed, Parity.None, 8, StopBits.One);
            _serialPort = new SerialPort();
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
               // OnPropertyChanged(nameof(ProgressBarVisibility));
            }
        }

        //public Visibility ProgressBarVisibility
        //{
        //    get
        //    {
        //        return (Writing) ? Visibility.Visible : Visibility.Hidden;
        //    }

        //}
        public Visibility ProgressBarVisibility {
            get => _visibility;
            set {
                _visibility = value;
                OnPropertyChanged();
            }
        }


        #endregion
        
        public void ChangeSettings(MySerialPort port) {
            _serialPort.PortName = port.SelectedPort;
            _serialPort.BaudRate = port.Speed;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
        }

        public void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string str = _serialPort.ReadLine();
            _data += str + " ";
            Message = _data;   
        }

        //public void WriteToPort(string segment)
        //{
        //    _serialPort.Open();
        //    Writing = true;
        //    if (_serialPort.IsOpen)
        //    {
        //        _serialPort.WriteLine("use flash\n");
        //        Thread.Sleep(1000);
        //        _serialPort.WriteLine(segment + "\n");
        //        Thread.Sleep(1000);
        //        while (_serialPort.IsOpen)
        //        {
        //            _serialPort.WriteLine("d\n"); 
        //            Thread.Sleep(1000);
        //            if (_message.Contains("FFF0:"))
        //            {
        //                break;
        //            }
        //        }            
        //    }
        //    _serialPort.Close();
        //}

        public async Task ReadSegment(string segment) {
            cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
            //Message = "Начало работы";
            //await Task.Run(() => WriteToPort(segment));
            Task task = new Task(() => {
                _serialPort.Open();
                Writing = true;
                if (_serialPort.IsOpen) {
                    _serialPort.WriteLine("use flash\n");
                    Thread.Sleep(1000);
                    _serialPort.WriteLine(segment + "\n");
                    Thread.Sleep(1000);
                    while (!token.IsCancellationRequested) {
                        _serialPort.WriteLine("d\n");
                        Thread.Sleep(1000);
                        if (_message.Contains("FFF0:")) {
                            WriteToFile();
                            Message = "Конец работы";
                            _serialPort.WriteLine("exit\n");
                            break;
                        }
                    }
                    _serialPort.WriteLine("exit\n");
                }
               // _serialPort.Close();
            }, token );
            task.Start();
            //WriteToFile();
            //Message = "Конец работы";
        }

        private void WriteToFile()
        {
            using (StreamWriter writer = new StreamWriter($"{FileName}.txt", false))
            {
                writer.WriteLine(_data);
            }
            Writing = false;
            ProgressBarVisibility = Visibility.Hidden;
        }

        public void ClosePort()
        {
            cancelTokenSource.Cancel();
            _serialPort.Close();
            cancelTokenSource.Dispose();
            WriteToFile();
            ProgressBarVisibility = Visibility.Collapsed;
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
