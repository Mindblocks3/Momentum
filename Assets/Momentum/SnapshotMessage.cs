using System;

namespace Mirage.Momentum
{
    public struct SnapshotMessage
    {
        public double Time ;
        public ushort SnapshotId ;
        public ushort Count;
        public Memory<ulong> Data ;
    }
}