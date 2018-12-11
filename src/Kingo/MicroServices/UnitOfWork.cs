using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a logical unit of work to which <see cref="IUnitOfWorkResourceManager">resource managers</see>
    /// enlist themselves and which controls their flush-moment.
    /// </summary>
    public abstract class UnitOfWork
    {
        #region [====== NoUnitOfWork ======]

        private sealed class NoUnitOfWork : UnitOfWork
        {                                  
            public override async Task EnlistAsync(IUnitOfWorkResourceManager resourceManager)
            {
                if (resourceManager == null)
                {
                    throw new ArgumentNullException(nameof(resourceManager));
                }
                if (resourceManager.RequiresFlush())
                {
                    await resourceManager.FlushAsync();
                }                
            }            

            internal override Task FlushAsync() =>
                Task.CompletedTask;

            internal override UnitOfWork Decorate() =>
                this;
        }       

        #endregion 
        
        /// <summary>
        /// Represents a unit of work that flushes resource managers the moment they enlist.
        /// </summary>
        public static readonly UnitOfWork None = new NoUnitOfWork();

        /// <summary>
        /// Enlists the specified <paramref name="resourceManager"/> with the context so that it can be flushed at the appropriate time.
        /// Note that this operation may flush the specified <paramref name="resourceManager"/> immediately.
        /// </summary>
        /// <param name="resourceManager">The resource manager to enlist.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resourceManager"/> is <c>null</c>.
        /// </exception>
        public abstract Task EnlistAsync(IUnitOfWorkResourceManager resourceManager);

        internal abstract Task FlushAsync();

        internal abstract UnitOfWork Decorate();
    }
}
