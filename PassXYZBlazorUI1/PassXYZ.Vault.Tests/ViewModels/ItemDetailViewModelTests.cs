using KPCLib;
using Microsoft.Extensions.Logging;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Tests.ViewModels;

public class ItemDetailViewModelTests
{
	private readonly IDataStore<Item> dataStore;
	private readonly ILogger<ItemDetailViewModel> logger;

	public ItemDetailViewModelTests()
	{
		using var loggerFactory = LoggerFactory.Create(builder =>
			builder.AddConsole()
			   .SetMinimumLevel(LogLevel.Debug));
		logger = loggerFactory.CreateLogger<ItemDetailViewModel>();
		dataStore = new DataStore();
	}

	[Fact]
	public async void LoadItemIdTests()
	{
		// Arrange
		ItemDetailViewModel vm = new(dataStore, logger);
		var items = await dataStore.GetItemsAsync(true);
		foreach (var item in items)
		{
			// Act
			if (item == null) throw new NullReferenceException(nameof(item));
			;
			#pragma warning disable CS8604 // Possible null reference argument.
			vm.LoadItemId(item.Id);
			#pragma warning restore CS8604 // Possible null reference argument.
			// Assert
			Assert.Equal(item.Name, vm.Title);
		}
	}

	[Fact]
	public async void SetItemIdTests()
	{
		// Arrange
		ItemDetailViewModel vm = new(dataStore, logger);
		var items = await dataStore.GetItemsAsync(true);
		foreach (var item in items)
		{
			// Act
			vm.ItemId = item.Id;
			// Assert
			Assert.Equal(item.Name, vm.Title);
		}
	}
}