using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace VoxelSeeds
{
    class Audio
    {
        XAudio2 device;
        MasteringVoice masteringVoice;
        SoundStream stream;
        AudioBuffer buffer;
        SourceVoice sourceVoice;

        public Audio(String fileName)
        {
            device = new XAudio2();
            masteringVoice = new MasteringVoice(device);
            stream = new SoundStream(File.OpenRead("Content/"+fileName));
            buffer = new AudioBuffer {Stream  = stream.ToDataStream(),
                AudioBytes = (int)stream.Length, Flags = BufferFlags.EndOfStream};
            stream.Close();
        }

        public void startSound(int loop = -1)
        {
            buffer.LoopCount = loop;
            sourceVoice = new SourceVoice(device, stream.Format, true);
            sourceVoice.SubmitSourceBuffer(buffer, stream.DecodedPacketsInfo);
            sourceVoice.Start();
        }

        public void endSound()
        {
            sourceVoice.DestroyVoice();
            sourceVoice.Dispose();
            buffer.Stream.Dispose();
        }
    }
}
