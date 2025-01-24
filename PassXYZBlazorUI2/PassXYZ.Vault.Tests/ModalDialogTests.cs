using PassXYZ.BlazorUI;

namespace PassXYZ.Vault.Tests;

[Collection("Serilog collection")]
public class ModalDialogTests : TestContext
{
	private readonly SerilogFixture serilogFixture;

	public ModalDialogTests(SerilogFixture serilogFixture)
	{
		this.serilogFixture = serilogFixture;
	}

	[Fact]
	public void ModalDialogInitTest()
	{
		var title = "ModalDialog Test";
		var cut = RenderComponent<ModalDialog>(
			parameters => parameters.Add(p => p.Title, title)
			   .Add(p => p.CloseButtonText, "Close")
			   .Add(p => p.SaveButtonText, "Save"));
		cut.Find("h5").TextContent.MarkupMatches(title);
		serilogFixture.Logger.Debug("ModalDialogInitTest: done");
	}

	[Fact]
	public void ConfirmDialogInitTest()
	{
		var title = "Deleting";
		var cut = RenderComponent<ConfirmDialog>();
		var dialog = cut.Instance;
		cut.Find("h5").TextContent.MarkupMatches(title);
		serilogFixture.Logger.Debug("ConfirmDialogInitTest: done");
	}
}