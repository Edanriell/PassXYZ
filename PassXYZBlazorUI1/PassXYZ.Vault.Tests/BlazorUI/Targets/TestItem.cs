using KPCLib;

namespace PassXYZ.Vault.Tests.BlazorUI.Targets;

public class TestItem : Item
{
	private readonly Guid uid = new();

	public override string Id => uid.ToString();

	public override string Name { get; set; } = string.Empty;
	public override string Notes { get; set; } = string.Empty;
	public override bool IsGroup => ItemType.Equals("group");
	public override DateTime LastModificationTime { get; set; } = default!;

	public override string Description =>
		$"{ItemType} | {LastModificationTime.ToString("yyyy'-'MM'-'dd")} | {Notes}".Truncate(50);

	public string ItemType { get; set; } = "Group";
}