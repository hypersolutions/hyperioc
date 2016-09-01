using HyperIoC;

namespace Tests.HyperIoC.Support
{
    public class ReleaseProfile : FactoryProfile
    {
        public override void Construct(IFactoryBuilder builder)
        {
            builder.Add<ITestClass, AnotherTestClass>();
        }
    }
}