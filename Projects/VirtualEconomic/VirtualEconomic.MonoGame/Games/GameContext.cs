using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VirtualEconomic.MonoGame;

public sealed class GameContext
{
	public Game Game { get; }
	public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;
	public ContentManager Content => Game.Content;

	public GameContext(Game game)
	{
		Game = game;
	}
}