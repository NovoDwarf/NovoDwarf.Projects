using CommunityToolkit.Mvvm.ComponentModel;

namespace NovoDwarf.Mathematics.App.Systems.Algorithms.Entities;

public class NoteItem : ObservableObject
{
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
}