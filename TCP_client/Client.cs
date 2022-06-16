using System.Net.Sockets;
using System.Text;

namespace TCP_client;

public class Client
{
    private readonly int _port;
    private readonly string _name;
    private readonly string _server;

    private int countsOfQuestions;
    
    private TcpClient _client;
    private NetworkStream _stream;

    public Client(string name, string server = "127.0.0.1", int port = 8888)
    {
        _name = name;
        _server = server;
        _port = port;

        _client = new TcpClient();
        _stream = null;
        countsOfQuestions = 10;
    }

    public void RunCLient()
    {
        try
        {
            _client.Connect(_server, _port);
            _stream = _client.GetStream();
            
            Console.WriteLine("Hello to the test! You must write in console number 1-4");
            var data = Encoding.Unicode.GetBytes(_name);
            _stream.Write(data, 0, data.Length);

            var receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();
            SendMessage();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Disconnect();
        }
    }
    
    void SendMessage()
    {
        while (true)
        {
            var message = Console.ReadLine();
            var data = Encoding.Unicode.GetBytes(message);
            _stream.Write(data, 0, data.Length);
        }
    }

    private void ReceiveMessage()
    {
        while (true)
        {
            try
            {
                var data = new byte[64];
                var builder = new StringBuilder();
                var bytes = 0;
                do
                {
                    bytes = _stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (_stream.DataAvailable);
 
                var message = builder.ToString();
                Console.WriteLine(message);
                
                //TODO передавати кількість питань тесту автоматично

                if (countsOfQuestions == 0) break;
                countsOfQuestions--;
            }
            catch
            {
                Console.WriteLine("Connection is lose!");
                Console.ReadLine();
                Disconnect();
            }
        }
        Disconnect();
    }

    private void Disconnect()
    {
        _stream.Close();
        _client.Close();
        Environment.Exit(0);
    }
}