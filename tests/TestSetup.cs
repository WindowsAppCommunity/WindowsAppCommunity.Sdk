using OwlCore.Diagnostics;

namespace WindowsAppCommunity.Sdk.Tests;

[TestClass]
public class TestSetup
{
    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        // Set up the logger for the tests.
        Logger.MessageReceived += TestSetupHelpers.LoggerOnMessageReceived;
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        // Clean up the logger for the tests.
        Logger.MessageReceived -= TestSetupHelpers.LoggerOnMessageReceived;
    }
}
