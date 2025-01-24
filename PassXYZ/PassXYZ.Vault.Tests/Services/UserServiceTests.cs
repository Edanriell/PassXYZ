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
	public async Task RemoveUserTestAsync()
	{
		var userService = new UserService(dataStore, logger);
		var user = new User();
		user.Username = "new user 1";

		await userService.AddUserAsync(user);
		Assert.True(user.IsUserExist);
		await userService.DeleteUserAsync(user);
		Assert.False(user.IsUserExist);
	}

	[Fact]
	public async Task LoginAsyncTest()
	{
		var userService = new UserService(dataStore, logger);
		var user = new User();
		user.Username = "new user 1";
		user.Password = "12345";

		await userService.AddUserAsync(user);
		Assert.True(user.IsUserExist);
		await userService.LoginAsync(user);
		await userService.DeleteUserAsync(user);
		Assert.False(user.IsUserExist);
	}

	[Fact]
	public async Task LogoutTest()
	{
		var userService = new UserService(dataStore, logger);
		var user = new User();
		user.Username = "new user 1";
		user.Password = "12345";

		await userService.AddUserAsync(user);
		Assert.True(user.IsUserExist);
		await userService.LoginAsync(user);
		userService.Logout();
		await userService.DeleteUserAsync(user);
		Assert.False(user.IsUserExist);
	}
}