// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <author>Ishan Sharma</author>
// <author>Anshul Kasera</author>

using CS3500.Networking;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Text;

namespace CS3500.Chatting;

/// <summary>
///   A simple ChatServer that handles clients separately and replies with a static message.
/// </summary>
public partial class ChatServer
{
    /// <summary>
    /// Dictionary that maps client names to the connection sockets. 
    /// </summary>
    private static Dictionary<String, NetworkConnection> clients = new();

    /// <summary>
    ///   The main program.
    /// </summary>
    /// <param name="args"> ignored. </param>
    /// <returns> A Task. Not really used. </returns>
    private static void Main(string[] args)
    {
        Server.StartServer(HandleConnect, 11_000);
        Console.Read(); // don't stop the program.
    }


    /// <summary>
    ///   <pre>
    ///     When a new connection is established, enter a loop that receives from and
    ///     replies to a client.
    ///   </pre>
    /// </summary>
    ///
    private static void HandleConnect(NetworkConnection connection)
    {
        // handle all messages until disconnect.
        string name = ""; 
        try
        {
            name = connection.ReadLine();
            lock (clients)
            {
                if (clients.ContainsKey(name))
                    name += " -duplicate";
                clients.Add(name, connection);
            }

            Console.WriteLine("received message by: " + name);
            while (true)
            {
                var message = connection.ReadLine();
                lock (clients)
                {
                    foreach (String client in clients.Keys)
                        clients[client].Send(name + ": " + message);
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("The client " + name + " has been disconnected");
            clients.Remove(name);
        }
    }
}