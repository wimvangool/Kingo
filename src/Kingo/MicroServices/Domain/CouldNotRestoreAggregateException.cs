using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// This exception is thrown by a <see cref="Repository{TKey,TVersion,TAggregate}"/> when it fails
    /// to restore an aggregate from a retrieved <see cref="AggregateDataSet" />.
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
        public CouldNotRestoreAggregateException(AggregateDataSet dataSet, string message, Exception innerException = null) :
            base(message, innerException)
        {
            DataSet = dataSet ?? throw new ArgumentNullException(nameof(dataSet));
        }

        /// <inheritdoc />
        protected CouldNotRestoreAggregateException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            DataSet = (AggregateDataSet) info.GetValue(nameof(DataSet), typeof(AggregateDataSet));
        }        

        /// <summary>
        /// The data-set that was retrieved.
        /// </summary>
        public AggregateDataSet DataSet
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
