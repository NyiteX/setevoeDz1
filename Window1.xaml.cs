using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Security.Policy;

namespace setevoeDz1
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        int port = 5002;
        TcpListener server = null;
        public Window1()
        {
            InitializeComponent();

            Task.Factory.StartNew(() =>
            {
                Ini2();
            });
        }
        public Window1(int s)
        {
            InitializeComponent();        
        }

        void Ini2()
        {
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddr, port);

            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sListener.Bind(iPEndPoint);
                sListener.Listen(10);

                while (true)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        tb_main.Text += ($"Ожидаем соединение через порт {iPEndPoint} \n");
                        label_ip.Content = ipAddr;
                        label_port.Content = port;
                    }));

                    Socket handler = sListener.Accept();
                    byte[] msg = Encoding.UTF8.GetBytes(DateTime.Now.ToString());

                    byte[] buffer = new byte[1024];
                    Dispatcher.Invoke(() =>
                    {
                        tb_main.Text += ($"Client: {Encoding.UTF8.GetString(buffer, 0, handler.Receive(buffer))}\n");
                    });
                    handler.Send(msg);
                    /* if (builer.ToString() == "Bye")
                     {
                         handler.Send(Encoding.UTF8.GetBytes(builer.ToString().ToCharArray()));
                         handler.Shutdown(SocketShutdown.Both);
                         handler.Disconnect(true);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        tb_main.Text += ("Конец соединения\n");
                    }));
                         break;
                     }*/

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    tb_main.Text += ex.Message + "\n";
                }));
            }
        }
        void SendMessageFromSocket(int port)
        {
            byte[] buffer = new byte[1024];

            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(iPEndPoint);

            tb_main.Text += ($"Сокет соединяется с {sender.RemoteEndPoint.ToString()}\n");
            tb_main.Text += ($"Server: {Encoding.UTF8.GetString(buffer, 0, sender.Receive(buffer))}\n");            
            if(tb_send.Text.Length > 0)
            {
                tb_main.Text += "Client: " + tb_send.Text + "\n";
            }

            sender.Send(Encoding.UTF8.GetBytes(tb_send.Text.ToCharArray()));
            sender.Shutdown(SocketShutdown.Both);
            sender.Disconnect(true);

            tb_main.Text += ("Конец соединения\n");
            tb_send.Text = "";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SendMessageFromSocket(port);
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    tb_main.Text += (ex.Message) + "\n";
                }));
            }
        }
    }
}
