using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;
using System.Diagnostics;   // para usar contagem de tempo

public class Client_UDP : MonoBehaviour
{
    public static Client_UDP Singleton;

    //public GameManager gm;

    byte[] data = new byte[1024];
    string input, stringData;

    InputField iField;
    string myName;

    Socket sock;
    IPEndPoint ep;
    EndPoint Remoto;

    static Stopwatch cronometro;
    static long latencia;

    private void Awake()
    {
        if (Singleton == null) Singleton = this;
        else Destroy(Singleton);
    }

    // Start is called before the first frame update
    void Start()
    {
        cronometro = new Stopwatch();

        print("Iniciando Cliente UDP na Unity");

        print("cria um IPEndPoint com o enderço do Servidor (no caso localhost).");
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

        print("Cria o socket UDP para comunicação com o servidor.");
        // Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // monta a mensagem para a ser enviada ao Servidor
        string welcome = "Estabelecendo conexao com o Servidor.";
        data = Encoding.ASCII.GetBytes(welcome);

        // Envia o dado para o Servidor. Para  enviar uma mensagem UDP, 
        // é necessário colocar o endereço do Servidor na mensagem (ipep)
        print("Estabelecendo conexao com o Servidor...");

        cronometro.Start();                 // inicia contagemm de tempo
        sock.SendTo(data, data.Length, SocketFlags.None, ipep);

        // cria um IPEndPoint com o endereço do cliente para  receber as mensagens  do servidor.
        // IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
        // EndPoint Remoto = (EndPoint)ep;

        ep = new IPEndPoint(IPAddress.Any, 0);
        Remoto = (EndPoint)ep;

        data = new byte[1024];

        // o método ReceiveFrom() irá colocar a informação do IPEndPoit do servidor no objeto 
        // Remoto (via referencia) isto ocorre no momento que a mensagem chega.
        int recv = sock.ReceiveFrom(data, ref Remoto);
        cronometro.Stop();                 // para contagemm de tempo
        latencia = cronometro.ElapsedMilliseconds;
        print("Mensagem recebida do Servidor: " + Encoding.ASCII.GetString(data, 0, recv));
        print(" ");
        print("--> Latencia entre envio de mensagem e a recepcao da resposta:" + latencia + " milissegundos");

        //StartCoroutine("GetServerResponse");
    }

    public string SendToServer(string msg)
    {
        sock.SendTo(Encoding.ASCII.GetBytes(msg), Remoto);

        int recv = sock.ReceiveFrom(data, ref Remoto);
        string answer = Encoding.ASCII.GetString(data, 0, recv);
        print("Mensagem do servidor: " + answer);

        return answer;
    }

    //Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            print("Pressionou space");
            SendToServer("espaco");
        }

        //if (Input.GetKeyDown("up"))
        //{
        //    print("Pressionou UP");
        //    sock.SendTo(Encoding.ASCII.GetBytes("cima"), Remoto);
        //}


        //if (Input.GetKeyDown("down"))
        //{
        //    print("Pressionou DOWN");
        //    sock.SendTo(Encoding.ASCII.GetBytes("baixo"), Remoto);
        //}


        //if (Input.GetKeyDown("left"))
        //{
        //    print("Pressionou LEFT");
        //    sock.SendTo(Encoding.ASCII.GetBytes("esquerda"), Remoto);
        //}


        //if (Input.GetKeyDown("right"))
        //{
        //    print("Pressionou RIGHT");
        //    sock.SendTo(Encoding.ASCII.GetBytes("direita"), Remoto);
        //}

        if (Input.GetKeyDown("end"))
        {
            print("Pressionou fim. Encerrando o Cliente Unity ...");
            sock.SendTo(Encoding.ASCII.GetBytes("Encerrando o Cliente Unity ..."), Remoto);
            sock.Close();
        }

    }
}

