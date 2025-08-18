// See https://aka.ms/new-console-template for more information

using Fluxor;
using CretNet.Platform.Fluxor;
using CretNet.Platform.Playground;

Console.WriteLine("Hello, World!");

IDispatcher dispatcher = null;

TaskCompletionSource taskCompletionSource = new();

await dispatcher.DispatchAsync(new FetchCustomerAction());