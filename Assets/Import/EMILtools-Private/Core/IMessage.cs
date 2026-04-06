using UnityEngine;

/// <summary>
/// Polymorphic Base
/// </summary>
public interface ISignalReceiver { }

/// <summary>
/// Typed Polymorphic Signal
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISignalReceiverContext<T> : ISignalReceiver { } 


/// <summary>
/// Tagged Signal
/// </summary>
public interface ISignalReceiverTagged : ISignalReceiver
{
    string tag { get; }
    void Send(string senderTag) => ReceiveSignal(senderTag);
    void ReceiveSignal(string senderTag);
}


/// <summary>
/// Tagged Signal
/// </summary>
public interface ISignalReceiverTaggedContext<T> : ISignalReceiver, ISignalReceiverContext<T>
{
    string tag { get; }
    void Send(string senderTag, T ctx) => ReceiveSignal(senderTag, ctx);
    void ReceiveSignal(string tag, T ctx);
}


