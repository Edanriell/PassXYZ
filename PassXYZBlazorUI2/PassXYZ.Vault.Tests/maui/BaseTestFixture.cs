using System.Globalization;

// By default, xUnit will run test collections (the tests in each class) in parallel
// with other test collections. Unfortunately, a _ton_ of the Controls legacy tests
// interact with properties on static classes (e.g., Application.Current), and if we 
// let them run in parallel they'll step on one another. So we tell xUnit to consider
// the whole assembly as a single collection for now, so all the tests run in sequence.
// (Hopefully in the future we can untangle some of the singletons and run these in parallel,
// because it'll be a lot faster.)
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

namespace Microsoft.Maui.Controls.Core.UnitTests;

public class BaseTestFixture : IDisposable
{
	private readonly CultureInfo _defaultCulture;
	private readonly CultureInfo _defaultUICulture;

	private bool _disposed;

	public BaseTestFixture()
	{
		//Microsoft.Maui.Controls.Hosting.CompatibilityCheck.UseCompatibility();
		_defaultCulture = Thread.CurrentThread.CurrentCulture;
		_defaultUICulture = Thread.CurrentThread.CurrentUICulture;
		//MockPlatformSizeService.Current?.Reset();
		//DispatcherProvider.SetCurrent(new DispatcherProviderStub());
		//DeviceDisplay.SetCurrent(null);
		//DeviceInfo.SetCurrent(null);
		//AppInfo.SetCurrent(null);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_disposed) return;

		if (disposing)
		{
			//MockPlatformSizeService.Current?.Reset();
			//AppInfo.SetCurrent(null);
			//DeviceDisplay.SetCurrent(null);
			//DeviceInfo.SetCurrent(null);
			Thread.CurrentThread.CurrentCulture = _defaultCulture;
			Thread.CurrentThread.CurrentUICulture = _defaultUICulture;
			DispatcherProvider.SetCurrent(null);
		}

		_disposed = true;
	}
}