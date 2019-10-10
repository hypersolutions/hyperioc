namespace HyperIoC.Tests.Support
{
    public class AnyProfile : FactoryProfile
    {
        public override void Construct(IFactoryBuilder builder)
        {
            builder.Add<ITestConfig, TestConfig>();
        }
    }
}