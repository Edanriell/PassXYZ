using System.Collections.ObjectModel;
using System.Diagnostics;
using KPCLib;
using Microsoft.AspNetCore.Components;
using PassXYZ.Vault.Services;
using PassXYZLib;

namespace PassXYZ.Vault.Pages;

public partial class Items
{
	private readonly ObservableCollection<Item> items;
	private readonly string _dialogDeleteId = "deleteModel";
	private readonly string _dialogEditId = "editModel";
	private bool _isNewItem;
	private Item listGroupItem;

	private readonly NewItem newItem;
	private Item? selectedItem;

	public Items()
	{
		listGroupItem = newItem = new NewItem();

		items = new ObservableCollection<Item>();
	}

	[Parameter] public string SelectedItemId { get; set; } = default!;

	[Inject] public IDataStore<Item> DataStore { get; set; } = default!;

	public string Title { get; set; } = default!;

	private async Task LoadGroup(Item? group)
	{
		Title = DataStore.SetCurrentGroup(group);
		items.Clear();
		var itemList = await DataStore.GetItemsAsync(true);
		foreach (var item in itemList)
			// item.SetAvatar();
			items.Add(item);
		Debug.WriteLine($"Items: Selected group is {Title}");
	}

    /// <summary>
    ///     We need to process query parameter here. There are three cases:
    ///     1. Case 1: Without parameter, this is the root group
    ///     2. Case 2: With parameter SelectedItemId and it is a group
    ///     3. Case 3: With parameter SelectedItemId and it is an entry
    /// </summary>
    protected override async void OnParametersSet()
	{
		base.OnParametersSet();

		if (SelectedItemId != null)
		{
			selectedItem = DataStore.GetItem(SelectedItemId);
			if (selectedItem == null)
			{
				Debug.WriteLine("Items: Item cannot be found.");
				throw new ArgumentNullException("SelectedItemId");
			}

			if (selectedItem.IsGroup)
			{
				// Case 2: set to the current group
				await LoadGroup(selectedItem);
			}
			else
			{
				// Case 3: it is an entry
				Debug.WriteLine($"Items: Selected entry is {selectedItem.Name}");
				throw new InvalidOperationException("Items: Selected item must be group here.");
			}
		}
		else
		{
			// Case 1: Set to Root Group
			await LoadGroup(null);
		}
	}

	private async void UpdateItemAsync(string key, string value)
	{
		if (listGroupItem == null) return;
		if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) return;

		listGroupItem.Name = key;
		listGroupItem.Notes = value;

		if (_isNewItem)
		{
			// Add new item
			if (listGroupItem is NewItem aNewItem)
			{
				var newItem = DataStore.CreateNewItem(aNewItem.SubType);
				if (newItem != null)
				{
					newItem.Name = aNewItem.Name;
					newItem.Notes = aNewItem.Notes;
					items.Add(newItem);
					await DataStore.AddItemAsync(newItem);
				}

				Debug.WriteLine(
					$"Items.AddNewItem: type={aNewItem.ItemType}, name={aNewItem.Name}, Notes={aNewItem.Notes}");
			}
		}
		else
		{
			// Update the current item
			await DataStore.UpdateItemAsync(listGroupItem);
			Debug.WriteLine($"Items.UpdateItem: name={listGroupItem.Name}, Notes={listGroupItem.Notes}");
		}
	}

	private async void DeleteItemAsync()
	{
		if (listGroupItem == null) return;

		if (items.Remove(listGroupItem)) _ = await DataStore.DeleteItemAsync(listGroupItem.Id);
		Debug.WriteLine($"Items.DeleteItem: name={listGroupItem.Name}, Notes={listGroupItem.Notes}");
	}
}