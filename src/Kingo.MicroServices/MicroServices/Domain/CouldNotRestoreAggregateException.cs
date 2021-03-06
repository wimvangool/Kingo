﻿using System;
using System.Runtime.Serialization;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// This exception is thrown by a <see cref="Repository{TKey,TVersion,TAggregate}"/> when it fails
    /// to restore an aggregate from a retrieved <see cref="AggregateReadSet" />.
    /// </summary>
    [Serializable]
    public class CouldNotRestoreAggregateException : InternalServerErrorException
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotRestoreAggregateException" /> class.        
        /// </summary>
        /// <param name="dataSet">/// The data-set that was retrieved.</param>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">An inner-exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dataSet"/> is c<c>null</c>.
        /// </exception>
        public CouldNotRestoreAggregateException(AggregateReadSet dataSet, string message, Exception innerException = null) :
            base(message, innerException)
        {
            DataSet = dataSet ?? throw new ArgumentNullException(nameof(dataSet));
        }

        /// <inheritdoc />
        protected CouldNotRestoreAggregateException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            DataSet = (AggregateReadSet) info.GetValue(nameof(DataSet), typeof(AggregateReadSet));
        }        

        /// <summary>
        /// The data-set that was retrieved.
        /// </summary>
        public AggregateReadSet DataSet
        {
            get;            
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(DataSet), DataSet);
        }
    }
}
