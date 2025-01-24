using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views;

public partial class ItemsPage : ContentPage
{
	private readonly ItemsViewModel viewModel;

	public ItemsPage(ItemsViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = this.viewModel = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		viewModel.OnAppearing();
	}
}