using NovoDwarf.Mathematics.App.Systems.Algorithms.DTOs;

namespace NovoDwarf.Mathematics.App.Views.Graphics;

public sealed class PianoRollDrawable : IDrawable
{
    private const int KeysCount = 48;
    private const float KeyHeight = 32f;
    private const float BeatWidth = 80f;
    private const int Beats = 16;
    private const float KeyboardWidth = 60f;

    private readonly SolidColorBrush _background = new(Color.FromArgb("#1E1E1E"));
    private readonly SolidColorBrush _gridLight = new(Color.FromArgb("#2A2A2A"));
    private readonly SolidColorBrush _gridDark = new(Color.FromArgb("#3A3A3A"));
    private readonly SolidColorBrush _noteBrush = new(Color.FromArgb("#FFBF00"));
    private readonly SolidColorBrush _playheadBrush = new(Colors.Red);

    private readonly List<PianoNote> _notes =
    [
        new(Note: 40, Start: 2, Length: 3),
        new(Note: 43, Start: 6, Length: 2),
    ];

    public float Playhead { get; set; } = 4.5f;

    private PianoNote? _activeNote;
    private bool _isDragging;
    private int _activeNoteIndex;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillRectangle(dirtyRect);

        DrawGrid(canvas);
        DrawKeyboard(canvas);
        DrawNotes(canvas);
        DrawPlayhead(canvas);
    }

    public void OnPointerDown(float x, float y)
    {
        if (x < KeyboardWidth)
            return;

        var note = YToNote(y);
        var start = XToBeat(x);

        start = Snap(start);

        var newNote = new PianoNote(
            Note: note,
            Start: start,
            Length: 1);

        _notes.Add(newNote);
        _activeNoteIndex = _notes.Count - 1;
        _activeNote = newNote;
        _isDragging = true;
    }
    public void OnPointerMove(float x, float y)
    {
        if (!_isDragging || _activeNoteIndex == -1)
            return;

        var endBeat = Snap(XToBeat(x));
        var newLength = Math.Max(0.25f, endBeat - _notes[_activeNoteIndex].Start);

        _notes[_activeNoteIndex] = _notes[_activeNoteIndex] with { Length = newLength };
    }

    public void OnPointerUp()
    {
        _activeNote = null;
        _activeNoteIndex = -1;
        _isDragging = false;
    }


    private void DrawGrid(ICanvas canvas)
    {
        canvas.FillColor = _background.Color;
        canvas.FillRectangle(KeyboardWidth, 0,
            Beats * BeatWidth,
            KeysCount * KeyHeight);

        for (var b = 0; b <= Beats; b++)
        {
            canvas.StrokeColor = (b % 4 == 0) ? _gridDark.Color : _gridLight.Color;
            canvas.DrawLine(
                KeyboardWidth + (b * BeatWidth), 0,
                KeyboardWidth + (b * BeatWidth), KeysCount * KeyHeight);
        }

        for (var k = 0; k <= KeysCount; k++)
        {
            canvas.StrokeColor = _gridLight.Color;
            canvas.DrawLine(
                KeyboardWidth, k * KeyHeight,
                KeyboardWidth + (Beats * BeatWidth), k * KeyHeight);
        }
    }

    private void DrawKeyboard(ICanvas canvas)
    {
        for (var i = 0; i < KeysCount; i++)
        {
            var isBlack = IsBlackKey(i);

            canvas.FillColor = isBlack ? Colors.Black : Colors.White;
            canvas.FillRectangle(0, i * KeyHeight, KeyboardWidth, KeyHeight);

            canvas.StrokeColor = Colors.Gray;
            canvas.DrawRectangle(0, i * KeyHeight, KeyboardWidth, KeyHeight);
        }
    }

    private static bool IsBlackKey(int index)
    {
        var note = index % 12;
        return note is 1 or 3 or 6 or 8 or 10;
    }

    private void DrawNotes(ICanvas canvas)
    {
        foreach (var note in _notes)
        {
            var x = KeyboardWidth + (note.Start * BeatWidth);
            var y = (KeysCount - note.Note) * KeyHeight;

            canvas.FillColor = _activeNote == note ? Colors.Orange : _noteBrush.Color;
            canvas.FillRoundedRectangle(
                x + 2, y + 2,
                (note.Length * BeatWidth) - 4,
                KeyHeight - 4,
                4);
        }
    }

    private void DrawPlayhead(ICanvas canvas)
    {
        var x = KeyboardWidth + (Playhead * BeatWidth);

        canvas.StrokeColor = _playheadBrush.Color;
        canvas.StrokeSize = 2;
        canvas.DrawLine(x, 0, x, KeysCount * KeyHeight);
    }

    private int YToNote(float y)
    {
        var index = (int)(y / KeyHeight);
        index = Math.Clamp(index, 0, KeysCount - 1);
        return KeysCount - 1 - index;
    }

    private float XToBeat(float x)
    {
        return (x - KeyboardWidth) / BeatWidth;
    }

    private static float Snap(float beat)
    {
        const float grid = 0.25f;
        return MathF.Round(beat / grid) * grid;
    }

}
