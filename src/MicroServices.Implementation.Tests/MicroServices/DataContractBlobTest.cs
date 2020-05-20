using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class DataContractBlobTest
    {
        #region [====== FromStream ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromStream_Throws_IfContentTypeStringIsNull()
        {
            using (var content = CreateStream(0, true))
            {
                DataContractBlob.FromStream(null as string, content);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromStream_Throws_IfContentTypeIsNull()
        {
            using (var content = CreateStream(0, true))
            {
                DataContractBlob.FromStream(null as DataContractType, content);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromStream_Throws_IfStreamIsNull()
        {
            DataContractBlob.FromStream(_ContentType, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromStream_Throws_IfStreamIsNotReadable()
        {
            using (var content = CreateStream(0, false))
            {
                DataContractBlob.FromStream(_ContentType, content);
            }
        }

        #endregion

        #region [====== FromBytes ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromBytes_Throws_IfContentTypeStringIsNull()
        {
            DataContractBlob.FromBytes(null as string, Enumerable.Empty<byte>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromBytes_Throws_IfContentTypeIsNull()
        {
            DataContractBlob.FromBytes(null as DataContractType, Enumerable.Empty<byte>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromBytes_Throws_IfByteSequenceIsNull()
        {
            DataContractBlob.FromBytes(_ContentType, null as IEnumerable<byte>);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromBytes_Throws_IfByteArrayIsNull()
        {
            DataContractBlob.FromBytes(_ContentType, null);
        }

        [TestMethod]
        public void FromBytes_ReturnsExpectedBlob_IfByteSequenceIsEmpty()
        {
            var blob = DataContractBlob.FromBytes(_ContentType, Enumerable.Empty<byte>());

            Assert.IsNotNull(blob);
            Assert.IsNotNull(blob.ContentType);
            Assert.IsNotNull(blob.Content);
            Assert.AreEqual(0, blob.Content.Length);
            Assert.IsTrue(blob.Content.CanRead);
            Assert.IsFalse(blob.Content.CanWrite);
        }

        [TestMethod]
        public void FromBytes_ReturnsExpectedBlob_IfByteArrayLengthIsZero()
        {
            var blob = DataContractBlob.FromBytes(_ContentType, new byte[0]);

            Assert.IsNotNull(blob);
            Assert.IsNotNull(blob.ContentType);
            Assert.IsNotNull(blob.Content);
            Assert.AreEqual(0, blob.Content.Length);
            Assert.IsTrue(blob.Content.CanRead);
            Assert.IsFalse(blob.Content.CanWrite);
        }

        [TestMethod]
        public void FromBytes_ReturnsExpectedBlob_IfByteArrayLengthIsGreaterThanZero()
        {
            var contentLength = DateTimeOffset.UtcNow.Millisecond + 1;
            var blob = DataContractBlob.FromBytes(_ContentType, new byte[contentLength]);

            Assert.IsNotNull(blob);
            Assert.IsNotNull(blob.ContentType);
            Assert.IsNotNull(blob.Content);
            Assert.AreEqual(contentLength, blob.Content.Length);
            Assert.IsTrue(blob.Content.CanRead);
            Assert.IsFalse(blob.Content.CanWrite);
        }

        [TestMethod]
        public void FromBytes_ReturnsImmutableBlob_IfByteArrayLengthIsGreaterThanZero()
        {
            var contentLength = DateTimeOffset.UtcNow.Millisecond + 1;
            var content = new byte[contentLength];
            var blobA = DataContractBlob.FromBytes(_ContentType, content);

            content[0] = 1;

            var blobB = DataContractBlob.FromBytes(_ContentType, content);

            Assert.AreEqual(contentLength, blobA.Content.Length);
            Assert.AreEqual(contentLength, blobB.Content.Length);
            Assert.AreNotEqual(blobA, blobB);
        }

        #endregion

        #region [====== Stubs ======]

        private sealed class StreamDecorator : Stream
        {
            private readonly Stream _stream;
            private readonly bool _canRead;

            public StreamDecorator(Stream stream, bool canRead = true)
            {
                _stream = stream;
                _canRead = canRead;
            }

            public override bool CanRead =>
                _canRead;

            public override bool CanSeek =>
                _stream.CanSeek;

            public override bool CanWrite =>
                _stream.CanWrite;

            public override long Length =>
                _stream.Length;

            public override long Position
            {
                get => _stream.Position;
                set => _stream.Position = value;
            }

            public override void Flush() =>
                _stream.Flush();

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (_canRead)
                {
                    return _stream.Read(buffer, offset, count);
                }
                throw NewNotSupportedException(nameof(Read));
            }

            public override long Seek(long offset, SeekOrigin origin) =>
                _stream.Seek(offset, origin);

            public override void SetLength(long value) =>
                _stream.SetLength(value);

            public override void Write(byte[] buffer, int offset, int count) =>
                _stream.Write(buffer, offset, count);

            private static Exception NewNotSupportedException(string name) =>
                new NotSupportedException($"Operation '{name}' is not supported.");
        }

        private static readonly DataContractType _ContentType = DataContractType.FromName("SomeType");

        private static Stream CreateStream(int length, bool canRead) =>
            new StreamDecorator(new MemoryStream(new byte[length], false), canRead);

        #endregion
    }
}
