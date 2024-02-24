using System;
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
    public static bool AttemptAuth = true;

    public static bool IsConnected { get { return NativeWebSocket.GetState() == SocketState.Open; } }
    public static bool CanSend { get { return IsConnected && PerformedAuthentication; } }

    public static bool Connecting { get => connectThread?.IsAlive ?? false; }

    
    public static bool ExpectingDisconnect = false;

    private static bool stopping = false;

    private static CancellationTokenSource cancelSource = new CancellationTokenSource();

    // don't even worry about it, this is temporary
    private static string tokenFile = "token.txt";
    public static bool WantConnection = System.IO.File.Exists(tokenFile); // yeah we'll mess with this later
    private static string token = System.IO.File.ReadAllText(tokenFile);


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
        NativeWebSocket.InitializeNetwork();

        connectThread = new Thread(() =>
        {
            RandomiserMod.Logger.LogInfo("Connection thread started");

            ExpectingDisconnect = false;
            try
            {
                RandomiserMod.Logger.LogInfo("attempting socket init");
                NativeWebSocket.SetUrl(ServerAddress);
                NativeWebSocket.Start();
                Serializer.PrepareSerializer<AuthenticateMessage>();
                Serializer.PrepareSerializer<Packet>();
                int retryAuthIn = 50;
                for (; ; )
                {
                    var state = NativeWebSocket.GetState();
                    if(state != SocketState.Open) {
                        RandomiserMod.Logger.LogInfo($"Socket currently {state}, waiting for Open");
                        Thread.Sleep(50);
                        continue;
                    }
                    if(AttemptAuth) {
                        retryAuthIn = 50;
                        var packet = new Packet
                        {
                            Id = Packet.PacketID.AuthenticateMessage,
                            packet = new AuthenticateMessage() { Jwt = token }.ToByteArray()
                        };
                        RandomiserMod.Logger.LogInfo($"Attempting token authentication");
                        NativeWebSocket.SendBinary(packet.ToByteArray());
                        AttemptAuth = false;
                    }
                    while (NativeWebSocket.HasPendingMessage()) {
                        HandleMessage(NativeWebSocket.GetPendingMessage());
                    }
                    if (PerformedAuthentication)
                        while (SendQueue.Any()) {
                            var packet = SendQueue.Take();
                            RandomiserMod.Logger.LogInfo($"Sending {packet.Id}");
                            NativeWebSocket.SendBinary(packet.ToByteArray());
                        }
                    else if (!AttemptAuth && --retryAuthIn <= 0)
                        AttemptAuth = true;
                    Thread.Sleep(50);
                }
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

    public static void HandleMessage(IEnumerable<byte> data) {
        try
        {
            var pack = Serializer.Deserialize<Packet>(new System.IO.MemoryStream(data.ToArray()));
            RandomiserMod.Logger.LogInfo($"Attempting to recieve a {pack.Id}");
            switch (pack.Id)
            {
                case Packet.PacketID.AuthenticatedMessage:
                    {
                        var authedMessage = Serializer.Deserialize<AuthenticatedMessage>(new System.IO.MemoryStream(pack.packet));
                        PerformedAuthentication = true;
                        RandomiserMod.Logger.LogInfo($"Successfully connected as: {authedMessage.User.Name}");
                        break;
                    }
                case Packet.PacketID.PrintTextMessage:
                    {
                        var printMessage = Serializer.Deserialize<PrintTextMessage>(new System.IO.MemoryStream(pack.packet));
                        Randomiser.Message(printMessage.Text, printMessage.Time);
                        break;
                    }
                case Packet.PacketID.UberStateBatchUpdateMessage:
                    {
                        var usMessage = Serializer.Deserialize<UberStateBatchUpdateMessage>(new System.IO.MemoryStream(pack.packet));
                        foreach (var usum in usMessage.Updates) handleUberStateUpdate(usum);
                        break;
                    }
                case Packet.PacketID.UberStateUpdateMessage:
                    {
                        handleUberStateUpdate(Serializer.Deserialize<UberStateUpdateMessage>(new System.IO.MemoryStream(pack.packet)));
                        break;
                    }
                default:
                    {
                        RandomiserMod.Logger.LogInfo($"No special handler for {pack.Id}");
                        break;
                    }
            }
        } catch (Exception ex)
        {
            RandomiserMod.Logger.LogError($"HandleMessage: ${ex}");
        }
    }

    private static void handleUberStateUpdate(UberStateUpdateMessage message)
    {
        try {
            message.State().Set(Convert.ToInt32(message.Value));
            RandomiserMod.Logger.LogInfo($"{message.UberId()} => {message.Value}"); 
        }
        catch (Exception ex)  {
            RandomiserMod.Logger.LogError($"handleUberStateUpdate: ${ex}");
        }
    }

    public static void Disconnect()
    {

    }
}
