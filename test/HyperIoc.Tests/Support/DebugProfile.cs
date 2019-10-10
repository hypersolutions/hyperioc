namespace HyperIoC.Tests.Support
{
    public class DebugProfile : FactoryProfile
    {
        public override void Construct(IFactoryBuilder builder)
        {
            builder.Add<ITestClass, TestClass>();
        }
    }
}