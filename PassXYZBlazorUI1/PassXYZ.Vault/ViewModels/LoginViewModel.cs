using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PassXYZ.Vault.Services;
using PassXYZLib;

namespace PassXYZ.Vault.ViewModels;

public partial class LoginViewModel : ObservableObject
{
	private readonly LoginService _currentUser;
	private readonly ILogger<LoginViewModel> _logger;

	[ObservableProperty] private bool isBusy;

	[ObservableProperty] [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
	private string? password;

	[ObservableProperty] [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
	private string? username;

	public LoginViewModel(LoginService user, ILogger<LoginViewModel> logger)
	{
		_currentUser = user;
		_logger = logger;
	}

	[RelayCommand(CanExecute = nameof(ValidateLogin))]
	private async Task Login(object obj)
	{
		try
		{
			IsBusy = true;

			if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Username))
			{
				await Shell.Current.DisplayAlert("", Properties.Resources.settings_empty_password,
					Properties.Resources.alert_id_ok);
				IsBusy = false;
				return;
			}

			_currentUser.Username = Username;
			_currentUser.Password = Password;
			var status = await _currentUser.LoginAsync();

			if (status)
			{
				if (AppShell.CurrentAppShell != null)
				{
					AppShell.CurrentAppShell.SetRootPageTitle(Username);

					await Shell.Current.GoToAsync("//RootPage");
				}
				else
				{
					throw new NullReferenceException("CurrentAppShell is null");
				}
			}

			IsBusy = false;
		}
		catch (Exception ex)
		{
			IsBusy = false;
			var msg = ex.Message;
			if (ex is IOException ioException)
			{
				_logger.LogError("Login error, need to recover");
				msg = Properties.Resources.message_id_recover_datafile;
			}

			await Shell.Current.DisplayAlert(Properties.Resources.LoginErrorMessage, msg,
				Properties.Resources.alert_id_ok);
		}
	}

	private bool ValidateLogin()
	{
		var canExecute = !string.IsNullOrWhiteSpace(Username)
					  && !string.IsNullOrWhiteSpace(Password);
		return canExecute;
	}

	public List<string> GetUsersList()
	{
		return User.GetUsersList();
	}
}