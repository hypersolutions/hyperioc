using HyperIoC;

namespace Tests.HyperIoC.Support
{
    public class DebugProfile : FactoryProfile
    {
        public override void Construct(IFactoryBuilder builder)
        {
            builder.Add<ITestClass, TestClass>();
        }
    }
}