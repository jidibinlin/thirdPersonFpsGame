using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Linq;
using ProtoDefine;
using UnityEngine.UI;
using Agent;
using Gateway;
using System.Text;

public class sync:MonoBehaviour
{
    private Int32 frame = 1;
    private Socket socket;
    private byte[] readBuff = new byte[8];
    // Start is called before the first frame update
    //[RuntimeInitializeOnLoadMethod]

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public InputField inputPlayerid;
    public InputField inputPassword;
    public Button loginButton;
    public Button enterScene;

    public string playerid;

    private Dictionary<string,dynamic> gamingPlayers = new Dictionary<string, dynamic>();
    // TUDO:player curret

    void Start(){
        Connect("127.0.0.1",8001);
        loginButton.onClick.AddListener(delegate(){
            Debug.Log("登录");
            string id = inputPlayerid.text;
            string password = inputPassword.text;
            playerid = id;
            login(id,password);
            inputPlayerid.GetComponent<CanvasGroup>().alpha=0;
            inputPlayerid.GetComponent<CanvasGroup>().interactable=false;
            inputPassword.GetComponent<CanvasGroup>().alpha = 0;
            inputPassword.GetComponent<CanvasGroup>().interactable=false;
            loginButton.GetComponent<CanvasGroup>().alpha = 0;
            loginButton.GetComponent<CanvasGroup>().interactable=false;
        });
        enterScene.onClick.AddListener(delegate(){
            Debug.Log("发送进入场景请求");
            EnterScene();
        });
    }

    public void EnterScene(){
        enterScene enmsg = new enterScene();
        enmsg.Cmd="enterScene";
        enmsg.Id = protoReolver.nameToId[enmsg.Cmd];
        enmsg.Playerid=playerid;
        byte[] sendBuf= protoReolver.pack(enmsg);
        socket.Send(sendBuf,0,sendBuf.Length,SocketFlags.None);
        // Instantiate(playerPrefab);
        // Instantiate(enemyPrefab);
        
    }

    public void Connect(string ip, int port)
    {
        protoReolver.addInstance();
        protoReolver.addParser();
        protoReolver.addNameToId();
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(ip, port);
        Debug.Log("connect successed");

        // Thread.Sleep(1100);
        // login("2694273649@qq.com","123456");
    }

    private bool login(string playerid,string password){
        Gateway.login msg = new Gateway.login();
        msg.Cmd = "login";
        msg.Password = password;
        msg.Playerid = playerid;
        msg.Id = protoReolver.nameToId["login"];
        byte[] send = protoReolver.pack(msg);
        Debug.Log("length =%d"+send.Length.ToString());
        socket.Send(send,0,send.Length,SocketFlags.None);
        // System.Console.Write(send);
        // Debug.Log("send content"+BitConverter.ToString(send));

        Debug.Log("login message sended");
        socket.BeginReceive(readBuff, 0, 8, SocketFlags.None, ReceiveCallback, socket);
        // msg.Cmd = 
        return true;
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        Debug.Log("receive call back");
        Debug.Log("redbuff content "+BitConverter.ToString(readBuff));
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            byte[] lenbuff = readBuff.Take(4).ToArray<byte>();
            byte[] idbuff = readBuff.Skip(4).ToArray<byte>();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lenbuff);
                Array.Reverse(idbuff);
            }

            Int32 len = BitConverter.ToInt32(lenbuff,0);
            Int32 id = BitConverter.ToInt32(idbuff,0);
            // Debug.Log("len="+len);
            // Debug.Log("id="+id);

            byte[] protoBuff = new byte[len];
            socket.Receive(protoBuff, 0, len, SocketFlags.None);
            Debug.Log("redbuff content "+BitConverter.ToString(protoBuff));

            dynamic msg = protoReolver.unPack(id,protoBuff);
            Debug.Log("id="+msg.Cmd);
            if("loginResp".Equals(msg.Cmd)){
                if(msg.Stat == 2 ){
                    Debug.Log("login failed reason:"+msg.Reason);
                    return;
                }else{
                    Debug.Log("login successed");
                }

            }
            Thread SyncThread = new Thread(startSync);
                // Int32 now = DateTime.Now.Millisecond;
                // SyncThread.Start(now);

            if("startGame".Equals(msg.Cmd)){

                Debug.Log("msg id = "+msg.Id);


                foreach (var player in msg.Players)
                {
                    Debug.Log("p");
                }
                Debug.Log("创建人物和预制体");
                Int32 now = DateTime.Now.Millisecond;
                SyncThread.Start(now);
                Debug.Log("同步线程开启成功");

                startGame s = new startGame();
            }

            if("endGame".Equals(msg.Cmd)){
                SyncThread.Abort();
            }


            if("Gaming".Equals(msg.Cmd)){

                // TODO: 向对应的消息队列插入此条消息
                // gamingPlayers[msg.playerid].insert()
            }

            socket.BeginReceive(readBuff, 0, 8, SocketFlags.None, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket receive fail" + ex.ToString());
        }
    }

    // private static void sendMsg(dynamic,){

    // }

    public void startSync(object startTime){
        Double stime = Convert.ToDouble(startTime);
        Debug.Log("Stime"+stime);
        while (true)
        {
            // TODO: 同步部分         
            // 1 快照本机的玩家             
            // 2 异步发送当前玩家数据
            // 3 update 玩家数据
            frame +=1;
            Double etime = DateTime.Now.Millisecond;            
            // Debug.Log("Etime"+etime);
            Double dTime = etime-stime;
            int sleepTime =(int)(frame * 20-dTime);
            if(sleepTime <0){
                sleepTime = 10;
            }
            Debug.Log(""+frame);
            Thread.Sleep(sleepTime);
        }
    }
}
