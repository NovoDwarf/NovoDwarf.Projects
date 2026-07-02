using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using NovoDwarf.Mathematics.App.Core.ViewModels;

namespace NovoDwarf.Mathematics.App.Views.Common;

public enum TreeNodeKind
{
	Group,
	Leaf
}

public enum DuplicateLeafPolicy
{
	Ignore,
	Throw,
	Replace
}

public static class TreeNodeCommandExtensions
{
	public static void AttachCommandRecursive(
		this IEnumerable<TreeNodeViewModel> nodes,
		Func<TreeNodeViewModel, ICommand?> commandSelector)
	{
		foreach (var node in nodes)
		{
			node.TapCommand ??= commandSelector(node);

			if (node.HasChildren)
				node.Children.AttachCommandRecursive(commandSelector);
		}
	}
}


public sealed class TreeBuilderOptions<T>
{
	public DuplicateLeafPolicy DuplicatePolicy { get; init; } = DuplicateLeafPolicy.Ignore;
	public IComparer<TreeNodeViewModel>? SortComparer { get; init; }
	public Func<string, string>? Localize { get; init; }
	public Func<T, ICommand?>? LeafCommandFactory { get; init; }
}

public static class TreeBuilder
{
	public static ObservableCollection<TreeNodeViewModel> Build<T>(
		IEnumerable<T> items,
		Func<T, IReadOnlyList<string>> pathIdSelector,
		Func<T, string> titleSelector,
		Func<T, string?> descriptionSelector,
		Func<string, string>? pathTitleSelector = null,
		TreeBuilderOptions<T>? options = null)
		where T : notnull
	{
		options ??= new TreeBuilderOptions<T>();

		var roots = new ObservableCollection<TreeNodeViewModel>();
		var rootIndex = new Dictionary<string, TreeNodeViewModel>(StringComparer.Ordinal);

		foreach (var item in items)
		{
			AddNode(
				roots,
				rootIndex,
				pathIdSelector(item),
				titleSelector(item),
				descriptionSelector(item),
				pathTitleSelector,
				item,
				options);
		}

		SortRecursive(roots, options.SortComparer);
		return roots;
	}

	private static void AddNode<T>(
		ObservableCollection<TreeNodeViewModel> roots,
		Dictionary<string, TreeNodeViewModel> rootIndex,
		IReadOnlyList<string> pathIds,
		string leafTitleRaw,
		string? leafDescriptionRaw,
		Func<string, string>? pathTitleSelector,
		T payload,
		TreeBuilderOptions<T> options)
	{
		TreeNodeViewModel? parent = null;

		foreach (var segmentId in pathIds)
		{
			var titleRaw = pathTitleSelector?.Invoke(segmentId) ?? segmentId;
			var title = options.Localize?.Invoke(titleRaw) ?? titleRaw;

			if (parent == null)
			{
				if (!rootIndex.TryGetValue(segmentId, out var root))
				{
					root = TreeNodeViewModel.Group(segmentId, title, null);
					rootIndex.Add(segmentId, root);
					roots.Add(root);
				}

				parent = root;
			}
			else
			{
				if (!parent.TryGetChild(segmentId, out var child))
				{
					child = TreeNodeViewModel.Group(segmentId, title, null);
					parent.AddChild(child);
				}

				parent = child;
			}
		}

		var leafId = leafTitleRaw; // или отдельный idSelector при необходимости
		var leafTitle = options.Localize?.Invoke(leafTitleRaw) ?? leafTitleRaw;
		var leafDescription = leafDescriptionRaw != null
			? options.Localize?.Invoke(leafDescriptionRaw) ?? leafDescriptionRaw
			: null;

		if (parent != null && parent.TryGetChild(leafId, out var existing))
		{
			switch (options.DuplicatePolicy)
			{
				case DuplicateLeafPolicy.Ignore:
					return;

				case DuplicateLeafPolicy.Throw:
					throw new InvalidOperationException($"Duplicate leaf id: {leafId}");

				case DuplicateLeafPolicy.Replace:
					parent.RemoveChild(existing);
					break;
			}
		}

		var leaf = TreeNodeViewModel.Leaf(leafId, leafTitle, leafDescription, payload);
		leaf.TapCommand = options.LeafCommandFactory?.Invoke(payload);

		if (parent != null)
			parent.AddChild(leaf);
		else
			roots.Add(leaf);
	}

	private static void SortRecursive(
		ObservableCollection<TreeNodeViewModel> nodes,
		IComparer<TreeNodeViewModel>? comparer)
	{
		if (comparer == null)
			return;

		var sorted = nodes.OrderBy(x => x, comparer).ToList();
		nodes.Clear();

		foreach (var node in sorted)
		{
			nodes.Add(node);
			SortRecursive(node.Children, comparer);
		}
	}
}


public sealed partial class TreeNodeViewModel : BaseViewModel
{
	private readonly Dictionary<string, TreeNodeViewModel> _childrenIndex;

	private TreeNodeViewModel(
		string id,
		TreeNodeKind kind,
		string title,
		string? description,
		object? payload)
	{
		Id = id;
		Kind = kind;
		Title = title;
		Description = description;
		Payload = payload;

		_childrenIndex = new Dictionary<string, TreeNodeViewModel>(StringComparer.Ordinal);
		Children = [];
	}

	public static TreeNodeViewModel Group(string id, string title, string? description)
		=> new(id, TreeNodeKind.Group, title, description, null);

	public static TreeNodeViewModel Leaf(string id, string title, string? description, object? payload)
		=> new(id, TreeNodeKind.Leaf, title, description, payload);

	public string Id { get; }
	public TreeNodeKind Kind { get; }

	public string Title { get; }
	public string? Description { get; }
	public object? Payload { get; }

	[ObservableProperty]
	public partial ICommand? TapCommand { get; set; }

	public ObservableCollection<TreeNodeViewModel> Children { get; }

	public bool IsGroup => Kind == TreeNodeKind.Group;
	public bool IsLeaf => Kind == TreeNodeKind.Leaf;
	public bool HasChildren => Children.Count > 0;
	public bool HasDescription => !string.IsNullOrWhiteSpace(Description);

	internal bool TryGetChild(string id, out TreeNodeViewModel node)
		=> _childrenIndex.TryGetValue(id, out node!);

	internal void AddChild(TreeNodeViewModel node)
	{
		_childrenIndex.Add(node.Id, node);
		Children.Add(node);
		OnPropertyChanged(nameof(HasChildren));
	}

	internal void RemoveChild(TreeNodeViewModel node)
	{
		if (!_childrenIndex.Remove(node.Id)) 
			return;
		
		Children.Remove(node);
		OnPropertyChanged(nameof(HasChildren));
	}
}


public partial class TreeNodeView : ContentView
{
	public TreeNodeView()
	{
		InitializeComponent();
	}
}