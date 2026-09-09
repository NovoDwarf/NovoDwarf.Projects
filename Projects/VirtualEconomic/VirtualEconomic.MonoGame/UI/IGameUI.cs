using Microsoft.Xna.Framework;

namespace VirtualEconomic.MonoGame;

public interface IGameUI
{
	public void Initialize(GameContext context);
	public void Update(GameTime gameTime);
	public void Draw(GameTime gameTime);
}