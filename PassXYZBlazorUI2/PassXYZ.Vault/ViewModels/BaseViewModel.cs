using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PassXYZ.Vault.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
	[RelayCommand]
	private void ItemSelectionChanged(object sender)
	{
		OnSelection(sender);
	}

	public abstract void OnSelection(object sender);
}