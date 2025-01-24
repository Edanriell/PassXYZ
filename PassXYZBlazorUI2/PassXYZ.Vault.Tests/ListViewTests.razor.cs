using System.Collections.ObjectModel;
using System.Diagnostics;
using KPCLib;
using PassXYZ.BlazorUI;
using PassXYZLib;

namespace PassXYZ.Vault.Tests;

public partial class ListViewTests : TestContext
{
	private readonly ObservableCollection<Item> items;
	private readonly string footerContent = "This is list view footer.";
	private readonly string headerContent = "This is list view headker.";
	private readonly NewItem testItem;

	public ListViewTests()
	{
		items = new ObservableCollection<Item>();
		testItem = new NewItem
				   {
					   Name = "New item",
					   Notes = "This is a new item."
				   };
		items.Add(testItem);
	}

	[Fact]
	public void ListView_Init_WithoutParameters()
	{
		var cut = RenderComponent<ListView<Item>>();
		Assert.NotNull(cut.Find("div[class=list-group]"));
		Debug.WriteLine($"{cut.Markup}");
	}

	[Fact]
	public void Display_ListView_Items()
	{
		var cut = Render(_listView);
		var item = cut.Find("a");
		Assert.Equal(testItem.Name, item.TextContent);
		Debug.WriteLine($"{cut.Markup}");
	}

	[Fact]
	public void Check_ListView_HeaderAndFooter()
	{
		var cut = Render(_listView);
		Assert.Equal(headerContent, cut.Find("#list_view_header").TextContent);
		Assert.Equal(footerContent, cut.Find("article").TextContent);
	}
}