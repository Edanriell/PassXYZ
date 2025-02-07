﻿using KPCLib;
using PassXYZ.Vault.Services;
using PassXYZLib;

namespace PassXYZ.Vault.Tests.Services;

[CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
public class NonParallelCollectionDefinitionClass
{
}

[Collection("Non-Parallel Collection")]
public class IDataStoreTest
{
	private readonly IDataStore<Item> dataStore;
	private readonly User _user;

	public IDataStoreTest()
	{
		dataStore = new DataStore();
		_user = new User
				{
					Username = "test1",
					Password = "12345"
				};
	}

	[Fact]
	public async void GetItemsAsyncTest()
	{
		var result = await dataStore.ConnectAsync(_user);
		Assert.True(result);
		dataStore.SetCurrentGroup();

		var items = await dataStore.GetItemsAsync(true);
		Assert.NotEmpty(items);
	}

	[Fact]
	public async void AddGroupAsyncTest()
	{
		var result = await dataStore.ConnectAsync(_user);
		Assert.True(result);
		dataStore.SetCurrentGroup();

		Item newItem = new PxGroup
					   {
						   Name = "New Group 1",
						   Notes = "This is a new group."
					   };
		result = await dataStore.AddItemAsync(newItem);
		Assert.True(result);
	}

	[Fact]
	public async void AddEntryAsyncTest()
	{
		var result = await dataStore.ConnectAsync(_user);
		Assert.True(result);
		dataStore.SetCurrentGroup();

		Item newItem = new PxEntry
					   {
						   Name = "New Entry 1",
						   Notes = "This is a new entry."
					   };
		result = await dataStore.AddItemAsync(newItem);
		Assert.True(result);
	}

	[Fact]
	public async void AddItemAsyncFailureTest()
	{
		var result = false;
		#pragma warning disable CS8625 // Possible null reference argument.
		var ex = await Assert.ThrowsAsync<ArgumentNullException>(
					 async () => result = await dataStore.AddItemAsync(null));
		#pragma warning restore CS8625 // Possible null reference argument.
		Assert.Equal("Value cannot be null. (Parameter 'item')", ex.Message);
	}

	[Fact]
	public async void UpdateItemAsyncTest()
	{
		var result = await dataStore.ConnectAsync(_user);
		Assert.True(result);
		dataStore.SetCurrentGroup();

		Item newItem = new PxEntry
					   {
						   Name = "New item 1",
						   Notes = "This is a new item."
					   };
		result = await dataStore.AddItemAsync(newItem);
		Assert.True(result);
		newItem.Name = "Updated item 1";
		result = await dataStore.UpdateItemAsync(newItem);
		Assert.True(result);
	}

	[Fact]
	public async void UpdateNullItemAsyncTest()
	{
		var result = false;
		#pragma warning disable CS8625 // Possible null reference argument.
		var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
					 result = await dataStore.UpdateItemAsync(null));
		#pragma warning restore CS8625 // Possible null reference argument.
		Assert.Equal("Value cannot be null. (Parameter 'item')", ex.Message);
	}

	[Fact]
	public async void UpdateNoExistItemAsyncTest()
	{
		var result = await dataStore.ConnectAsync(_user);
		Assert.True(result);
		dataStore.SetCurrentGroup();

		Item newItem = new NewItem
					   {
						   Name = "No item 1",
						   Notes = "You cannot find this item."
					   };
		result = await dataStore.UpdateItemAsync(newItem);
		Assert.False(result);
	}

	[Fact]
	public async void DeleteItemAsyncTest()
	{
		var result = await dataStore.ConnectAsync(_user);
		Assert.True(result);
		dataStore.SetCurrentGroup();

		Item newItem = new PxEntry
					   {
						   Name = "New item 1",
						   Notes = "Please delete it."
					   };
		result = await dataStore.AddItemAsync(newItem);
		Assert.True(result);
		result = await dataStore.DeleteItemAsync(newItem.Id);
		Assert.True(result);
	}

	[Fact]
	public async void DeleteNullItemAsyncTest()
	{
		var result = false;
		#pragma warning disable CS8625 // Possible null reference argument.
		var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
					 result = await dataStore.DeleteItemAsync(null));
		#pragma warning restore CS8625 // Possible null reference argument.
		Assert.Equal("Value cannot be null. (Parameter 'id')", ex.Message);
	}

	[Fact]
	public async void DeleteNoExistItemAsyncTest()
	{
		var result = await dataStore.ConnectAsync(_user);
		Assert.True(result);
		dataStore.SetCurrentGroup();

		Item newItem = new PxEntry
					   {
						   Name = "No item 1",
						   Notes = "You cannot find this item."
					   };
		result = await dataStore.DeleteItemAsync(newItem.Id);
		Assert.False(result);
	}

	[Fact]
	public async void GetItemAsyncTest()
	{
		var result = await dataStore.ConnectAsync(_user);
		Assert.True(result);
		dataStore.SetCurrentGroup();

		Item newItem = new PxEntry
					   {
						   Name = "New item 1",
						   Notes = "This is a new item."
					   };
		result = await dataStore.AddItemAsync(newItem);
		Assert.True(result);
		var item = dataStore.GetItem(newItem.Id);
		Assert.NotNull(item);
		Assert.Equal(newItem.Name, item.Name);
	}

	[Fact]
	public async void GetNullItemAsyncTest()
	{
		var result = await dataStore.ConnectAsync(_user);
		Assert.True(result);
		dataStore.SetCurrentGroup();

		#pragma warning disable CS8625 // Possible null reference argument.
		var ex = Assert.Throws<ArgumentNullException>(() => dataStore.GetItem(null));
		#pragma warning restore CS8625 // Possible null reference argument.
		Assert.Equal("Value cannot be null. (Parameter 'id')", ex.Message);
	}

	[Fact]
	public async void GetNoExistItemAsyncTest()
	{
		var result = await dataStore.ConnectAsync(_user);
		Assert.True(result);
		dataStore.SetCurrentGroup();

		Item newItem = new NewItem
					   {
						   Name = "No item 1",
						   Notes = "You cannot find this item."
					   };
		var item = dataStore.GetItem(newItem.Id);
		Assert.Null(item);
	}
}