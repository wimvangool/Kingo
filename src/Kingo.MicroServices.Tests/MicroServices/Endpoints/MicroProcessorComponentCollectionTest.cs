using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Endpoints
{
    [TestClass]
    public sealed partial class MicroProcessorComponentCollectionTest
    {
        private readonly MicroProcessorComponentCollection _components;

        public MicroProcessorComponentCollectionTest()
        {
            _components = new MicroProcessorComponentCollection();
        }

        private IServiceProvider BuildServiceProvider() =>
            BuildServiceCollection().BuildServiceProvider();

        private IServiceCollection BuildServiceCollection() =>
            (_components as IServiceCollectionBuilder).BuildServiceCollection();
    }
}
