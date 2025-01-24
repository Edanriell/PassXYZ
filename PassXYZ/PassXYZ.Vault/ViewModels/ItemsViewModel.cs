using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KPCLib;
using Microsoft.Extensions.Logging;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.Views;
using PassXYZLib;

namespace PassXYZ.Vault.ViewModels;

[QueryProperty(nameof(ItemId), nameof(ItemId))]
public partial class ItemsViewModel : BaseViewModel
{
	private readonly IDataStore<Item> dataStore;

	[ObservableProperty] private bool isBusy;

	private readonly ILogger<ItemsViewModel> logger;

	[ObservableProperty] private Item? selectedItem;

	[ObservableProperty] private string? title;

	public ItemsViewModel(IDataStore<Item> dataStore, ILogger<ItemsViewModel> logger)
	{
		this.dataStore = dataStore;
		this.logger = logger;
		Title = "Browse";
		Items = new ObservableCollection<Item>();
		IsBusy = false;
	}

	public ObservableCollection<Item> Items { get; }

	public string ItemId
	{
		get => SelectedItem == null ? string.Empty : SelectedItem.Id;
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				SelectedItem = null;
			}
			else
			{
				var item = dataStore.GetItem(value);
				if (item != null)
					SelectedItem = item;
				else
					throw new ArgumentNullException(nameof(ItemId), "cannot find the selected item");
			}
		}
	}

	[RelayCommand]
	private async Task AddItem(object obj)
	{
		string[] templates =
		{
			Properties.Resources.item_subtype_group,
			Properties.Resources.item_subtype_entry,
			Properties.Resources.item_subtype_notes,
			Properties.Resources.item_subtype_pxentry
		};

		var template = await Shell.Current.DisplayActionSheet(Properties.Resources.pt_id_choosetemplate,
						   Properties.Resources.action_id_cancel, null, templates);
		ItemSubType type;
		if (template == Properties.Resources.item_subtype_entry)
			type = ItemSubType.Entry;
		else if (template == Properties.Resources.item_subtype_pxentry)
			type = ItemSubType.PxEntry;
		else if (template == Properties.Resources.item_subtype_group)
			type = ItemSubType.Group;
		else if (template == Properties.Resources.item_subtype_notes)
			type = ItemSubType.Notes;
		else if (template == Properties.Resources.action_id_cancel)
			type = ItemSubType.None;
		else
			type = ItemSubType.None;

		if (type != ItemSubType.None)
		{
			var itemType = new Dictionary<string, object>
						   {
							   { "Type", type }
						   };
			await Shell.Current.GoToAsync(nameof(NewItemPage), itemType);
		}
	}

	public override async void OnSelection(object sender)
	{
		var item = sender as Item;
		if (item == null)
		{
			logger.LogWarning("item is null.");
			return;
		}

		if (item.IsGroup)
		{
			await Shell.Current.GoToAsync($"{nameof(ItemsPage)}?{nameof(ItemId)}={item.Id}");
		}
		else
		{
			if (item.IsNotes())
				await Shell.Current.GoToAsync($"{nameof(NotesPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
			else
				await Shell.Current.GoToAsync(
					$"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
		}
	}

    /// <summary>
    ///     Update an item. The item can be a group or an entry.
    /// </summary>
    /// <param name="item">an instance of Item</param>
    public async void Update(Item item)
	{
		if (item == null) return;

		await Shell.Current.Navigation.PushAsync(new FieldEditPage(async (k, v, isProtected) =>
		{
			item.Name = k;
			item.Notes = v;
			await dataStore.UpdateItemAsync(item);
		}, item.Name, item.Notes, true));
	}

    /// <summary>
    ///     Delete an item.
    /// </summary>
    /// <param name="item">an instance of Item</param>
    public async Task Delete(Item item)
	{
		if (item == null) return;

		if (Items.Remove(item))
			_ = await dataStore.DeleteItemAsync(item.Id);
		else
			throw new NullReferenceException("Delete item error");
	}

	[RelayCommand]
	private async Task LoadItems()
	{
		try
		{
			Items.Clear();
			var items = await dataStore.GetItemsAsync(true);
			foreach (var item in items) Items.Add(item);
			logger.LogDebug($"IsBusy={IsBusy}, added {Items.Count()} items");
		}
		catch (Exception ex)
		{
			logger.LogError("{ex}", ex);
		}
		finally
		{
			IsBusy = false;
			logger.LogDebug("Set IsBusy to false");
		}
	}

    /// <summary>
    ///     The logic of navigation is implemented here.
    ///     The current group is set here according to the selected item.
    /// </summary>
    public void OnAppearing()
	{
		if (SelectedItem == null)
			// If SelectedItem is null, this is the root group.
			Title = dataStore.SetCurrentGroup();
		else
			Title = dataStore.SetCurrentGroup(SelectedItem);
		// load items
		IsBusy = true;
	}
}