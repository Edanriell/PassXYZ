using System.Diagnostics;
using PassXYZ.BlazorUI;

namespace PassXYZ.Vault.Tests.BlazorUI;

public class ModelDialogTests : TestContext
{
	[Fact]
	public void ModalDialogInitTest()
	{
		var title = "ModalDialog Test";
		var cut = RenderComponent<ModalDialog>(
			parameters => parameters.Add(p => p.Title, title)
			   .Add(p => p.CloseButtonText, "Close")
			   .Add(p => p.SaveButtonText, "Save"));
		cut.Find("h5").TextContent.MarkupMatches(title);
		Debug.WriteLine($"{cut.Markup}");
	}

	[Fact]
	public void ConfirmDialogInitTest()
	{
		var title = "Deleting";
		var cut = RenderComponent<ConfirmDialog>();
		var dialog = cut.Instance;
		cut.Find("h5").TextContent.MarkupMatches(title);
		Debug.WriteLine($"{cut.Markup}");
	}
}