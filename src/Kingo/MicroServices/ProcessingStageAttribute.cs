using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base class for all filter-attributes that are executed during the processing stage
    /// of the microprocessor pipeline.
    /// </summary>
    public abstract class ProcessingStageAttribute : MicroProcessorFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingStageAttribute" /> class.
        /// </summary>
        protected ProcessingStageAttribute() :
            base(MicroProcessorFilterStage.ProcessingStage) { }
    }
}
