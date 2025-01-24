using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PassXYZ.Vault.Services;

namespace PassXYZ.Vault.ViewModels;

public partial class AboutViewModel : ObservableObject
{
	private readonly LoginService _currentUser;
	private ILogger<AboutViewModel> _logger;

	[ObservableProperty] private string? title = Properties.Resources.About;

	public AboutViewModel(LoginService user, ILogger<AboutViewModel> logger)
	{
		_currentUser = user;
		_logger = logger;
	}

	[RelayCommand]
	private async Task OpenWeb()
	{
		await Browser.OpenAsync(Properties.Resources.about_url);
	}

	public string GetStoreName()
	{
		return _currentUser.Username;
	}

	public DateTime GetStoreModifiedTime()
	{
		return DateTime.Now;
	}
}