using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ChatClient.ServiceChat;
using Microsoft.Win32;
using NLog;
using WCFChat;

namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IServiceChatCallback
    {
        private bool isConnected = false;
        private ServiceChatClient client;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private int ID;
        public MainWindow()
        {
            InitializeComponent();
            BttnSendFile.IsEnabled = false;
            DownloadBttn.IsEnabled = false;
        }

        void ConnectUser()
        {
            if (!isConnected)
            {
                client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                ID = client.Connect(tbUserName.Text);
                if (ID != 0)
                {
                    client.ShowOnline();
                    tbUserName.IsEnabled = false;
                    BttnSendFile.IsEnabled = true;
                    DownloadBttn.IsEnabled = true;
                    bttnConnect.Content = "Disconnect";
                    isConnected = true;
                }
                else
                {
                    MessageBox.Show("Никнейм занят!");
                }
            }
        }

        public void ShowOnlineCallback(string[] users)
        {
            lbOnline.Items.Clear();
            for (int i = 0; i < users.Count(); i++)
            {
                lbOnline.Items.Add(users[i]);
            }
        }

        void DisconnectUser()
        {
            if (isConnected)
            {
                client.Disconnect(ID);
                client.ShowOnline();
                client = null;
                tbUserName.IsEnabled = true;
                bttnConnect.Content = "Connect";
                isConnected = false;
                BttnSendFile.IsEnabled = false;
                DownloadBttn.IsEnabled = false;
            }

        }

        private void ButtonConnect(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                DisconnectUser();
            }
            else
            {
                ConnectUser();
            }

        }

        public void MsgCallback(string msg)
        {
            lbChat.Items.Add(msg);
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (client != null)
                {
                    client.SendMsg(tbMessage.Text, ID);
                    tbMessage.Text = string.Empty;
                }
            }
        }

        private void BttnSendFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                var FileStream = File.Open(openFile.FileName, FileMode.Open, FileAccess.Read);
                byte[] file = new byte[FileStream.Length];
                for (int i = 0; i < FileStream.Length; i++)
                {
                    file[i] = (byte)FileStream.ReadByte();
                    string s = Encoding.Default.GetString(file);
                }
                client.AddFile(file, Path.GetFileName(openFile.FileName));
                FileStream.Close();
            }
        }

        public void ShowFilesCallback(string filepath)
        {
            FilesToDownload.Items.Add(filepath);
        }

        private void DownloadBttn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            if (saveFile.ShowDialog() == true)
            {
                client.DownloadFile(ID, saveFile.FileName);
            }
        }

        public void DownloadFileCallback(byte[] file, string path)
        {
            var filestream = File.Open(path, FileMode.Create, FileAccess.Write);
            filestream.Write(file, 0, file.Length);
            filestream.Close();
            MessageBox.Show("Файл успешно загружен!");
        }
    }
}
