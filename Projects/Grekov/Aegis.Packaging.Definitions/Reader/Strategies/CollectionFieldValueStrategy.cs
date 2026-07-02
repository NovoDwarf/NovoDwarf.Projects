using System.Collections;
using Aegis.Packaging.Definitions.Reader.Entities;
using Aegis.Packaging.Definitions.Reader.Interfaces;
using Aegis.Packaging.Definitions.Reader.Parsers;

namespace Aegis.Packaging.Definitions.Reader.Strategies;

internal sealed class CollectionFieldValueStrategy : IFieldValueStrategy
{
	public bool CanHandle(FieldValueReadContext context)
	{
		return context is { Source: FieldValueSource.Element, Field.Kind: DefFieldKind.Auto or DefFieldKind.Collection } &&
		       DefinitionXmlCollectionParser.GetCollectionItemType(context.PropertyType) != null;
	}

	public FieldReadResult Read(FieldValueReadContext context)
	{
		var itemType = DefinitionXmlCollectionParser.GetCollectionItemType(context.PropertyType);
		if (itemType == null)
			return FieldReadResult.Missing();

		var buffer = Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType))
		             ?? throw new InvalidOperationException(
			             $"Failed to instantiate temporary collection buffer for '{context.PropertyType.FullName}'.");

		var bufferList = (IList)buffer;
		var deferredAssignments = new List<(int pendingIndex, int itemIndex)>();

		foreach (var itemElement in DefinitionXmlCollectionParser.EnumerateCollectionItems(context.ValueElement!, context.Field))
		{
			var insertionIndex = bufferList.Count;
			bufferList.Add(itemType.IsValueType ? Activator.CreateInstance(itemType) : null);
			var pendingStartIndex = context.PendingReferences.Count;

			var result = context.Reader.ReadValue(new XmlValueReadContext(
				context.Reader,
				itemType,
				context.Property.Name,
				context.OwnerInstance.GetType().Name,
				itemElement,
				itemElement.HasElements ? null : DefinitionXmlValueParsers.GetValue(itemElement),
				context.PackageId,
				context.ResourcePath,
				context.PendingReferences,
				resolved => bufferList[insertionIndex] = resolved));

			switch (result.Status)
			{
				case FieldReadStatus.Success:
					if (!ReferenceEquals(result.Value, PendingValue.Instance))
						bufferList[insertionIndex] = result.Value;
					else
						for (var pendingIndex = pendingStartIndex; pendingIndex < context.PendingReferences.Count; pendingIndex++)
							deferredAssignments.Add((pendingIndex, insertionIndex));
					break;

				case FieldReadStatus.Missing:
					return FieldReadResult.Invalid(
						$"Failed to parse collection item '{itemElement.Name.LocalName}' as '{itemType.Name}' " +
						$"for property '{context.Property.Name}' on type '{context.OwnerInstance.GetType().Name}' in '{context.ResourcePath}'.");

				case FieldReadStatus.Invalid:
					return result;
			}
		}

		var value = DefinitionXmlCollectionParser.MaterializeCollection(context.PropertyType, itemType, bufferList, context.ResourcePath);

		foreach (var (pendingIndex, itemIndex) in deferredAssignments)
		{
			var pending = context.PendingReferences[pendingIndex];
			context.PendingReferences[pendingIndex] = pending with
			{
				Assign = resolved => AssignCollectionItem(value, itemIndex, resolved)
			};
		}

		return FieldReadResult.Success(value);
	}

	private static void AssignCollectionItem(object collection, int index, object resolved)
	{
		if (collection is Array array)
		{
			array.SetValue(resolved, index);
			return;
		}

		if (collection is IList list)
		{
			list[index] = resolved;
			return;
		}

		throw new InvalidOperationException(
			$"Deferred collection assignment is not supported for collection type '{collection.GetType().FullName}'.");
	}
}
