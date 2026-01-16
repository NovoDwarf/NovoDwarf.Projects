using CommunityToolkit.Mvvm.ComponentModel;

namespace Mathematics.App.Maui.Models.Entities;

public class NoteItem : ObservableObject
{
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
}