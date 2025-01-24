using HybridWebView;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views;

public partial class NotesPage : ContentPage
{
	private readonly ItemDetailViewModel _viewModel;

	public NotesPage(ItemDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		#if DEBUG
		markdownview.EnableWebDevTools = true;
		#endif
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		markdownview.Reload();
	}

	private void OnHybridWebViewRawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
	{
		markdownview.DisplayMarkdown(_viewModel.MarkdownText);
	}
}