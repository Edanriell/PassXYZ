using System.Diagnostics;
using Microsoft.AspNetCore.Components;

namespace PassXYZ.BlazorUI;

public partial class EditorDialog
{
	private bool _isKeyEditingEnable = false;

	private string _key = string.Empty;

	private string _value = string.Empty;

	[Parameter]
	public bool IsKeyEditingEnable
	{
		get => _isKeyEditingEnable;
		set
		{
			if (value != _isKeyEditingEnable)
			{
				_isKeyEditingEnable = value;
				IsKeyEditingEnableChanged?.InvokeAsync(_isKeyEditingEnable);
				Debug.WriteLine($"EditorDialog: _isKeyEditingEnable={_isKeyEditingEnable}");
			}
		}
	}

	[Parameter] public EventCallback<bool>? IsKeyEditingEnableChanged { get; set; }

	[Parameter]
	public string Key
	{
		get => _key;
		set
		{
			if (_key != value)
			{
				_key = value;
				KeyChanged?.InvokeAsync(_key);
				Debug.WriteLine($"EditorDialog: Key={_key}");
			}
		}
	}

	[Parameter] public EventCallback<string>? KeyChanged { get; set; }

	[Parameter] public string? KeyPlaceHolder { get; set; }

	[Parameter]
	public string Value
	{
		get => _value;
		set
		{
			if (value != _value)
			{
				_value = value ?? string.Empty;
				ValueChanged?.InvokeAsync(_value);
				Debug.WriteLine($"EditorDialog: Value={_value}");
			}
		}
	}

	[Parameter] public EventCallback<string>? ValueChanged { get; set; }

	[Parameter] public string? ValuePlaceHolder { get; set; }

	[Parameter] public RenderFragment ChildContent { get; set; } = default!;

	[Parameter] public Action<string, string>? OnSave { get; set; }

	[Parameter] public Action? OnClose { get; set; }

	private async Task<bool> OnSaveClicked()
	{
		OnSave?.Invoke(Key, Value);
		Debug.WriteLine($"EditorDialog: OnSaveClicked(key={Key}, value={Value})");
		return await Task.FromResult(true);
	}

	private void OnCloseClicked()
	{
		OnClose?.Invoke();
	}
}