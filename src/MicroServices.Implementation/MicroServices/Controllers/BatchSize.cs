using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents the size of a batch of items to dequeue in a single operation.
    /// </summary>
    public readonly struct BatchSize : IEquatable<BatchSize>
    {
        /// <summary>
        /// The minimum size of a batch.
        /// </summary>
        public const int MinValue = 1;

        /// <summary>
        /// The default size of a batch.
        /// </summary>
        public const int DefaultValue = 50;

        private readonly int _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchSize" /> class.
        /// </summary>
        /// <param name="value">Size of the batch.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is smaller than <see cref="MinValue"/>.
        /// </exception>
        public BatchSize(int value)
        {
            if (value < MinValue)
            {
                throw NewBatchSizeTooSmallException(value);
            }
            _value = value - MinValue;
        }

        /// <inheritdoc />
        public override string ToString() =>
            ToInt32().ToString();

        #region [====== ToInt32 ======]

        /// <summary>
        /// Gets the int-value of this batch-size.
        /// </summary>
        /// <returns>The int-value of this batch-size.</returns>
        public int ToInt32() =>
            _value + MinValue;

        /// <summary>
        /// Implicitly converts the specified <paramref name="value"/> to an <see cref="int"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator int(BatchSize value) =>
            value.ToInt32();

        #endregion

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is BatchSize other && Equals(other);

        /// <inheritdoc />
        public bool Equals(BatchSize other) =>
            _value == other._value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            _value;

        #endregion

        private static Exception NewBatchSizeTooSmallException(int value)
        {
            var messageFormat = ExceptionMessages.BatchSize_BatchSizeTooSmall;
            var message = string.Format(messageFormat, value, MinValue);
            return new ArgumentOutOfRangeException(nameof(value), message);
        }
    }
}
