using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Powderkeg;

public class Main : Game
{
private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private const int GridWidth = 180;
    private const int GridHeight = 90;

    private Material[,] _grid = new Material[GridWidth, GridHeight];
    private Color[] _pixelColors = new Color[GridWidth * GridHeight];
    private Texture2D _simTexture;

    private Material CurrentMaterial;

    public Main()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _simTexture = new Texture2D(GraphicsDevice, GridWidth, GridHeight);
    }

    protected override void Update(GameTime gameTime)
    {
        SpawnMaterial();
        UpdateCell();

        base.Update(gameTime);
    }

    private void SpawnMaterial()
    {
        var mouse = Mouse.GetState();
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.D0)) CurrentMaterial = Material.Air;
        if (keyboard.IsKeyDown(Keys.D1)) CurrentMaterial = Material.Sand;
        if (keyboard.IsKeyDown(Keys.D2)) CurrentMaterial = Material.Water;

        if (mouse.LeftButton == ButtonState.Pressed)
        {
            var viewport = GraphicsDevice.Viewport;

            float scaleX = (float)GridWidth / viewport.Width;
            float scaleY = (float)GridHeight / viewport.Height;

            int mouseX = (int)(mouse.X * scaleX);
            int mouseY = (int)(mouse.Y * scaleY);

            if (mouseX >= 0 && mouseX < GridWidth && mouseY >= 0 && mouseY < GridHeight)
            {
                _grid[mouseX, mouseY] = CurrentMaterial;
            }
        }
    }

    private void UpdateCell()
    {
        for (int y = GridHeight - 2; y >= 0; y--)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                Material currentPixel = _grid[x, y];
                var properties = MaterialRegistry.Get(currentPixel);

                if (properties.Phase == Phase.Empty) continue;

                int bestX = x;
                int bestY = y;
                int lowestCost = int.MaxValue;

                int kxStart = -1;
                int kxEnd = 1;
                int kxStep = 1;

                if (Random.Shared.Next(2) == 0)
                {
                    kxStart = 1;
                    kxEnd = -1;
                    kxStep = -1;
                }

                for (int ky = -1; ky <= 1; ky++)
                {
                    for (int kx = kxStart; kx != kxEnd + kxStep; kx += kxStep)
                    {
                        if (kx == 0 && ky == 0) continue;

                        int checkX = x + kx;
                        int checkY = y + ky;

                        if (checkX < 0 || checkX >= GridWidth || checkY < 0 || checkY >= GridHeight) continue;

                        Material neighborMaterial = _grid[checkX, checkY];
                        var neighborProperties = MaterialRegistry.Get(neighborMaterial);

                        int cost = GetUnifiedMovementCost(properties, kx, ky, neighborProperties);

                        if (cost < lowestCost)
                        {
                            lowestCost = cost;
                            bestX = checkX;
                            bestY = checkY;
                        }
                    }
                }

                ExecuteUniversalMove(x, y, bestX, bestY, currentPixel);
            }
        }
    }

    private int GetUnifiedMovementCost(MaterialProperties actorProperties, int dx, int dy, MaterialProperties neighborProperties)
    {
        if (dy < 0)
        {
            return int.MaxValue;
        }

        if (dy == 0)
        {
            if (actorProperties.Phase == Phase.Liquid && neighborProperties.Phase == Phase.Empty)
            {
                return 20;
            }
            return int.MaxValue;
        }

        int priorityBonus = (dx == 0) ? 0 : 2;

        if (neighborProperties.Phase == Phase.Empty)
        {
            return 1 + priorityBonus;
        }

        if (actorProperties.Density > neighborProperties.Density)
        {
            return 10 + Math.Abs(actorProperties.Density - neighborProperties.Density) + priorityBonus;
        }

        return int.MaxValue;
    }

    private void ExecuteUniversalMove(int startX, int startY, int targetX, int targetY, Material actor)
    {
        if (startX == targetX && startY == targetY) return;

        Material destinationContent = _grid[targetX, targetY];
        var actorProperties = MaterialRegistry.Get(actor);
        var destinationProperties = MaterialRegistry.Get(destinationContent);

        if (startY == targetY)
        {
            _grid[startX, startY] = destinationContent;
            _grid[targetX, targetY] = actor;
        }
        else if (destinationProperties.Phase != Phase.Empty && actorProperties.Density > destinationProperties.Density)
        {
            _grid[startX, startY] = destinationContent;
            _grid[targetX, targetY] = actor;
        }
        else
        {
            _grid[startX, startY] = destinationContent;
            _grid[targetX, targetY] = actor;
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                int index = y * GridWidth + x;
                _pixelColors[index] = MaterialRegistry.Get(_grid[x, y]).Color;
            }
        }

        _simTexture.SetData(_pixelColors);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        Rectangle destinationRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        _spriteBatch.Draw(_simTexture, destinationRect, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
