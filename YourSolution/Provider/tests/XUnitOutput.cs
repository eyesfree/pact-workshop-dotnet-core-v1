using PactNet.Infrastructure.Outputters;
using Xunit.Abstractions;

namespace tests
{
    internal class XUnitOutput : IOutput
    {
        private ITestOutputHelper _testOutputHelper;

        public XUnitOutput(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void WriteLine(string line)
        {
            _testOutputHelper.WriteLine(line);
        }
    }
}