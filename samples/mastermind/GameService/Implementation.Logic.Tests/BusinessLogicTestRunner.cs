using System;
using System.Collections.Generic;
using System.Text;
using Kingo.MicroServices.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MasterMind.GameService
{
    /// <summary>
    /// Serves as a base-class for all test-runners that are designed to test specific business logic
    /// of this service.
    /// </summary>
    [TestClass]
    public abstract class BusinessLogicTestRunner : MicroProcessorOperationTestRunner
    {
        /// <summary>
        /// Adds the MicroProcessor that will be used to run all tests and all other dependencies to the specified
        /// <see cref="services"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        protected override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services.AddMicroProcessor(processor =>
            {
                processor.MessageHandlers.Add("*.Application.dll");
            });
        }
    }
}
