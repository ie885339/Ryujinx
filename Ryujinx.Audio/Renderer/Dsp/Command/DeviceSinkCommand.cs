using Ryujinx.Audio.Integration;
using Ryujinx.Audio.Renderer.Server.Sink;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ryujinx.Audio.Renderer.Dsp.Command
{
    public class DeviceSinkCommand : ICommand
    {
        public bool Enabled { get; set; }

        public int NodeId { get; }

        public CommandType CommandType => CommandType.DeviceSink;

        public ulong EstimatedProcessingTime { get; set; }

        public string DeviceName { get; }

        public int SessionId { get; }

        public uint InputCount { get; }
        public ushort[] InputBufferIndices { get; }

        public Memory<float> Buffers { get; }

        public DeviceSinkCommand(uint bufferOffset, DeviceSink sink, int sessionId, Memory<float> buffers, int nodeId)
        {
            Enabled = true;
            NodeId = nodeId;

            DeviceName = Encoding.ASCII.GetString(sink.Parameter.DeviceName).TrimEnd('\0');
            SessionId = sessionId;
            InputCount = sink.Parameter.InputCount;
            InputBufferIndices = new ushort[InputCount];

            for (int i = 0; i < Math.Min(InputCount, Constants.ChannelCountMax); i++)
            {
                InputBufferIndices[i] = (ushort)(bufferOffset + sink.Parameter.Input[i]);
            }

            if (sink.UpsamplerState != null)
            {
                Buffers = sink.UpsamplerState.OutputBuffer;
            }
            else
            {
                Buffers = buffers;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Span<float> GetBuffer(int index, int sampleCount)
        {
            return Buffers.Span.Slice(index * sampleCount, sampleCount);
        }

        public void Process(CommandList context)
        {
            IHardwareDevice device = context.OutputDevice;

            if (device.GetSampleRate() == Constants.TargetSampleRate)
            {
                int channelCount = (int)device.GetChannelCount();
                uint bufferCount = Math.Min(device.GetChannelCount(), InputCount);

                const int sampleCount = Constants.TargetSampleCount;

                short[] outputBuffer = new short[bufferCount * sampleCount];

                for (int i = 0; i < bufferCount; i++)
                {
                    ReadOnlySpan<float> inputBuffer = GetBuffer(InputBufferIndices[i], sampleCount);

                    for (int j = 0; j < sampleCount; j++)
                    {
                        outputBuffer[i + j * channelCount] = PcmHelper.Saturate(inputBuffer[j]);
                    }
                }

                device.AppendBuffer(outputBuffer, InputCount);
            }
            else
            {
                // TODO: support resampling for device only supporting something different
                throw new NotImplementedException();
            }
        }
    }
}
