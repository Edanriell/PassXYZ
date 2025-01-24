using Serilog;

namespace PassXYZ.Vault.Tests;

public class SerilogFixture : IDisposable
{
	public SerilogFixture()
	{
		Logger = new LoggerConfiguration()
		   .MinimumLevel.Debug()
		   .WriteTo.File(@"logs\xunit_log.txt")
		   .CreateLogger();

		Logger.Debug("SerilogFixture: initialized");
	}

	public ILogger Logger { get; }

	public void Dispose()
	{
		Logger.Debug("SerilogFixture: closed");
		Log.CloseAndFlush();
	}
}

[CollectionDefinition("Serilog collection")]
public class SerilogCollection : ICollectionFixture<SerilogFixture>
{
}