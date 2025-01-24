using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Maui.Controls.Core.UnitTests;

public class ShellTestBase : BaseTestFixture
{
	#pragma warning disable CS8604 // Possible null reference argument.
	#pragma warning disable CS8603 // Possible null reference argument.

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			//Routing.Clear();
		}

		base.Dispose(disposing);
	}

	protected T FindParentOfType<T>(Element element)
	{
		var navPage = GetParentsPath(element)
		   .OfType<T>()
		   .FirstOrDefault();

		return navPage;
	}

	protected T GetVisiblePage<T>(Shell shell)
		where T : Page
	{
		if (shell?.CurrentItem?.CurrentItem is IShellSectionController scc)
			return (T)scc.PresentedPage;

		return default;
	}

	protected IEnumerable<Element> GetParentsPath(Element self)
	{
		var current = self;

		//while (!Application.IsApplicationOrNull(current.RealParent))
		//{
		//    current = current.RealParent;
		//    yield return current;
		//}
		yield return current;
	}

	protected bool IsModal(BindableObject bindableObject)
	{
		return (Shell.GetPresentationMode(bindableObject) & PresentationMode.Modal) == PresentationMode.Modal;
	}

	protected bool IsAnimated(BindableObject bindableObject)
	{
		return (Shell.GetPresentationMode(bindableObject) & PresentationMode.NotAnimated) !=
			   PresentationMode.NotAnimated;
	}

	//protected Uri CreateUri(string uri) => ShellUriHandler.CreateUri(uri);

	protected ShellSection MakeSimpleShellSection(string route, string contentRoute)
	{
		return MakeSimpleShellSection(route, contentRoute, new ShellTestPage());
	}

	protected ShellSection MakeSimpleShellSection(string route, string contentRoute, ContentPage contentPage)
	{
		var shellSection = new ShellSection();
		shellSection.Route = route;
		var shellContent = new ShellContent { Content = contentPage, Route = contentRoute };
		shellSection.Items.Add(shellContent);
		return shellSection;
	}

	[QueryProperty("DoubleQueryParameter", "DoubleQueryParameter")]
	[QueryProperty("SomeQueryParameter", "SomeQueryParameter")]
	[QueryProperty("CancelNavigationOnBackButtonPressed", "CancelNavigationOnBackButtonPressed")]
	[QueryProperty("ComplexObject", "ComplexObject")]
	public class ShellTestPage : ContentPage
	{
		public string? CancelNavigationOnBackButtonPressed { get; set; }

		public string? SomeQueryParameter { get; set; }

		public double? DoubleQueryParameter { get; set; }

		public object? ComplexObject { get; set; }

		protected override void OnParentSet()
		{
			base.OnParentSet();
		}

		protected override bool OnBackButtonPressed()
		{
			if (CancelNavigationOnBackButtonPressed == "true")
				return true;

			if (CancelNavigationOnBackButtonPressed == "false")
				return false;

			return base.OnBackButtonPressed();
		}
	}

	protected ShellItem CreateShellItem(
		TemplatedPage? page = null,
		bool asImplicit = false,
		string? shellContentRoute = null,
		string? shellSectionRoute = null,
		string? shellItemRoute = null,
		bool templated = false)
	{
		return CreateShellItem<ShellItem>(
			page,
			asImplicit,
			shellContentRoute,
			shellSectionRoute,
			shellItemRoute,
			templated);
	}

	protected T CreateShellItem<T>(
		TemplatedPage? page = null,
		bool asImplicit = false,
		string? shellContentRoute = null,
		string? shellSectionRoute = null,
		string? shellItemRoute = null,
		bool templated = false) where T : ShellItem
	{
		T? item = null;
		var section = CreateShellSection(page, asImplicit, shellContentRoute, shellSectionRoute, templated);

		if (!string.IsNullOrWhiteSpace(shellItemRoute))
		{
			item = Activator.CreateInstance<T>();
			item.Route = shellItemRoute;
			item.Items.Add(section);
		}
		//else if (asImplicit)
		//    item = (T)ShellItem.CreateFromShellSection(section);
		else
		{
			item = Activator.CreateInstance<T>();
			item.Items.Add(section);
		}

		return item;
	}

	protected ShellSection CreateShellSection(
		TemplatedPage? page = null,
		bool asImplicit = false,
		string? shellContentRoute = null,
		string? shellSectionRoute = null,
		bool templated = false)
	{
		return CreateShellSection<ShellSection>(
			page,
			asImplicit,
			shellContentRoute,
			shellSectionRoute,
			templated);
	}

	protected T CreateShellSection<T>(
		TemplatedPage? page = null,
		bool asImplicit = false,
		string? shellContentRoute = null,
		string? shellSectionRoute = null,
		bool templated = false) where T : ShellSection
	{
		var content = CreateShellContent(page, asImplicit, shellContentRoute, templated);

		T? section = null;

		if (!string.IsNullOrWhiteSpace(shellSectionRoute))
		{
			section = Activator.CreateInstance<T>();
			section.Route = shellSectionRoute;
			section.Items.Add(content);
		}
		//else if (asImplicit)
		//    section = (T)ShellSection.CreateFromShellContent(content);
		else
		{
			section = Activator.CreateInstance<T>();
			section.Items.Add(content);
		}

		return section;
	}

	protected ShellContent CreateShellContent(TemplatedPage? page = null, bool asImplicit = false,
											  string? shellContentRoute = null, bool templated = false)
	{
		ShellContent? content = null;

		if (!string.IsNullOrWhiteSpace(shellContentRoute))
		{
			if (templated)
				content = new ShellContent { ContentTemplate = new DataTemplate(() => page ?? new ContentPage()) };
			else
				content = new ShellContent { Content = page ?? new ContentPage() };

			content.Route = shellContentRoute;
		}
		else if (asImplicit)
		{
			content = (ShellContent)(page ?? new ContentPage());
		}
		else
		{
			if (templated)
				content = new ShellContent { ContentTemplate = new DataTemplate(() => page ?? new ContentPage()) };
			else
				content = new ShellContent { Content = page ?? new ContentPage() };
		}


		return content;
	}

	protected ReadOnlyCollection<ShellContent> GetItems(ShellSection section)
	{
		return (section as IShellSectionController).GetItems();
	}

	protected ReadOnlyCollection<ShellSection> GetItems(ShellItem item)
	{
		return (item as IShellItemController).GetItems();
	}

	protected ReadOnlyCollection<ShellItem> GetItems(Shell item)
	{
		return (item as IShellController).GetItems();
	}


	public class TestFlyoutItem : FlyoutItem
	{
		public TestFlyoutItem()
		{
		}

		public TestFlyoutItem(ShellSection shellSection)
		{
			Items.Add(shellSection);
		}
	}

	public class TestShellSection : ShellSection
	{
		public TestShellSection()
		{
		}

		public TestShellSection(ShellContent shellContent)
		{
			Items.Add(shellContent);
		}

		public bool? LastPopWasAnimated { get; private set; }

		protected override Task<Page> OnPopAsync(bool animated)
		{
			LastPopWasAnimated = animated;
			return base.OnPopAsync(animated);
		}
	}

	public class TestShell : Shell
	{
		public ShellNavigatedEventArgs? LastShellNavigatedEventArgs;
		public ShellNavigatingEventArgs? LastShellNavigatingEventArgs;
		public int NavigatedCount;
		public int NavigatingCount;
		public int OnBackButtonPressedCount;
		public Func<bool>? OnBackButtonPressedFunc;
		public int OnNavigatedCount;
		public int OnNavigatingCount;

		public TestShell()
		{
			//_ = new TestWindow() { Page = this };
			Routing.RegisterRoute(nameof(TestPage1), typeof(TestPage1));
			Routing.RegisterRoute(nameof(TestPage2), typeof(TestPage2));
			Routing.RegisterRoute(nameof(TestPage3), typeof(TestPage3));

			Navigated += (_, __) => NavigatedCount++;
			Navigating += (_, __) => NavigatingCount++;
		}

		public TestShell(params ShellItem[] shellItems) : this()
		{
			//shellItems.ForEach(x => Items.Add(x));
		}

		public IShellController Controller => this;
		public Action<ShellNavigatedEventArgs>? OnNavigatedHandler { get; set; }

		public bool? LastPopWasAnimated => (CurrentItem.CurrentItem as TestShellSection)?.LastPopWasAnimated;

		public List<List<Element>> GenerateTestFlyoutItems()
		{
			List<List<Element>> returnValue = new List<List<Element>>();


			//FlyoutItems
			//    .OfType<IEnumerable>()
			//    .ForEach(l => returnValue.Add(l.OfType<Element>().ToList()));

			return returnValue;
		}

		public ContentPage RegisterPage(string route)
		{
			var page = new ContentPage();
			RegisterPage(route, page);
			return page;
		}

		public void RegisterPage(string route, ContentPage contentPage)
		{
			Routing.SetRoute(contentPage, route);
			Routing.RegisterRoute(route, new ConcretePageFactory(contentPage));
		}

		public void AssertCurrentStateEquals(string expectedState)
		{
			Assert.Equal(expectedState, CurrentState.Location.ToString());
		}

		protected override void OnNavigated(ShellNavigatedEventArgs args)
		{
			LastShellNavigatedEventArgs = args;
			base.OnNavigated(args);
			OnNavigatedHandler?.Invoke(args);
			OnNavigatedCount++;
		}

		protected override void OnNavigating(ShellNavigatingEventArgs args)
		{
			LastShellNavigatingEventArgs = args;
			base.OnNavigating(args);
			OnNavigatingCount++;

			if (args.Target.Location.ToString() == "NewItemPage") args.Cancel();

			if (args.Current != null)
				Debug.WriteLine($"TestShell: source={args.Current.Location}, target={args.Target.Location}");
		}

		public void TestNavigationArgs(ShellNavigationSource source, string from, string to)
		{
			TestNavigatingArgs(source, from, to);
			TestNavigatedArgs(source, from, to);
		}

		public void TestNavigatedArgs(ShellNavigationSource source, string from, string to)
		{
			if (LastShellNavigatedEventArgs == null)
				throw new NullReferenceException("LastShellNavigatedEventArgs is null");

			Assert.Equal(source, LastShellNavigatedEventArgs.Source);

			if (from == null)
				Assert.Null(LastShellNavigatedEventArgs.Previous);
			else
				Assert.Equal(from, LastShellNavigatedEventArgs.Previous.Location.ToString());

			Assert.Equal(to, LastShellNavigatedEventArgs.Current.Location.ToString());
			Assert.Equal(to, CurrentState.Location.ToString());
		}

		public void TestNavigatingArgs(ShellNavigationSource source, string from, string to)
		{
			if (LastShellNavigatingEventArgs == null)
				throw new NullReferenceException("LastShellNavigatingEventArgs is null");
			Assert.Equal(source, LastShellNavigatingEventArgs.Source);

			if (from == null)
				Assert.Null(LastShellNavigatingEventArgs.Current);
			else
				Assert.Equal(from, LastShellNavigatingEventArgs.Current.Location.ToString());

			Assert.Equal(to, LastShellNavigatingEventArgs.Target.Location.ToString());
		}

		protected override bool OnBackButtonPressed()
		{
			var result = OnBackButtonPressedFunc?.Invoke() ?? false;

			OnBackButtonPressedCount++;

			if (!result)
				result = base.OnBackButtonPressed();

			return result;
		}

		public void Reset()
		{
			OnNavigatedCount =
				OnNavigatingCount =
					NavigatedCount =
						NavigatingCount =
							OnBackButtonPressedCount = 0;
		}

		public void TestCount(int count, string? message = null)
		{
			Assert.True(count == OnNavigatedCount, $"OnNavigatedCount: {message}");
			Assert.True(count == NavigatingCount, $"NavigatingCount: {message}");
			Assert.True(count == OnNavigatingCount, $"OnNavigatingCount: {message}");
			Assert.True(count == NavigatedCount, $"NavigatedCount: {message}");
		}

		public class ConcretePageFactory : RouteFactory
		{
			private readonly ContentPage _contentPage;

			public ConcretePageFactory(ContentPage contentPage)
			{
				_contentPage = contentPage;
			}

			public override Element GetOrCreate(IServiceProvider services)
			{
				return _contentPage;
			}

			public override Element GetOrCreate()
			{
				return _contentPage;
			}
		}
	}


	public class TestShellViewModel : INotifyPropertyChanged
	{
		private string? _text;
		public TestShellViewModel? SubViewModel { get; set; }
		public TestShellViewModel? SubViewModel2 { get; set; }

		public string Text
		{
			get => _text ?? throw new NullReferenceException("Text cannot be null");
			set
			{
				_text = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;
	}

	public class TestPage1 : ContentPage
	{
	}

	public class TestPage2 : ContentPage
	{
	}

	public class TestPage3 : ContentPage
	{
	}

	public class PageWithDependency : ContentPage
	{
		public PageWithDependency(Dependency dependency)
		{
			TestDependency = dependency;
		}

		public Dependency TestDependency { get; set; }
	}

	public class PageWithDependencyAndMultipleConstructors : ContentPage
	{
		public PageWithDependencyAndMultipleConstructors(Dependency dependency)
		{
			TestDependency = dependency;
		}

		public PageWithDependencyAndMultipleConstructors(Dependency dependency,
														 UnregisteredDependency unregisteredDependency)
		{
			OtherTestDependency = unregisteredDependency;
		}

		public PageWithDependencyAndMultipleConstructors()
		{
			// parameterless constructor
		}

		public Dependency? TestDependency { get; set; }
		public UnregisteredDependency? OtherTestDependency { get; set; }
	}

	public class PageWithUnregisteredDependencyAndParameterlessConstructor : ContentPage
	{
		public PageWithUnregisteredDependencyAndParameterlessConstructor(UnregisteredDependency dependency)
		{
		}

		public PageWithUnregisteredDependencyAndParameterlessConstructor()
		{
		}
	}

	public class Dependency
	{
		public int Test { get; set; }
	}

	public class UnregisteredDependency
	{
		public int Test { get; set; }
	}
	#pragma warning restore CS8604 // Possible null reference argument.
	#pragma warning restore CS8603 // Possible null reference argument.
}