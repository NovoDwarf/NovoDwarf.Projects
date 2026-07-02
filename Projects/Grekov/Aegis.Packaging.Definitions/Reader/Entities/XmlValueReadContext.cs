using System.Xml.Linq;
using Aegis.Packaging.Definitions.Entities;
using Aegis.Packaging.Definitions.Reader.Services;

namespace Aegis.Packaging.Definitions.Reader.Entities;

internal sealed record XmlValueReadContext(
	DefinitionXmlReader Reader,
	Type TargetType,
	string MemberName,
	string OwnerTypeName,
	XElement? ValueElement,
	string? RawValue,
	string PackageId,
	string ResourcePath,
	List<PendingReference> PendingReferences,
	Action<object?>? DeferredValueSetter);
