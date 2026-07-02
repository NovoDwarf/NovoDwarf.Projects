using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NovoDwarf.Mathematics.App.Core.ViewModels;
using NovoDwarf.Mathematics.App.Systems.Algorithms.Entities;

namespace NovoDwarf.Mathematics.App.ViewModels.Utilities.Organizers;

public partial class NotesViewModel : BaseViewModel
{
	private const string PreferencesKey = "notes_data";

	[ObservableProperty]
	public partial string NewNoteTitle { get; set; } = string.Empty;

	[ObservableProperty]
	public partial string NewNoteContent { get; set; } = string.Empty;

	public ObservableCollection<NoteItem> Notes { get; } = [];

	public NotesViewModel()
	{
		LoadNotes();
	}

	[RelayCommand]
	private void AddNote()
	{
		if (string.IsNullOrWhiteSpace(NewNoteTitle) && string.IsNullOrWhiteSpace(NewNoteContent))
			return;

		Notes.Add(new NoteItem
		{
			Title = NewNoteTitle.Trim(),
			Content = NewNoteContent.Trim()
		});

		NewNoteTitle = string.Empty;
		NewNoteContent = string.Empty;

		SaveNotes();
	}

	[RelayCommand]
	private void RemoveNote(NoteItem noteItem)
	{
		Notes.Remove(noteItem);
		SaveNotes();
	}

	private void LoadNotes()
	{
		if (!Preferences.ContainsKey(PreferencesKey))
			return;

		var json = Preferences.Get(PreferencesKey, "[]");

		try
		{
			var list = JsonSerializer.Deserialize<List<NoteItem>>(json);

			if (list == null)
				return;

			foreach (var note in list)
				Notes.Add(note);
		}
		catch { }
	}

	private void SaveNotes()
	{
		var json = JsonSerializer.Serialize(Notes);
		Preferences.Set(PreferencesKey, json);
	}
}