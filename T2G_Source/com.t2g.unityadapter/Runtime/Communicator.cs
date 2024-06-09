using System;
using Unity.Jobs;
using Unity.Collections;
using Unity.Networking.Transport;

namespace T2G.UnityAdapter
{
    public enum eMessageType : byte     //TODO: used to improve the communication's message serialization and deserialization 
    {
        //System reserved types
        ClientId = 0,
        UserName,
        ResourcePath,
        GenerateGame,
        //Primitive types
        Int,
        Byte,
        Bytes,
        Float,
        Double,
        Long,
        Short,
        UInt,
        ULong,
        UShort,
        PackedInt,
        PackedFloat,
        PackedDouble,
        PackedLong,
        PackedUInt,
        PackedULong,
        String32,
        String64,
        String128,
        String512,
        String4096,
        RawBits
        //Complex data structures
        //...
    }

    public struct MessageStruct
    {
        public eMessageType Type;
        public FixedString4096Bytes Message;
    }

    public class Communicator
    {
        public float ConnectiontimeOut = 3.0f;
        public readonly string IPAddress = "127.0.0.1";
        public ushort Port = 7778;
        public int SendMessagePoolSize = 32;
        public int ReceiveMessagePoolSize = 256;
        public readonly int MaxMessageLength = 4096;


        public Action<string> OnSystemError;
        public Action<string> OnSentMessage;
        public Action<string> OnReceivedMessage;

        public enum eNetworkPipeline
        {
            None = 0,
            FragmentationStageOnly,
            FragmentationAndReliableStages,
            FragmentationAndSimulatorStages,
            FragmentationAndReliableAndSimulatorStages
        }

        public eNetworkPipeline NetworkPipelineType = eNetworkPipeline.FragmentationAndReliableStages;

        protected NetworkPipeline _networkpipeline;
        protected NetworkSettings _networkSettings;
        protected NetworkDriver _networkDriver;
        protected NativeArray<NetworkConnection> _connections;
        protected JobHandle _jobHandle;

  
        protected NativeArray<MessageStruct> _sendMessagePool;
        protected int _sendPoolHead = 0;
        protected int _sendPoolTail = 0;
        protected NativeArray<MessageStruct> _receiveMessagePool;
        protected int _receivePoolHead = 0;
        protected int _receivePoolTail = 0;

        protected virtual void Initialize()
        {
            _networkSettings = new NetworkSettings();
            _networkSettings.WithNetworkConfigParameters();         //Use default
            _networkSettings.WithNetworkSimulatorParameters();      //Use default
            _networkDriver = NetworkDriver.Create(_networkSettings);
            _connections = new NativeArray<NetworkConnection>(1, Allocator.Persistent);
            _connections[0] = default(NetworkConnection);
            _sendMessagePool = new NativeArray<MessageStruct>(SendMessagePoolSize, Allocator.Persistent);
            _receiveMessagePool = new NativeArray<MessageStruct>(ReceiveMessagePoolSize, Allocator.Persistent);
            _sendPoolHead = _sendPoolTail = _receivePoolHead = _receivePoolTail = 0;

            switch (NetworkPipelineType)
            {
                case eNetworkPipeline.FragmentationStageOnly:
                    _networkpipeline = _networkDriver.CreatePipeline(typeof(FragmentationPipelineStage));
                    break;
                case eNetworkPipeline.FragmentationAndReliableStages:
                    _networkpipeline = _networkDriver.CreatePipeline(
                        typeof(FragmentationPipelineStage),
                        typeof(ReliableSequencedPipelineStage));
                    break;
                case eNetworkPipeline.FragmentationAndSimulatorStages:
                    _networkpipeline = _networkDriver.CreatePipeline(
                        typeof(FragmentationPipelineStage),
                        typeof(SimulatorPipelineStage));
                    break;
                case eNetworkPipeline.FragmentationAndReliableAndSimulatorStages:
                    _networkpipeline = _networkDriver.CreatePipeline(
                        typeof(FragmentationPipelineStage),
                        typeof(ReliableSequencedPipelineStage),
                        typeof(SimulatorPipelineStage));
                    break;
                default:
                    _networkpipeline = NetworkPipeline.Null;
                    break;
            }
        }

        protected virtual void Dispose()
        {
            if (!IsActive)
            {
                return;
            }

            _jobHandle.Complete();

            _connections.Dispose();
            _sendMessagePool.Dispose();
            _receiveMessagePool.Dispose();
            _sendPoolHead = _sendPoolTail = _receivePoolHead = _receivePoolTail = 0;
            _networkDriver.ScheduleUpdate().Complete();
            _networkDriver.Dispose();
        }

        public bool IsActive => _networkDriver.IsCreated;

        public bool IsSendPoolEmpty => (_sendPoolHead == _sendPoolTail);
        public bool IsReceivePoolEmpty => (_receivePoolHead == _receivePoolTail);
        public bool IsReceivingPoolFull => (
            (_receivePoolTail > 0 && _receivePoolHead == _receivePoolTail - 1)
            || (_receivePoolTail == 0 && _receivePoolHead == _receiveMessagePool.Length - 1));
        public bool IsConnected => (_connections.Length > 0 && _connections[0].IsCreated);

        public bool SendMessage(MessageStruct messageData)
        {
            if (_sendPoolTail == 0 && _sendPoolHead == _sendMessagePool.Length - 1 ||
                _sendPoolTail > 0 && _sendPoolHead == _sendPoolTail - 1)
            {
                return false;       //The pool is full
            }

            _sendMessagePool[_sendPoolHead++] = messageData;

            if (_sendPoolHead == _sendMessagePool.Length)
            {
                _sendPoolHead = 0;
            }
            return true;
        }

        public bool GetReceivedMessage(out MessageStruct messageData)
        {
            if (IsReceivePoolEmpty)
            {
                messageData = default;
                return false;
            }

            messageData = _receiveMessagePool[_receivePoolTail++];
            if (_receivePoolTail == _receiveMessagePool.Length)
            {
                _receivePoolTail = 0;
            }
            return true;
        }

        protected bool PoolReceivedMessage(MessageStruct messageData)
        {
            if (IsReceivingPoolFull)
            {
                OnSystemError?.Invoke("The receiving pool is full!");
                return false;
            }
            _receiveMessagePool[_receivePoolHead++] = messageData;
            if (_receivePoolHead == _receiveMessagePool.Length)
            {
                _receivePoolHead = 0;
            }
            OnReceivedMessage?.Invoke(messageData.Message.ToString());
            return true;
        }

        protected virtual void SendPooledMessege()
        {
            if(!IsConnected)
            {
                return;
            }

            if (_sendPoolHead != _sendPoolTail)
            {
                var sendMessage = _sendMessagePool[_sendPoolTail++];
                if(_sendPoolTail >= SendMessagePoolSize)
                {
                    _sendPoolTail = 0;
                }

                if (sendMessage.Message.Length <= MaxMessageLength)
                {
                    _networkDriver.BeginSend(_networkpipeline, _connections[0], out var writer);
                    writer.WriteFixedString4096(sendMessage.Message);
                    _networkDriver.EndSend(writer);
                    OnSentMessage?.Invoke(sendMessage.Message.ToString());
                }
            }
        }
    }
}

