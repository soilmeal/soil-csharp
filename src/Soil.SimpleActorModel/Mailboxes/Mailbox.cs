using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Soil.SimpleActorModel.Actors;
using Soil.SimpleActorModel.Messages;
using Soil.SimpleActorModel.Messages.System;
using Soil.Threading.Atomic;

namespace Soil.SimpleActorModel.Mailboxes;

public abstract class Mailbox : IMessageQueue
{
    private readonly AtomicInt32 _state = (int)MailboxState.Open;

    private readonly IActorContext _owner;

    private readonly IProducerConsumerCollection<SystemMessage> _systemMessages = new ConcurrentQueue<SystemMessage>();

    private readonly IMessageQueue _messageQueue;

    public int Count
    {
        get
        {
            return _messageQueue.Count;
        }
    }

    public bool IsSynchronized
    {
        get
        {
            return _messageQueue.IsSynchronized;
        }
    }

    public object SyncRoot
    {
        get
        {
            return _messageQueue.SyncRoot;
        }
    }

    public IActorRef Owner
    {
        get
        {
            return _owner;
        }
    }

    public IMessageQueue MessageQueue
    {
        get
        {
            return _messageQueue;
        }
    }

    public bool IsClosed
    {
        get
        {
            return HasState(MailboxState.Closed);
        }
    }

    protected Mailbox(IActorContext owner, IMessageQueue messageQueue)
    {
        _owner = owner;
        _messageQueue = messageQueue;
    }

    public bool TryAddSystemMessage(SystemMessage systemMessage)
    {
        return _systemMessages.TryAdd(systemMessage);
    }

    public bool TryTakeSystemMessage(out SystemMessage? systemMessage)
    {
        return _systemMessages.TryTake(out systemMessage);
    }

    public bool TryAdd(Envelope item)
    {
        return _messageQueue.TryAdd(item);
    }

    public bool TryTake(out Envelope item)
    {
        return _messageQueue.TryTake(out item);
    }

    public void Clear()
    {
        _messageQueue.Clear();
    }

    public void Close()
    {
        ChangeState(MailboxState.Closed);
    }

    public void CopyTo(Envelope[] array, int index)
    {
        _messageQueue.CopyTo(array, index);
    }

    public void CopyTo(Array array, int index)
    {
        _messageQueue.CopyTo(array, index);
    }

    public bool TrySetScheduled()
    {
        return TryChangeState(MailboxState.Scheduled, MailboxState.Open);
    }

    public bool TryBackToOpen()
    {
        return TryChangeState(MailboxState.Open, MailboxState.Scheduled);
    }

    public void Process()
    {
        try
        {
            if (IsClosed)
            {
                return;
            }

            ProcessMessage();
            ProcessAllSystemMessage();
        }
        finally
        {
            TryBackToOpen();
            _owner.Dispatcher.TryExecuteMailbox(this);
        }
    }

    private void ProcessAllSystemMessage()
    {
        while (TryTakeSystemMessage(out SystemMessage? systemMessage))
        {
            if (systemMessage == null)
            {
                continue;
            }

            _owner.InvokeSystem(systemMessage);
        }
    }

    private void ProcessMessage()
    {
        ProcessMessage(Math.Min(int.MaxValue, _owner.Dispatcher.ThroughputPerActor));
    }

    private void ProcessMessage(int throughput)
    {
        int processCount = 0;
        while (processCount < throughput && TryTake(out Envelope envelope))
        {
            _owner.Invoke(envelope);

            ProcessAllSystemMessage();

            processCount += 1;
        }
    }

    public Envelope[] ToArray()
    {
        return _messageQueue.ToArray();
    }

    public IEnumerator<Envelope> GetEnumerator()
    {
        return _messageQueue.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _messageQueue.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MailboxState GetState()
    {
        return (MailboxState)_state.Read();
    }

    private bool TryChangeState(MailboxState state, MailboxState comparandState)
    {
        return ((MailboxState)_state.CompareExchange((int)state, (int)comparandState)) == comparandState;
    }

    private MailboxState ChangeState(MailboxState state)
    {
        return (MailboxState)_state.Exchange((int)state);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool HasState(MailboxState state)
    {
        return GetState() == state;
    }
}
