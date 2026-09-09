using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VirtualEconomic.MonoGame;

public class VirtualEconomicGame : Game
{
	private readonly GraphicsDeviceManager _graphics;
	private readonly IGameUI _ui;
	
	public VirtualEconomicGame(IGameUI ui)
	{
		_ui = ui;
		_graphics = new GraphicsDeviceManager(this);
		
		Content.RootDirectory = "Content";
		
		IsMouseVisible = true;
	}
	
	public GameContext Context { get; private set; } = null!;
	
	protected override void Initialize()
	{
		Context = new GameContext(this);

		_ui.Initialize(Context);
		
		base.Initialize();
	}

	protected override void LoadContent()
	{
		
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
		    Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();
		
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);
		
		base.Draw(gameTime);
	}
}