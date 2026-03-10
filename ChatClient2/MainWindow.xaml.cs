using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;

namespace ChatClient
{
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;
        private bool isConnected = false;

        private string username = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ip = txtIP.Text.Trim();
                int port = int.Parse(txtPort.Text.Trim());
                username = txtUsername.Text.Trim();

                client = new TcpClient();
                client.Connect(ip, port);
                stream = client.GetStream();

                // 傳送第一次訊息（包含 username）
                var joinMsg = new ChatMessage { Username = username, Message = $"{username} joined" };
                string json = JsonConvert.SerializeObject(joinMsg);
                byte[] data = Encoding.UTF8.GetBytes(json);
                stream.Write(data, 0, data.Length);

                // 啟動接收訊息的 thread
                isConnected = true;
                receiveThread = new Thread(ReceiveMessages);
                receiveThread.Start();

                txtStatus.Text = "Connected to server";
                btnConnect.IsEnabled = false;
                btnDisconnect.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("連線失敗：" + ex.Message);
            }
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            isConnected = false;
            stream?.Close();
            client?.Close();

            txtStatus.Text = "Disconnected";
            btnConnect.IsEnabled = true;
            btnDisconnect.IsEnabled = false;
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];

            while (isConnected)
            {
                try
                {
                    int byteCount = stream.Read(buffer, 0, buffer.Length);
                    if (byteCount == 0) break;

                    string json = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    ChatMessage msg = JsonConvert.DeserializeObject<ChatMessage>(json);

                    Dispatcher.Invoke(() =>
                    {
                        txtMessages.AppendText($"{msg.Username}: {msg.Message}\n");
                        txtMessages.ScrollToEnd();
                    });
                }
                catch
                {
                    break;
                }
            }

            Dispatcher.Invoke(() =>
            {
                txtMessages.AppendText("🔌 與 Server 斷線\n");
                txtMessages.ScrollToEnd();
                txtStatus.Text = "Disconnected";
                btnConnect.IsEnabled = true;
                btnDisconnect.IsEnabled = false;
            });
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string text = txtInput.Text.Trim();
            if (!string.IsNullOrEmpty(text) && isConnected)
            {
                var msg = new ChatMessage { Username = username, Message = text };
                string json = JsonConvert.SerializeObject(msg);
                byte[] data = Encoding.UTF8.GetBytes(json);
                stream.Write(data, 0, data.Length);
                txtInput.Clear();
            }
        }
    }

    public class ChatMessage
    {
        public string Username { get; set; }
        public string Message { get; set; }
    }
}
