﻿using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class SignUpPage : ContentPage
{
	private readonly LoginViewModel _viewModel;

	public SignUpPage(LoginViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}