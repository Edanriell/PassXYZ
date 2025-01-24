using System.Diagnostics;
using System.Reflection;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views;

public partial class AboutPage : ContentPage
{
	private readonly AboutViewModel _viewModel;

	public AboutPage(AboutViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		DatabaseName.Text = _viewModel.GetStoreName();

		var localTime = _viewModel.GetStoreModifiedTime().ToLocalTime();
		LastModifiedDate.Text = localTime.ToLongDateString();
		LastModifiedTime.Text = localTime.ToLongTimeString();

		var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
		var version = Properties.Resources.Version + " " + (assemblyVersion != null ? assemblyVersion.ToString() : "");
		#if DEBUG
		version = version + " (Debug)";
		#endif
		AppVersion.Text = version;
		Debug.WriteLine($"Version: {version}");
	}
}