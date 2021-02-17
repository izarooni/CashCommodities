﻿using System;
using NAudio.Wave;

namespace NAudioTests.Utils
{
    class NullWaveStream : WaveStream
    {
        private readonly long length;
        private long position;
        
        public NullWaveStream(WaveFormat format, long length)
        {
            WaveFormat = format;
            this.length = length;
        }

        public override WaveFormat WaveFormat { get; }

        public override long Length => length;

        public override long Position
        {
            get { return position; }
            set { position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (position > length)
            {
                return 0;
            }
            count = (int)Math.Min(count, length - position);
            Array.Clear(buffer, offset, count);
            position += count;
            return count;
        }
    }
}
