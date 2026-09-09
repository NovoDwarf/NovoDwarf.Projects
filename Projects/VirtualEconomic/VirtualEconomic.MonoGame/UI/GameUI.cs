using Gum;
using Gum.Forms;
using Microsoft.Xna.Framework;

namespace VirtualEconomic.MonoGame;

public sealed class GameUI : IGameUI
{
	private GameContext _context = null!;
	
	public void Initialize(GameContext context)
	{
		_context = context;

		GumService.Default.Initialize(context.Game, DefaultVisualsVersion.V3);
	}

	public void Update(GameTime gameTime)
	{
		
	}

	public void Draw(GameTime gameTime)
	{
		
	}
}