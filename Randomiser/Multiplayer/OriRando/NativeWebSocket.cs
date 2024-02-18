using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Randomiser.Multiplayer.OriRando;

public enum SocketState : int
{
    Connecting = 0,
    Open = 1,
    Closing = 2,
    Closed = 3,
}

public unsafe static class NativeWebSocket
{
    [DllImport("BepInEx/plugins/Rando/NativeWebSocket.dll", EntryPoint = "initialize_network", CallingConvention = CallingConvention.Cdecl)]
    public static extern void InitializeNetwork();

    [DllImport("BepInEx/plugins/Rando/NativeWebSocket.dll", EntryPoint = "finalize_network", CallingConvention = CallingConvention.Cdecl)]
    public static extern void FinalizeNetwork();

    [DllImport("BepInEx/plugins/Rando/NativeWebSocket.dll", EntryPoint = "set_url", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetUrl([MarshalAs(UnmanagedType.LPStr)] string path);

    [DllImport("BepInEx/plugins/Rando/NativeWebSocket.dll", EntryPoint = "start", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Start();

    [DllImport("BepInEx/plugins/Rando/NativeWebSocket.dll", EntryPoint = "send_binary", CallingConvention = CallingConvention.Cdecl)]
    private static extern void SendBinaryRaw(byte[] data, int length);

    public static void SendBinary(byte[] data)
    {
        SendBinaryRaw(data, data.Length);
    }

    [DllImport("BepInEx/plugins/Rando/NativeWebSocket.dll", EntryPoint = "stop", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Stop();

    [DllImport("BepInEx/plugins/Rando/NativeWebSocket.dll", EntryPoint = "get_state", CallingConvention = CallingConvention.Cdecl)]
    public static extern SocketState GetState();

    [DllImport("BepInEx/plugins/Rando/NativeWebSocket.dll", EntryPoint = "has_pending_message", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool HasPendingMessage();

    [DllImport("BepInEx/plugins/Rando/NativeWebSocket.dll", EntryPoint = "get_pending_message", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GetPendingMessage(out int length);

    [DllImport("BepInEx/plugins/Rando/NativeWebSocket.dll", EntryPoint = "pop_pending_message", CallingConvention = CallingConvention.Cdecl)]
    private static extern void PopPendingMessage();

    public static IEnumerable<byte> GetPendingMessage()
    {
        var message = GetPendingMessage(out var length);
        var bytes = new byte[length];
        Marshal.Copy(message, bytes, 0, length);
        PopPendingMessage();
        return bytes;
    }
}
