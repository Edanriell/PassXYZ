using System.Diagnostics;
using KPCLib;

namespace PassXYZ.Vault.Tests;

public partial class FieldEditTests : TestContext
{
	private readonly string _dialogId = "editField";
	private readonly string updated_key = "PIN";
	private readonly string updated_value = "1234";

	public FieldEditTests()
	{
		TestField = new Field("", "", false);
	}

	public bool IsNewField { get; set; }
	public Field TestField { get; set; }

	private void OnSaveClicked(string key, string value)
	{
		TestField.Key = key;
		TestField.Value = value;

		Debug.WriteLine(
			$"FieldNew: OnSaveClicked(key={TestField.Key}, value={TestField.Value}, type={TestField.IsProtected})");
	}

	[Fact]
	public void Edit_Existing_Field()
	{
		// Arrange
		IsNewField = false;
		var cut = Render(_editorDialog);
		// Act
		cut.Find("textarea").Change(updated_value);
		cut.Find("button[type=submit]").Click();
		// Assert
		Assert.Equal(updated_value, TestField.Value);
	}

	[Fact]
	public void Edit_New_Field()
	{
		// Arrange
		IsNewField = true;
		var cut = Render(_editorDialog);
		// Act
		cut.Find("#flexCheckDefault").Change(true);
		cut.Find("input").Change(updated_key);
		cut.Find("textarea").Change(updated_value);
		cut.Find("button[type=submit]").Click();
		// Assert
		Assert.True(TestField.IsProtected);
		Assert.Equal(updated_key, TestField.Key);
		Assert.Equal(updated_value, TestField.EditValue);
	}
}