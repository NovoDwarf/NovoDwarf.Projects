using Mathematics.App.Maui.UI.Base;
using SkiaSharp;
using SKPaintSurfaceEventArgs = SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs;

namespace Mathematics.App.Maui.UI.Views.Games;

public partial class SlidingTilePuzzleViewModel : BaseViewModel
{
	private int[,] _gameBoard = new int[4, 4];
        public int[,] GameBoard
        {
            get => _gameBoard;
            private set
            {
                _gameBoard = value;
                OnPropertyChanged();
            }
        }

        private readonly Random _rand = new();

        public SlidingTilePuzzleViewModel()
        {
            InitGame();
        }

        public void InitGame()
        {
            Array.Clear(_gameBoard, 0, _gameBoard.Length);
            AddRandomTile();
            AddRandomTile();
            OnPropertyChanged(nameof(GameBoard));
        }

        public void AddRandomTile()
        {
            var emptyCells = new List<(int r, int c)>();

            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    if (_gameBoard[r, c] == 0)
                        emptyCells.Add((r, c));

            if (emptyCells.Count == 0) return;

            var (row, col) = emptyCells[_rand.Next(emptyCells.Count)];
            _gameBoard[row, col] = _rand.Next(0, 10) == 0 ? 4 : 2;
            OnPropertyChanged(nameof(GameBoard));
        }

        // ===== Логика движения и слияния =====
        public bool MoveUp() => Move(isHorizontal: false, reverse: false);
        public bool MoveDown() => Move(isHorizontal: false, reverse: true);
        public bool MoveLeft() => Move(isHorizontal: true, reverse: false);
        public bool MoveRight() => Move(isHorizontal: true, reverse: true);

        private bool Move(bool isHorizontal, bool reverse)
        {
            bool moved = false;

            for (int line = 0; line < 4; line++)
            {
                var nums = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    int index = reverse ? 3 - i : i;
                    int value = isHorizontal ? _gameBoard[line, index] : _gameBoard[index, line];
                    nums.Add(value);
                }

                var merged = MergeLine(nums);

                for (int i = 0; i < 4; i++)
                {
                    int index = reverse ? 3 - i : i;
                    if (isHorizontal)
                    {
                        if (_gameBoard[line, index] != merged[i]) moved = true;
                        _gameBoard[line, index] = merged[i];
                    }
                    else
                    {
                        if (_gameBoard[index, line] != merged[i]) moved = true;
                        _gameBoard[index, line] = merged[i];
                    }
                }
            }

            if (moved) OnPropertyChanged(nameof(GameBoard));
            return moved;
        }

        private List<int> MergeLine(List<int> line)
        {
            var newLine = line.Where(x => x != 0).ToList();
            for (int i = 0; i < newLine.Count - 1; i++)
            {
                if (newLine[i] == newLine[i + 1])
                {
                    newLine[i] *= 2;
                    newLine[i + 1] = 0;
                }
            }
            return newLine.Where(x => x != 0).Concat(Enumerable.Repeat(0, 4)).ToList();
        }
}

public partial class SlidingTilePuzzlePage : ContentPage
{
    private readonly SlidingTilePuzzleViewModel _viewModel;
    private readonly int _tileSize = 120;

    public SlidingTilePuzzlePage()
    {
        InitializeComponent();

        _viewModel = new SlidingTilePuzzleViewModel();
        BindingContext = _viewModel;

        AddSwipeGestures();
    }

    private void AddSwipeGestures()
    {
        var directions = new (SwipeDirection dir, Func<bool> move)[]
        {
            (SwipeDirection.Up, _viewModel.MoveUp),
            (SwipeDirection.Down, _viewModel.MoveDown),
            (SwipeDirection.Left, _viewModel.MoveLeft),
            (SwipeDirection.Right, _viewModel.MoveRight)
        };

        foreach (var (dir, action) in directions)
        {
            var swipe = new SwipeGestureRecognizer { Direction = dir };
            swipe.Swiped += (s, e) =>
            {
                if (!action()) return;

                _viewModel.AddRandomTile();
                GameCanvas.InvalidateSurface();
            };
            
            Main.GestureRecognizers.Add(swipe);
        }
    }

    private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        var paint = new SKPaint { IsAntialias = true };
        var board = _viewModel.GameBoard;

        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                int value = board[r, c];
                var rect = new SKRect(c * _tileSize + 10, r * _tileSize + 10,
                    (c + 1) * _tileSize - 10, (r + 1) * _tileSize - 10);

                paint.Color = value switch
                {
                    0 => SKColors.LightGray,
                    2 => SKColors.Beige,
                    4 => SKColors.Bisque,
                    8 => SKColors.Orange,
                    16 => SKColors.OrangeRed,
                    32 => SKColors.Red,
                    64 => SKColors.DarkRed,
                    128 => SKColors.Gold,
                    256 => SKColors.Yellow,
                    512 => SKColors.GreenYellow,
                    1024 => SKColors.Green,
                    2048 => SKColors.Blue,
                    _ => SKColors.Black
                };

                canvas.DrawRoundRect(rect, 10, 10, paint);

                if (value == 0) continue;

                var textPaint = new SKPaint
                {
                    Color = SKColors.Black,
                    TextSize = 48,
                    IsAntialias = true,
                    TextAlign = SKTextAlign.Center,
                    Typeface = SKTypeface.Default
                };

                var textBounds = new SKRect();
                textPaint.MeasureText(value.ToString(), ref textBounds);
                canvas.DrawText(value.ToString(), rect.MidX, rect.MidY + textBounds.Height / 2, textPaint);
            }
        }
    }
}