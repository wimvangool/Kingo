using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MessageStreamTest
    {        
        #region [====== Empty - Properties & Indexer ======]

        [TestMethod]
        public void EmptyStream_Count_IsZero()
        {
            Assert.AreEqual(0, MessageStream.Empty.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void EmptyStream_Throws_IfIndexIsNegative()
        {
            MessageStream.Empty[-1].IgnoreValue();
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void EmptyStream_Throws_IfIndexIsZero()
        {
            MessageStream.Empty[0].IgnoreValue();
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void EmptyStream_Throws_IfIndexIsOne()
        {
            MessageStream.Empty[1].IgnoreValue();
        }

        #endregion

        #region [====== Empty - Append & Concat ======]

        [TestMethod]        
        public void EmptyStream_Append_ReturnsSelf_IfMessageIsNull()
        {
            Assert.AreEqual(MessageStream.Empty.Append(null as object).Count, 0);
        }

        [TestMethod]        
        public void EmptyStream_Append_ReturnsSelf_IfMessageCollectionIsNull()
        {
            Assert.AreEqual(MessageStream.Empty.Append(null as IEnumerable<object>).Count, 0);
        }

        [TestMethod]        
        public void EmptyStream_Append_Throws_IfMessageArrayIsNull()
        {
            Assert.AreEqual(MessageStream.Empty.Append(null).Count, 0);
        }

        [TestMethod]        
        public void EmptyStream_Concat_ReturnsSelf_IfStreamIsNull()
        {
            Assert.AreEqual(MessageStream.Empty.Concat(null as MessageStream).Count, 0);
        }

        [TestMethod]
        public void EmptyStream_Concat_ReturnsStream_IfStreamIsNotNull()
        {
            var stream = MessageStream.CreateStream(new object());

            Assert.AreSame(stream, MessageStream.Empty.Concat(stream));
        }

        #endregion

        #region [====== EmptyStream - HandleWith ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EmptyStream_HandleWithAsync_Throws_IfHandlerIsNull()
        {
            await MessageStream.Empty.HandleWithAsync(null);
        }

        [TestMethod]
        public async Task EmptyStream_HandleWithAsync_DoesNothing_IfHandlerIsNotNull()
        {
            var handler = new MessageHandlerSpy();

            await MessageStream.Empty.HandleWithAsync(handler);

            handler.AssertMessageCountIs(0);
        }

        #endregion

        #region [====== Append & Concat ======]

        [TestMethod]        
        public void Append_ReturnsSelf_IfMessageIsNull()
        {
            var streamA = CreateStream();
            var streamB = streamA.Append(null);

            Assert.AreSame(streamA, streamB);            
        }        

        [TestMethod]
        public void Concat_ReturnsSelf_ISpecifiedStreamIsEmpty()
        {
            var stream = CreateStream();

            Assert.AreSame(stream, stream + MessageStream.Empty);
        }        

        [TestMethod]
        public void Concat_ReturnsNewStream_IfSpecifiedStreamIsNotEmpty()
        {
            var streamA = CreateStream();
            var streamB = CreateStream(3);
            var streamC = streamA + streamB;

            Assert.IsNotNull(streamC);
            Assert.AreEqual(5, streamC.Count);
            Assert.AreSame(streamA[0], streamC[0]);
            Assert.AreSame(streamA[1], streamC[1]);
            Assert.AreSame(streamB[0], streamC[2]);
            Assert.AreSame(streamB[1], streamC[3]);
            Assert.AreSame(streamB[2], streamC[4]);
        }

        #endregion

        #region [====== HandleWithAsync ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandleWithAsync_Throws_IfHandlerIsNull()
        {
            await CreateStream().HandleWithAsync(null);
        }

        [TestMethod]
        public async Task HandleWithAsync_LetsHandlerVisitAllMessages_IfHandleIsNotNull()
        {
            var stream = CreateStream(4) + CreateStream(2, index => index + 1) + CreateStream(2, index => index.ToString());
            var handler = new MessageHandlerSpy();

            await stream.HandleWithAsync(handler);            

            handler.AssertMessageCountIs(8);
            handler.AssertVisitedAll(stream);
            handler.VerifyGenericTypeInvocations();
        }

        #endregion

        #region [====== Concat ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Concat_Throws_IfStreamsIsNull()
        {
            MessageStream.Concat(null as IEnumerable<MessageStream>);
        }

        [TestMethod]
        public void Concat_ReturnsEmptyStream_IfStreamsContainsOnlyNullElements()
        {
            var stream = MessageStream.Concat(new MessageStream[2]);

            Assert.IsNotNull(stream);
            Assert.AreEqual(0, stream.Count);
        }

        [TestMethod]
        public void Concat_ReturnsSpecifiedStream_IfStreamsContainsOnlyOneStream()
        {
            var stream = MessageStream.Concat(new[] { CreateStream() });

            Assert.IsNotNull(stream);
            Assert.AreEqual(2, stream.Count);
        }

        [TestMethod]
        public void Concat_ReturnsSpecifiedStream_IfStreamsContainsMultipleStreams()
        {
            var streamA = CreateStream();
            var streamB = CreateStream();
            var streamC = CreateStream();
            var streamD = MessageStream.Concat(new[]
            {
                streamA,
                null,
                streamB,
                null,
                streamC
            });

            Assert.IsNotNull(streamD);
            Assert.AreEqual(6, streamD.Count);

            Assert.AreSame(streamA[0], streamD[0]);
            Assert.AreSame(streamA[1], streamD[1]);

            Assert.AreSame(streamB[0], streamD[2]);
            Assert.AreSame(streamB[1], streamD[3]);

            Assert.AreSame(streamC[0], streamD[4]);
            Assert.AreSame(streamC[1], streamD[5]);
        }

        #endregion

        private static MessageStream CreateStream(int count = 2, Func<int, object> messageFactory = null)
        {
            var stream = MessageStream.Empty;

            for (int index = 0; index < count; index++)
            {
                stream = stream.Append(messageFactory?.Invoke(index) ?? new object());
            }
            return stream;
        }
    }
}
