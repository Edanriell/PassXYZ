using System.Diagnostics;
using KPCLib;
using PassXYZ.Vault.Services;
using PassXYZLib;

namespace PassXYZ.Vault.Tests;

[Collection("Serilog collection")]
public class DataStoreTests
{
	private readonly IDataStore<Item> datastore;
	private readonly SerilogFixture serilogFixture;

	public DataStoreTests(SerilogFixture fixture)
	{
		datastore = new MockDataStore();
		serilogFixture = fixture;
		serilogFixture.Logger.Debug("DataStoreTests initialized");
	}

	[Fact]
	public async void Add_Item()
	{
		// Arrange
		var itemSubType = ItemSubType.Entry;

		// Act
		var newItem = datastore.CreateNewItem(itemSubType);
		if (newItem == null) throw new NullReferenceException("newItem is null");
		newItem.Name = $"{itemSubType.ToString()}01";
		await datastore.AddItemAsync(newItem);
		var item = datastore.GetItem(newItem.Id);

		// Assert
		if (item == null) throw new NullReferenceException("item is null");
		Assert.Equal(newItem.Id, item.Id);
		serilogFixture.Logger.Debug("Add_Item done");
	}

	[Theory]
	[InlineData(ItemSubType.Entry)]
	[InlineData(ItemSubType.Group)]
	[InlineData(ItemSubType.Notes)]
	[InlineData(ItemSubType.PxEntry)]
	public async void Delete_Item(ItemSubType itemSubType)
	{
		// Arrange
		var newItem = datastore.CreateNewItem(itemSubType);
		if (newItem == null) throw new NullReferenceException("newItem is null");
		newItem.Name = $"{itemSubType.ToString()}01";
		await datastore.AddItemAsync(newItem);

		// Act
		var result = await datastore.DeleteItemAsync(newItem.Id);
		Debug.WriteLine($"Delete_Item: {newItem.Name}");

		// Assert
		Assert.True(result);
		serilogFixture.Logger.Debug("Delete_Item: {Name}", newItem.Name);
	}

	[Theory]
	[InlineData(ItemSubType.Entry)]
	[InlineData(ItemSubType.Group)]
	[InlineData(ItemSubType.Notes)]
	[InlineData(ItemSubType.PxEntry)]
	public void Create_Item(ItemSubType itemSubType)
	{
		var item = datastore.CreateNewItem(itemSubType);
		if (item == null) throw new NullReferenceException("item is null");
		item.Name = itemSubType.ToString();
		Debug.WriteLine($"Create_Item: {item.Name}");
		serilogFixture.Logger.Debug("Create_Item: {Name}", item.Name);

		Assert.NotNull(item);
	}
}