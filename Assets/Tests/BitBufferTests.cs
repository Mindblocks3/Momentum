using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Mirage.Momentum
{
    public class BitBufferTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void BitBufferTestsSimplePasses()
        {
            BitBuffer buffer = new BitBuffer(1500);
            buffer.Write(1, 16);

            BitBuffer reader = new BitBuffer(buffer.ToMemory());
            Assert.AreEqual(1, reader.ReadUShort());
        }
        // write a bunch of booleans
        [Test]
        public void WritingBooleans()
        {
            BitBuffer buffer = new BitBuffer(1500);
            buffer.WriteBoolean(false);
            buffer.WriteBoolean(true);
            buffer.WriteBoolean(false);
            buffer.WriteBoolean(false);
            buffer.WriteBoolean(false);
            buffer.WriteBoolean(true);
            buffer.WriteBoolean(true);

            BitBuffer reader = new BitBuffer(buffer.ToMemory());
            Assert.AreEqual(false, reader.ReadBool());
            Assert.AreEqual(true, reader.ReadBool());
            Assert.AreEqual(false, reader.ReadBool());
            Assert.AreEqual(false, reader.ReadBool());
            Assert.AreEqual(false, reader.ReadBool());
            Assert.AreEqual(true, reader.ReadBool());
            Assert.AreEqual(true, reader.ReadBool());

        }

        [Test]
        public void WritingVector3()
        {
            Vector3 v3 = new Vector3(50, 20, 15);

            BitBuffer buffer = new BitBuffer(1500);
            buffer.WriteVector3(v3);

            BitBuffer reader = new BitBuffer(buffer.ToMemory());

            Assert.AreEqual(v3, reader.ReadVector3());

        }

        [Test]
        public void WriteRandomValues()
        {
            int[] samples = new int[1000];
            BitBuffer buffer = new BitBuffer(1500);
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = Random.Range(0, 1000);
                buffer.Write(samples[i], 10);
            }

            BitBuffer reader = new BitBuffer(buffer.ToMemory());
            for (int i = 0; i < samples.Length; i++)
            {
                Assert.AreEqual(samples[i], reader.ReadUInt32(10));
            }
        }

    }
}