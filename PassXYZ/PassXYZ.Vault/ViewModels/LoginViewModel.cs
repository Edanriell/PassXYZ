using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PassXYZ.Vault.Services;
using PassXYZLib;
using Plugin.Fingerprint.Abstractions;

namespace PassXYZ.Vault.ViewModels;

public partial class LoginViewModel : ObservableObject
{
	private readonly IFingerprint _fingerprint;
	private readonly LoginService _currentUser;
	private readonly ILogger<LoginViewModel> _logger;

	[ObservableProperty] private bool isBusy;

	[ObservableProperty] private bool isFingerprintAvailable;

	[ObservableProperty] private bool isFingerprintEnabled;

	private string? password;

	[ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
	private string? password2;

	private string? username;

	public LoginViewModel(LoginService user, ILogger<LoginViewModel> logger, IFingerprint fingerprint)
	{
		_currentUser = user;
		_logger = logger;
		_fingerprint = fingerprint;
	}

	public string? Username
	{
		get => username;
		set
		{
			if (SetProperty(ref username, value))
			{
				_currentUser.Username = value;
				LoginCommand.NotifyCanExecuteChanged();
				SignUpCommand.NotifyCanExecuteChanged();
				FingerprintLoginCommand.NotifyCanExecuteChanged();
			}
		}
	}

	public string? Password
	{
		get => password;
		set
		{
			if (SetProperty(ref password, value))
			{
				_currentUser.Password = value;
				LoginCommand.NotifyCanExecuteChanged();
				SignUpCommand.NotifyCanExecuteChanged();
			}
		}
	}

	public bool IsDeviceLockEnabled
	{
		get => _currentUser.IsDeviceLockEnabled;

		set => _currentUser.IsDeviceLockEnabled = value;
	}

	[RelayCommand(CanExecute = nameof(ValidateLogin))]
	private async Task Login()
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
			_logger.LogDebug("data path: {path}", _currentUser.Path);
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

	[RelayCommand(CanExecute = nameof(ValidateSignUp))]
	private async Task SignUp()
	{
		try
		{
			IsBusy = true;

			if (string.IsNullOrWhiteSpace(Password2) || string.IsNullOrWhiteSpace(Password) ||
				string.IsNullOrWhiteSpace(Username))
			{
				await Shell.Current.DisplayAlert("", Properties.Resources.settings_empty_password,
					Properties.Resources.alert_id_ok);
				IsBusy = false;
				return;
			}

			_currentUser.Username = Username;
			_currentUser.Password = Password;

			if (_currentUser.IsUserExist)
			{
				await Shell.Current.DisplayAlert(Properties.Resources.SignUpPageTitle,
					Properties.Resources.SignUpErrorMessage1, Properties.Resources.alert_id_ok);
				IsBusy = false;
				return;
			}

			await _currentUser.SignUpAsync();
			await Shell.Current.DisplayAlert(Properties.Resources.SignUpPageTitle, Properties.Resources.SiguUpMessage,
				Properties.Resources.alert_id_ok);
			Username = "";
			Password = "";
			Password2 = "";
			IsBusy = false;
		}
		catch (Exception ex)
		{
			await Shell.Current.DisplayAlert(Properties.Resources.SignUpPageTitle, ex.Message,
				Properties.Resources.alert_id_ok);
		}

		Debug.WriteLine(
			$"LoginViewModel: OnSignUpClicked {_currentUser.Username}, DeviceLock: {_currentUser.IsDeviceLockEnabled}");
	}

	private bool ValidateSignUp()
	{
		var canExecute = !string.IsNullOrWhiteSpace(Username)
					  && !string.IsNullOrWhiteSpace(Password)
					  && !string.IsNullOrWhiteSpace(Password2);

		if (canExecute) return Password!.Equals(Password2);

		return canExecute;
	}

	[RelayCommand(CanExecute = nameof(ValidateFingerprintLogin))]
	private async Task FingerprintLogin()
	{
		var cancel = new CancellationTokenSource();
		var dialogConfig = new AuthenticationRequestConfiguration(Username,
							   Properties.Resources.fingerprint_login_message)
						   {
							   CancelTitle = "Cancel fingerprint login",
							   FallbackTitle = "Use Password",
							   AllowAlternativeAuthentication = true
						   };

		var result = await _fingerprint.AuthenticateAsync(dialogConfig, cancel.Token);

		if (result.Authenticated)
		{
			// Username cannot be null when FingerprintLogin is invokved
			Password = await _currentUser.GetSecurityAsync();
			if (!string.IsNullOrWhiteSpace(Password))
				await Login();
			else
				_logger.LogWarning("GetSecurityAsync() error.");
		}
		else
		{
			_logger.LogWarning("Failed to login with fingerprint.");
		}
	}

	private bool ValidateFingerprintLogin()
	{
		CheckFingerprintStatus();
		return !string.IsNullOrWhiteSpace(Username);
	}

	public async void CheckFingerprintStatus()
	{
		_currentUser.Username = Username;
		var password = await _currentUser.GetSecurityAsync();
		IsFingerprintAvailable = await _fingerprint.IsAvailableAsync();
		IsFingerprintEnabled = IsFingerprintAvailable && !string.IsNullOrWhiteSpace(password);
	}

	private bool CheckDeviceLock()
	{
		User user = new()
					{
						Username = Username
					};

		if (user.IsUserExist)
		{
			// This is important, since we need to reset device lock status based on existing file.
			_currentUser.IsDeviceLockEnabled = user.IsDeviceLockEnabled;
			return !_currentUser.IsKeyFileExist && _currentUser.IsDeviceLockEnabled;
		}

		return false;
	}

	public List<string> GetUsersList()
	{
		return User.GetUsersList();
	}

	public void Logout()
	{
		_currentUser.Logout();
	}

	public async Task<bool> AuthenticateAsync(string reason, string? cancel = null, string? fallback = null,
											  string? tooFast = null)
	{
		CancellationTokenSource cancelToken;

		cancelToken = new CancellationTokenSource();

		var dialogConfig = new AuthenticationRequestConfiguration("Verify your fingerprint", reason)
						   {
							   // all optional
							   CancelTitle = cancel,
							   FallbackTitle = fallback,
							   AllowAlternativeAuthentication = false
						   };

		// optional
		dialogConfig.HelpTexts.MovedTooFast = tooFast;

		var result = await _fingerprint.AuthenticateAsync(dialogConfig, cancelToken.Token);

		return result.Authenticated;
	}
}