// Ignore Spelling: DTO xmldoc

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData.DTO;

internal abstract record AbstractDTO
{
	protected static readonly System.Xml.XmlDocument xmldocCDataCreator = new();
}