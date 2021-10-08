using System;
using System.Collections;
using System.Collections.Generic;
using Agent;
using Gateway;
using System.IO;
using Google.Protobuf;
using System.Linq;
using UnityEngine;

namespace ProtoDefine
{
    public class protoReolver
    {
        private static Dictionary<int, Delegate> protos = new Dictionary<int, Delegate>();
        public static Dictionary<string, int> nameToId = new Dictionary<string, int>();
        public static Dictionary<int,Delegate> parser = new Dictionary<int, Delegate>();
        public static void addInstance()
        {
            protos.Add(1, new Func<Gateway.login>(login));
            protos.Add(2, new Func<Gateway.loginResp> (loginResp));
            protos.Add(3, new Func<Agent.enterScene>(enterScene));
            protos.Add(4, new Func<Agent.sureEnterScene>(sureEnterScene));
            protos.Add(5, new Func<Agent.startGame>(startGame));
        }

        public static void addParser(){
            parser.Add(1, new Func<byte[],Gateway.login>(plogin));
            parser.Add(2, new Func<byte[],Gateway.loginResp>(ploginResp));
            parser.Add(3, new Func<byte[],Agent.enterScene> (penterScene));
            parser.Add(4, new Func<byte[],Agent.sureEnterScene>(psureEnterScene));
            parser.Add(5, new Func<byte[], Agent.startGame> (pstartGame));
        }

        public static Gateway.login plogin(byte [] buf){
            return Gateway.login.Parser.ParseFrom(buf);
        }
        public static Gateway.loginResp ploginResp(byte[] buf){
            return Gateway.loginResp.Parser.ParseFrom(buf);
        }
        public static Agent.enterScene penterScene(byte[] buf){
            return Agent.enterScene.Parser.ParseFrom(buf);
        }
        public static Agent.sureEnterScene psureEnterScene(byte[] buf){
            return Agent.sureEnterScene.Parser.ParseFrom(buf);
        }
        public static Agent.startGame pstartGame(byte[] buf){
            return Agent.startGame.Parser.ParseFrom(buf);
        }

        public static Gateway.login login(){
            return new Gateway.login();
        }
        public static Gateway.loginResp loginResp(){
            return new Gateway.loginResp();
        }
        public static Agent.enterScene enterScene(){
            return new Agent.enterScene();
        }
        public static Agent.sureEnterScene sureEnterScene(){
            return new Agent.sureEnterScene();
        }
        public static Agent.startGame startGame(){
            return new Agent.startGame();
        }

        public static void addNameToId(){
            nameToId.Add("login",1);
            nameToId.Add("loginResp",2);
            nameToId.Add("enterScene",3);
            nameToId.Add("sureEnterScene",4);
            nameToId.Add("startGame",5);
            nameToId.Add("addPerson",6);
            nameToId.Add("resp",7);
        }

        public static dynamic unPack(int id, byte[] msg)
        {
            // Array.Reverse(msg);
            dynamic proto = parser[id].DynamicInvoke(msg);
            // Debug.Log("cmd"+proto.Cmd);
            return proto;
        }

        public static byte[] pack(dynamic msg)
        {
            Int32 len = msg.CalculateSize();
            Int32 id = msg.Id;
            byte[] data = new byte[len];
            using(CodedOutputStream cos = new CodedOutputStream(data)){
                msg.WriteTo(cos);
            }

            // Debug.Log("id= "+msg.Id);
            // Debug.Log("playerid"+msg.Playerid);
            // Debug.Log("password"+msg.Password);
            Debug.Log("datacontent "+BitConverter.ToString(data));


            byte []lenByte = BitConverter.GetBytes(len) ;
            Array.Reverse(lenByte);
            byte[] idByte = BitConverter.GetBytes(id);
            Array.Reverse(idByte);
            byte [] send = new byte[lenByte.Length+idByte.Length+data.Length];
            // Array.Reverse(data);
            // send = lenByte.Concat(idByte).ToArray().Concat(send).ToArray();
            // send.Reverse();
            lenByte.CopyTo(send,0);
            idByte.CopyTo(send,lenByte.Length);
            data.CopyTo(send,idByte.Length+lenByte.Length);
            Debug.Log("datacontent "+BitConverter.ToString(send));

            return send;
        }
    }
}
