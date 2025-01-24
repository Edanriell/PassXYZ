using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KPCLib;
using Microsoft.Extensions.Logging;
using PassXYZ.Vault.Services;
using PassXYZLib;

namespace PassXYZ.Vault.ViewModels;

[QueryProperty(nameof(Type), nameof(Type))]
public partial class NewItemViewModel : ObservableObject
{
	private readonly IDataStore<Item>? _dataStore;
	private readonly ILogger<NewItemViewModel> _logger;
	private ItemSubType _type = ItemSubType.Group;

	[ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
	private string? description;

	[ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
	private string? name;

	[ObservableProperty] private string? placeholder;

	public NewItemViewModel(IDataStore<Item> dataStore, ILogger<NewItemViewModel> logger)
	{
		_dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
		_logger = logger;
	}

	public ItemSubType Type
	{
		get => _type;
		set
		{
			_ = SetProperty(ref _type, value);
			SetPlaceholder(_type);
		}
	}

	private void SetPlaceholder(ItemSubType type)
	{
		if (type == ItemSubType.Group)
			Placeholder = Properties.Resources.action_id_add + " " + Properties.Resources.item_subtype_group;
		else
			Placeholder = Properties.Resources.action_id_add + " " + Properties.Resources.item_subtype_entry;
	}

	[RelayCommand]
	private async Task Cancel()
	{
		// This will pop the current page off the navigation stack
		await Shell.Current.GoToAsync("..");
	}

	[RelayCommand(CanExecute = nameof(ValidateSave))]
	private async Task Save()
	{
		if (_dataStore == null) throw new ArgumentNullException("dataStore cannot be null");
		var newItem = _dataStore.CreateNewItem(_type);

		if (newItem != null)
		{
			newItem.Name = Name;
			newItem.Notes = Description;
			await _dataStore.AddItemAsync(newItem);
		}

		// This will pop the current page off the navigation stack
		await Shell.Current.GoToAsync("..");
	}

	private bool ValidateSave()
	{
		var canExecute = !string.IsNullOrWhiteSpace(Name)
					  && !string.IsNullOrWhiteSpace(Description);
		_logger.LogDebug("ValidateSave: {canExecute}", canExecute);
		return canExecute;
	}
}