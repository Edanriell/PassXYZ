using KPCLib;
using Microsoft.Extensions.Logging;
using User = PassXYZLib.User;

namespace PassXYZ.Vault.Services;

public class UserService : IUserService<User>
{
	private readonly IDataStore<Item> dataStore;
	private User? _user;
	private readonly ILogger<UserService> logger;

	public UserService(IDataStore<Item> dataStore, ILogger<UserService> logger)
	{
		this.dataStore = dataStore;
		this.logger = logger;
	}

	public User GetUser(string username)
	{
		var user = new User();
		user.Username = username;
		logger.LogDebug($"Path={user.Path}");
		return user;
	}

	public void Logout()
	{
		logger.LogDebug("Logout");
	}

	#pragma warning disable CS1998 // TODO: Remove after implementation
	public async Task UpdateUserAsync(User user)
	{
		throw new NotImplementedException();
	}

	public async Task DeleteUserAsync(User user)
	{
		throw new NotImplementedException();
	}

	public List<string> GetUsersList()
	{
		return User.GetUsersList();
	}

	public async Task AddUserAsync(User user)
	{
		throw new NotImplementedException();
	}

	public async Task<bool> LoginAsync(User user)
	{
		if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null");
		_user = user;

		return await dataStore.ConnectAsync(user);
	}
	#pragma warning restore CS1998 // TODO: Remove after implementation
}