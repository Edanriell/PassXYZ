using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace PassXYZ.BlazorUI;

public partial class ModalDialog : IDisposable
{
	[CascadingParameter(Name = "Id")] public string Id { get; set; } = default!;

	[Parameter] public string? Title { get; set; }

	[Parameter] public RenderFragment ChildContent { get; set; } = default!;

	[Parameter] public Action? OnClose { get; set; }

	[Parameter] public Func<Task<bool>>? OnSaveAsync { get; set; }

	[Parameter] [NotNull] public string? CloseButtonText { get; set; }

	[Parameter] [NotNull] public string? SaveButtonText { get; set; }

	void IDisposable.Dispose()
	{
		GC.SuppressFinalize(this);
		Debug.WriteLine($"ModalDialog: delete - {Id}");
	}

	private void OnClickClose()
	{
		Debug.WriteLine($"ModalDialog: DialogId={Id}");
		OnClose?.Invoke();
	}

	private async Task OnClickSave()
	{
		if (OnSaveAsync != null) await OnSaveAsync();
	}
}