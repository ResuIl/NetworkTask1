using System.Net.Sockets;
using System.Net;
using System.Windows;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace Client;

public partial class MainWindow : Window
{
    private Socket client;
    private EndPoint remoteEP;
    public MainWindow()
    {
        InitializeComponent();

        client = new Socket(AddressFamily.InterNetwork,
                            SocketType.Dgram,
                            ProtocolType.Udp);

        var ip = IPAddress.Parse("127.0.0.1");
        var port = 45678;
        remoteEP = new IPEndPoint(ip, port);

        Receiever();
    }

    private async void Receiever()
    {
        var buffer = new byte[ushort.MaxValue - 29];
        await client.SendToAsync(buffer, SocketFlags.None, remoteEP);
        var list = new List<byte>();
        var len = 0;
        do
        {
            try
            {
                var result = await client.ReceiveFromAsync(buffer, SocketFlags.None, remoteEP);
                len = result.ReceivedBytes;

                list.AddRange(buffer.Take(len));

                var image = LoadImage(list.ToArray());
                if (image != null)
                    img.Source = image;
            } catch (Exception ex)
            {

            }
            

        } while (len == buffer.Length);
    }

    private static BitmapImage? LoadImage(byte[] imageData)
    {
        var image = new BitmapImage();
        image.BeginInit();
        image.StreamSource = new MemoryStream(imageData);
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        return image;
    }

    private void Button_Click(object sender, RoutedEventArgs e) => Receiever();
}
