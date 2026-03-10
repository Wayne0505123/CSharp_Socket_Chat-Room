using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;

namespace ChatServer
{
    public partial class MainWindow : Window
    {
        private TcpListener listener;
        private Thread listenerThread;
        private bool isRunning = false;

        private Dictionary<TcpClient, string> clients = new Dictionary<TcpClient, string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ipString = txtServerIP.Text.Trim();
                string portString = txtServerPort.Text.Trim();

                IPAddress ip = IPAddress.Parse(ipString);
                int port = int.Parse(portString);

                listener = new TcpListener(ip, port);
                listener.Start();
                isRunning = true;

                listenerThread = new Thread(ListenForClients);
                listenerThread.Start();

                txtStatus.Text = $"Server started at {ip}:{port}";
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("無法啟動 Server，請確認 IP 與 Port 是否正確。\n\n錯誤訊息：" + ex.Message);
            }
        }


        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            isRunning = false;
            listener.Stop();

            foreach (var client in clients.Keys)
                client.Close();

            clients.Clear();
            listClients.Items.Clear();

            txtStatus.Text = "Server stopped";
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        private void ListenForClients()
        {
            while (isRunning)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Thread clientThread = new Thread(HandleClient);
                    clientThread.Start(client);
                }
                catch { break; }
            }
        }

        public class ChatMessage
        {
            public string Username { get; set; }
            public string Message { get; set; }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();

            string clientEndPoint = client.Client.RemoteEndPoint.ToString();
            string username = "Unknown";

            try
            {
                byte[] buffer = new byte[1024];
                int byteCount = stream.Read(buffer, 0, buffer.Length);
                string json = Encoding.UTF8.GetString(buffer, 0, byteCount);
                ChatMessage firstMsg = JsonConvert.DeserializeObject<ChatMessage>(json);
                username = firstMsg.Username;

                // 儲存 client
                clients.Add(client, username);

                // 更新 UI
                Dispatcher.Invoke(() =>
                {
                    listClients.Items.Add($"{clientEndPoint} | {username}");
                    ShowMessage($"{username} joined the chat");
                });

                // 廣播加入訊息
                Broadcast(new ChatMessage
                {
                    Username = "Server",
                    Message = $"{username} joined the chat"
                });

                // 開始接收訊息
                while (isRunning && client.Connected)
                {
                    byteCount = stream.Read(buffer, 0, buffer.Length);
                    if (byteCount == 0) break;

                    string jsonMsg = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    ChatMessage msg = JsonConvert.DeserializeObject<ChatMessage>(jsonMsg);

                    ShowMessage($"{msg.Username}: {msg.Message}");

                    // 廣播這則訊息
                    Broadcast(msg);
                }
            }
            catch
            {
                // 可能 client 關閉或中斷
            }
            finally
            {
                if (clients.ContainsKey(client))
                {
                    string leftUsername = clients[client];

                    Dispatcher.Invoke(() =>
                    {
                        listClients.Items.Remove($"{clientEndPoint} | {leftUsername}");
                        ShowMessage($"{leftUsername} left the chat");
                    });

                    Broadcast(new ChatMessage
                    {
                        Username = "Server",
                        Message = $"{leftUsername} left the chat"
                    });

                    clients.Remove(client);
                    client.Close();
                }
            }
        }

        private void Broadcast(ChatMessage msg)
        {
            string json = JsonConvert.SerializeObject(msg);
            byte[] data = Encoding.UTF8.GetBytes(json);

            foreach (var client in clients.Keys)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(data, 0, data.Length);
                }
                catch
                {
                    // 忽略錯誤的 client
                }
            }
        }

        private void ShowMessage(string text)
        {
            Dispatcher.Invoke(() =>
            {
                txtMessages.AppendText(text + Environment.NewLine);
                txtMessages.ScrollToEnd();
            });
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string text = txtBroadcast.Text.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                ChatMessage msg = new ChatMessage
                {
                    Username = "Server",
                    Message = text
                };
                ShowMessage($"Server: {text}");
                Broadcast(msg);
                txtBroadcast.Clear();
            }
        }
    }
}



