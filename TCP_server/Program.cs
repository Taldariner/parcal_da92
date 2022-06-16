﻿using TCP_test;

var server = new ServerObject();
try
{
    var listenThread = new Thread(server.Listen);
    listenThread.Start();
}
catch (Exception e)
{
    server.Disconnect();
    Console.WriteLine(e);
}