using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ReadDataSegment
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private List<string> _segments = new List<string>() {"segment 8000","segment 9000", "segment A000", "segment B000", "segment C000", "segment D000"};
        private List<string> _comPorts = new List<string>();
        private MySerialPort _port = null;
        private string _selectedSegment = null;
        private Data _data = null;
        
        public MainWindow()
        {
            InitializeComponent();
            pbProgress.Visibility = Visibility.Collapsed;
            _port = new MySerialPort();
            _comPorts.AddRange(_port.PortsName);
            cbComPorts.ItemsSource = _comPorts;
            cbSegments.ItemsSource = _segments;
           
        }

        private void cbComPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _port.SelectedPort = (string)cbComPorts.SelectedValue;
        }

        private void cbSegments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedSegment = (string)cbSegments.SelectedValue;
        }

        private void tbFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _data.FileName = tbFileName.Text;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            bStart.IsEnabled = false;
            bStop.IsEnabled = true;
            _data = new Data(_port);
            pbProgress.Visibility = Visibility.Visible;
            await _data.ReadSegment(_selectedSegment);
        }

        private void  bStop_Click(object sender, RoutedEventArgs e)
        {
            _data.ClosePort();
            bStart.IsEnabled = true;
            bStop.IsEnabled = false;
        }
    }
}
