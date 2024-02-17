﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using Network;
using System.Collections.Concurrent;
using WebSocketSharp;
using ProtoBuf;
using Randomiser.Extensions;
using System.Security.Authentication;
using UnityEngine;
using System.Net;

namespace Randomiser.Multiplayer.OriRando;

public class WebsocketController : MonoBehaviour
{
    public void Awake()
    {
        WebsocketClient.Connect();
    }
}
public static class WebsocketClient 
{
    public static string Domain = "wotw.orirando.com";

    private static string ServerAddress => $"wss://{Domain}/api/game_sync/";

    public static BlockingCollection<Packet> SendQueue = new BlockingCollection<Packet>();
    public static BlockingCollection<Packet> RecieveQueue = new BlockingCollection<Packet>();

    public static float TimeUntilReconnectAttempt = 5.0f;

    private static Thread updateThread;
    private static Thread connectThread;
    public static bool PerformedAuthentication = false;

    public static bool IsConnected { get { return socket != null && (socket.ReadyState == WebSocketState.Open || socket.ReadyState == WebSocketState.Closing); } }
    public static bool CanSend { get { return IsConnected && socket.ReadyState == WebSocketState.Open && PerformedAuthentication; } }

    public static bool Connecting { get => connectThread?.IsAlive ?? false; }

    private static string tokenFile = "token.txt";

    public static bool WantConnection = System.IO.File.Exists(tokenFile); // yeah we'll mess with this later

    public static bool ExpectingDisconnect = false;

    private static bool stopping = false;

    private static CancellationTokenSource cancelSource = new CancellationTokenSource();

    // don't even worry about it, this is temporary
    private static string token = System.IO.File.ReadAllText(tokenFile);

    private static WebSocket socket;

    private static Dictionary<LogLevel, BepInEx.Logging.LogLevel> llt = new Dictionary<LogLevel, BepInEx.Logging.LogLevel>(){
        {LogLevel.Trace, BepInEx.Logging.LogLevel.Debug},
        {LogLevel.Debug, BepInEx.Logging.LogLevel.Debug},
        {LogLevel.Info, BepInEx.Logging.LogLevel.Info},
        {LogLevel.Warn, BepInEx.Logging.LogLevel.Warning},
        {LogLevel.Error, BepInEx.Logging.LogLevel.Error},
        {LogLevel.Fatal, BepInEx.Logging.LogLevel.Fatal},
    };
    public static void Connect()
    {
        RandomiserMod.Logger.LogInfo("Connection Request Recieved");

        if (!WantConnection) return;
        //        setupUpdateThread();
        if (Connecting)
        {
            RandomiserMod.Logger.LogDebug("Skipping connection request as one is in-progress");
            TimeUntilReconnectAttempt = 2.0f;
            return;
        }
        connectThread = new Thread(() =>
        {
            RandomiserMod.Logger.LogInfo("Connection thread started");

            if (socket != null)
                Disconnect();

            ExpectingDisconnect = false;
            try
            {
                RandomiserMod.Logger.LogInfo("attempting socket init");

                socket = new WebSocket(ServerAddress);

                socket.Log.Level = LogLevel.Info;
                socket.Log.Output = (logdata, output) => {
                    try
                    {
                        if (logdata != null)
                            RandomiserMod.Logger.Log(llt[logdata.Level], $"Websocket says: {logdata.Message}");
                        else
                            RandomiserMod.Logger.LogInfo($"Websocket output: {output}");
                    }
                    catch (Exception e)
                    {
                        RandomiserMod.Logger.LogError($"Websocket log {e}");
                    }
                };
                socket.OnError += (sender, e) => {
                RandomiserMod.Logger.LogError($"WebSocket: {e} {e?.Exception}");
                    TimeUntilReconnectAttempt = 10.0f;
                    PerformedAuthentication = false;
                };
                socket.OnClose += (sender, e) => {
                    RandomiserMod.Logger.LogError($"Closing socket ({e?.Code}: {e?.Reason})");
                    PerformedAuthentication = false;
                    if (!ExpectingDisconnect)
                        RandomiserMod.Logger.LogInfo("Disconnected! Retrying in 5s");
                };
                socket.OnOpen += (sender, args) => {
                    RandomiserMod.Logger.LogInfo($"Connected to server");
                    Packet packet = new Packet
                    {
                        Id = Packet.PacketID.AuthenticateMessage,
                        packet = new AuthenticateMessage() { Jwt = token }.ToByteArray()
                    };
                    socket.Send(packet.ToByteArray());
                    PerformedAuthentication = true;
                };
                socket.OnMessage += HandleMessage;
                RandomiserMod.Logger.LogInfo($"Attempting to connect to {Domain}");
                socket.Connect();
            }
            catch (Exception ex)
            {
                TimeUntilReconnectAttempt = 2.0f;
                RandomiserMod.Logger.LogError($"Connect (socket): ${ex}");
                PerformedAuthentication = false;
            }
        });
        connectThread.Start();
    }
    public static void HandleMessage(object sender, MessageEventArgs args) {
        try
        {
            var data = args.RawData;
            if (data == null)
            {
                RandomiserMod.Logger.LogError("received message with no data!");
                return;
            }
            var pack = Serializer.Deserialize<Packet>(new System.IO.MemoryStream(data));
            switch(pack.Id)
            {
                case Packet.PacketID.AuthenticatedMessage:
                    {
                        var authedMessage = Serializer.Deserialize<AuthenticatedMessage>(new System.IO.MemoryStream(pack.packet));
                        PerformedAuthentication = true;
                        RandomiserMod.Logger.LogInfo($"Successfully connected: {authedMessage.User}");
                        break;
                    }
                default:
                    {
                        RandomiserMod.Logger.LogInfo($"Recieved Packet ID {pack.Id}");
                        break;
                    }
            }
        } catch (Exception ex)
        {
            RandomiserMod.Logger.LogError($"HandleMessage: ${ex}");
        }
    }

    public static void Disconnect()
    {

    }
}