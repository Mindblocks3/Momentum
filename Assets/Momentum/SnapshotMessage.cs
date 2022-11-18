using System;

namespace Mirage.Momentum
{
    public struct SnapshotMessage
    {
        public ushort SnapshotId ;
        public ushort BaselineId ;
        public double Time ;
        public ReadOnlyMemory<byte> Data ;
    }
}