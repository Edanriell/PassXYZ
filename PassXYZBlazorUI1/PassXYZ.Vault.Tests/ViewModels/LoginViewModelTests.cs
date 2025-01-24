using KPCLib;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Core.UnitTests;
using NSubstitute;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Tests.ViewModels;

public class LoginViewModelTests : ShellTestBase
{
	private readonly IDataStore<Item> dataStore;
	private readonly Application app;
	private ILogger<LoginViewModel> loginViewModelLogger;
	private UserService userService;

	public LoginViewModelTests()
	{
		var shell = new TestShell();
		var abougPage = new ShellItem { Route = "AboutPage" };
		var page = MakeSimpleShellSection("Readme", "content");
		abougPage.Items.Add(page);
		shell.Items.Add(abougPage);
		app = Substitute.For<Application>();
		app.MainPage = shell;

		using var loggerFactory = LoggerFactory.Create(builder =>
			builder.AddDebug()
			   .AddConsole()
			   .SetMinimumLevel(LogLevel.Debug));
		var logger = loggerFactory.CreateLogger<UserService>();
		loginViewModelLogger = loggerFactory.CreateLogger<LoginViewModel>();
		dataStore = new DataStore();
		userService = new UserService(dataStore, logger);
	}

	[Fact]
	public void LoginCommandTest()
	{
		// TODO: Need to fix this.
		//var loginservice = new LoginService(userService);
		//LoginViewModel vm = new(loginservice, loginViewModelLogger);
		//vm.Username = "test1";
		//vm.Password = "12345";
		//vm.LoginCommand.Execute(null);
	}
}