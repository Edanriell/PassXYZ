using System.Diagnostics;
using PassXYZLib;

namespace PassXYZ.Vault.Services;

/// <summary>
///     Current user who is a valid user. The name is stored in the preference. It should be a singleton instance.
/// </summary>
public class LoginService : PxUser
{
	private const string PrivacyNotice = "Privacy Notice";
	private readonly IUserService<User> _userService;

	public LoginService(IUserService<User> userService)
	{
		_userService = userService;
	}

	public static bool IsPrivacyNoticeAccepted
	{
		get => Preferences.Get(PrivacyNotice, false);

		set => Preferences.Set(PrivacyNotice, value);
	}

	public async Task<bool> LoginAsync()
	{
		return await _userService.LoginAsync(this);
	}

	public async Task SignUpAsync()
	{
		await _userService.AddUserAsync(this);
	}

	public override void Logout()
	{
		_userService.Logout();
	}

    /// <summary>
    ///     Get password in secure storage
    /// </summary>
    public async Task<string> GetSecurityAsync()
	{
		if (string.IsNullOrWhiteSpace(Username)) return string.Empty;

		var data = await SecureStorage.GetAsync(Username);
		return data;
	}

    /// <summary>
    ///     Store password in secure storage
    /// </summary>
    public async Task SetSecurityAsync(string password)
	{
		if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password)) return;

		await SecureStorage.SetAsync(Username, password);
	}

	public async Task<bool> DisableSecurityAsync()
	{
		if (string.IsNullOrWhiteSpace(Username)) return false;

		try
		{
			var data = await SecureStorage.GetAsync(Username);
			if (data != null) return SecureStorage.Remove(Username);

			return false;
		}
		catch (Exception ex)
		{
			// Possible that device doesn't support secure storage on device.
			Debug.WriteLine($"{ex}");
			return false;
		}
	}
}