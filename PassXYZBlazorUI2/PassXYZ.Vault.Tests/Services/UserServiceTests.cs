using KPCLib;
using Microsoft.Extensions.Logging;
using PassXYZ.Vault.Services;
using User = PassXYZLib.User;

namespace PassXYZ.Vault.Tests.Services;

public class UserServiceTests
{
	private readonly IDataStore<Item> dataStore;
	private readonly ILogger<UserService> logger;

	public UserServiceTests()
	{
		using var loggerFactory = LoggerFactory.Create(builder =>
			builder.AddDebug()
			   .AddConsole()
			   .SetMinimumLevel(LogLevel.Debug));
		logger = loggerFactory.CreateLogger<UserService>();
		dataStore = new DataStore();
	}

	[Fact]
	public void GetUserTest()
	{
		var userService = new UserService(dataStore, logger);
		var user = userService.GetUser("test");
	}

	[Fact]
	public void GetUsersListTest()
	{
		var userService = new UserService(dataStore, logger);
		var list = userService.GetUsersList();
	}

	[Fact]
	public void AddUserTest()
	{
		var userService = new UserService(dataStore, logger);
		var user = new User();
		user.Username = "new user 1";

		_ = userService.AddUserAsync(user);
	}

	[Fact]
	public void RemoveUserTest()
	{
		var userService = new UserService(dataStore, logger);
		var user = new User();
		user.Username = "new user 1";

		_ = userService.AddUserAsync(user);
		_ = userService.DeleteUserAsync(user);
	}

	[Fact]
	public void UpdateUserTest()
	{
		var userService = new UserService(dataStore, logger);
		var user = new User();
		user.Username = "new user 1";

		_ = userService.AddUserAsync(user);
		_ = userService.UpdateUserAsync(user);
	}

	[Fact]
	public void LoginAsyncTest()
	{
		var userService = new UserService(dataStore, logger);
		var user = new User();
		user.Username = "new user 1";

		_ = userService.LoginAsync(user);
	}

	[Fact]
	public void LogoutTest()
	{
		var userService = new UserService(dataStore, logger);
		userService.Logout();
	}
}