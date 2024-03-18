using LunarLander;
using Maze.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CS5410
{
    public class GamePlayView : GameStateView
    {
        private BasicEffect m_effect;
        private SpriteFont m_font;
        private Random m_random = new Random();
        private Ship m_ship; // https://opengameart.org/content/rocket
        private Texture2D m_texShip;
        private VertexPositionColor[] m_vertsLineStrip;
        VertexPositionColor[] verticesToDraw;

        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/menu");
            m_effect = new BasicEffect(m_graphics.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up),

                Projection = Matrix.CreateOrthographicOffCenter(
                    0, m_graphics.GraphicsDevice.Viewport.Width,
                    m_graphics.GraphicsDevice.Viewport.Height, 0,   // doing this to get it to match the default of upper left of (0, 0)
                    0.1f, 2)
            };

            GenerateTerrain();

            m_texShip = contentManager.Load<Texture2D>("Images/rocket");

            m_ship = new Ship(new Vector2(50, 50), new Vector2(0, 0), new Vector2(1, 0), false);

        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                // TODO: remove
                GenerateTerrain();
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.GamePlay;
        }

        public override void render(GameTime gameTime)
        {
            Draw();
            m_spriteBatch.Begin();

            float rotationAngle = (float)Math.Atan2(m_ship.direction.Y, m_ship.direction.X);
            Vector2 origin = new Vector2(m_texShip.Width / 2f, m_texShip.Height / 2f);
            m_spriteBatch.Draw(
                m_texShip, 
                new Vector2(m_ship.position.X, m_ship.position.Y),
                null, 
                Color.White, 
                rotationAngle, 
                origin, 
                0.5f, 
                SpriteEffects.None, 
                0f);

            string velocityText = $"Velocity: {m_ship.velocity.X:F2}, {m_ship.velocity.Y:F2}";
            Vector2 position = new Vector2(10, 10); // Example position for the text
            m_spriteBatch.DrawString(m_font, velocityText, position, Color.White);

            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
            m_ship.Update(gameTime);
        }

        public void GenerateTerrain()
        {
            List<Vector3> points = new List<Vector3>();

            int yStartRange = (int)(m_graphics.PreferredBackBufferHeight - m_graphics.PreferredBackBufferHeight * 0.60);
            int yEndRange = (int)(m_graphics.PreferredBackBufferHeight * 0.75);
            int xLeftSafeZone = m_graphics.PreferredBackBufferWidth - (int)(m_graphics.PreferredBackBufferWidth * 0.85);
            int xRightSafeZone = (int)(m_graphics.PreferredBackBufferWidth * 0.85);
            const int SAFE_ZONE_LENGTH = 200;
            const int MIN_DISTANCE_BETWEEN = 200;

            int firstSafeZoneX = m_random.Next(xLeftSafeZone, xRightSafeZone - SAFE_ZONE_LENGTH);
            int firstSafeZoneY = m_random.Next(yStartRange, yEndRange);

            int leftSpace = firstSafeZoneX - xLeftSafeZone;
            int rightSpace = xRightSafeZone - (firstSafeZoneX + SAFE_ZONE_LENGTH);

            if (leftSpace > rightSpace)
            {
                xRightSafeZone = firstSafeZoneX - MIN_DISTANCE_BETWEEN - SAFE_ZONE_LENGTH;
            }
            else
            {
                xLeftSafeZone = firstSafeZoneX + SAFE_ZONE_LENGTH + MIN_DISTANCE_BETWEEN;
            }

            int secondSafeZoneX = m_random.Next(xLeftSafeZone, xRightSafeZone);
            int secondSafeZoneY = m_random.Next(yStartRange, yEndRange);

            Lines lines = new Lines();

            // Start
            points.Add(new Vector3(0, m_graphics.PreferredBackBufferHeight / 2, 0));

            Line startLine;
            Line firstSafeLine;
            Line connectorSafeLine;
            Line secondSafeLine;
            Line lastLine;
            if (leftSpace > rightSpace)
            {
                startLine = new Line(
                    new Vector3(0, m_random.Next(yStartRange, yEndRange), 0),
                    new Vector3(secondSafeZoneX, secondSafeZoneY, 0),
                    false);
                firstSafeLine = new Line(
                    new Vector3(secondSafeZoneX, secondSafeZoneY, 0),
                    new Vector3(secondSafeZoneX + SAFE_ZONE_LENGTH, secondSafeZoneY, 0),
                    true);
                connectorSafeLine = new Line(
                    new Vector3(secondSafeZoneX + SAFE_ZONE_LENGTH, secondSafeZoneY, 0),
                    new Vector3(firstSafeZoneX, firstSafeZoneY, 0),
                    false);
                secondSafeLine = new Line(
                    new Vector3(firstSafeZoneX, firstSafeZoneY, 0),
                    new Vector3(firstSafeZoneX + SAFE_ZONE_LENGTH, firstSafeZoneY, 0),
                    true);
                lastLine = new Line(
                    new Vector3(firstSafeZoneX + SAFE_ZONE_LENGTH, firstSafeZoneY, 0),
                    new Vector3(m_graphics.PreferredBackBufferWidth, m_random.Next(yStartRange, yEndRange), 0),
                    false);
            }
            else
            {
                startLine = new Line(
                    new Vector3(0, m_graphics.PreferredBackBufferHeight / 2, 0),
                    new Vector3(firstSafeZoneX, firstSafeZoneY, 0),
                    false);
                firstSafeLine = new Line(
                    new Vector3(firstSafeZoneX, firstSafeZoneY, 0),
                    new Vector3(firstSafeZoneX + SAFE_ZONE_LENGTH, firstSafeZoneY, 0),
                    true);
                connectorSafeLine = new Line(
                    new Vector3(firstSafeZoneX + SAFE_ZONE_LENGTH, firstSafeZoneY, 0),
                    new Vector3(secondSafeZoneX, secondSafeZoneY, 0),
                    false);
                secondSafeLine = new Line(
                    new Vector3(secondSafeZoneX, secondSafeZoneY, 0),
                    new Vector3(secondSafeZoneX + SAFE_ZONE_LENGTH, secondSafeZoneY, 0),
                    true);
                lastLine = new Line(
                    new Vector3(secondSafeZoneX + SAFE_ZONE_LENGTH, secondSafeZoneY, 0),
                    new Vector3(m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight / 2, 0),
                    false);

            }
            lines.AddLine(startLine);
            lines.AddLine(firstSafeLine);
            lines.AddLine(connectorSafeLine);
            lines.AddLine(secondSafeLine);
            lines.AddLine(lastLine);

            // mid point


            // draw lines with safezone
            //m_vertsLineStrip = new VertexPositionColor[points.Count];
            //for (int i = 0; i < m_vertsLineStrip.Length; i++)
            //{
            //    m_vertsLineStrip[i].Position = points[i];
            //    m_vertsLineStrip[i].Color = Color.White;
            //}

            //m_vertsLineStrip = lines.getPointsLines();

            MidpointDisplacement(lines);

            List<VertexPositionColor> triangleVertices = new List<VertexPositionColor>();
            Color fillColor = Color.Black; // The color to fill the area with

            for (int i = 0; i < m_vertsLineStrip.Length - 1; i++)
            {
                // Current and next point in the line strip
                Vector3 currentPoint = m_vertsLineStrip[i].Position;
                Vector3 nextPoint = m_vertsLineStrip[i + 1].Position;

                // Corresponding points on the base line
                Vector3 currentBasePoint = new Vector3(currentPoint.X, m_graphics.PreferredBackBufferHeight, currentPoint.Z);
                Vector3 nextBasePoint = new Vector3(nextPoint.X, m_graphics.PreferredBackBufferHeight, nextPoint.Z);

                // Create two triangles to fill the area under the segment
                // Triangle 1
                triangleVertices.Add(new VertexPositionColor(currentPoint, fillColor));
                triangleVertices.Add(new VertexPositionColor(nextPoint, fillColor));
                triangleVertices.Add(new VertexPositionColor(currentBasePoint, fillColor));

                // Triangle 2
                triangleVertices.Add(new VertexPositionColor(nextPoint, fillColor));
                triangleVertices.Add(new VertexPositionColor(nextBasePoint, fillColor));
                triangleVertices.Add(new VertexPositionColor(currentBasePoint, fillColor));
            }

            // Convert the list to an array for drawing
            verticesToDraw = triangleVertices.ToArray();

        }

        private double GaussianRandomNumber()
        {
            double u1 = 1.0 - m_random.NextDouble(); // uniform(0,1] random doubles
            double u2 = 1.0 - m_random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                   Math.Sin(2.0 * Math.PI * u2); // random normal(0,1)
            return randStdNormal; // mean 0, variance 1
        }

        private void MidpointDisplacement(Lines lines)
        {
            const int N_LEVELS = 8;
            float s = 2f;
            Lines temp_lines = new Lines(lines);
            for (int i = 0; i < N_LEVELS; i++)
            {
                foreach (Line line in lines.m_lines)
                {
                    if (line.is_safe_zone) continue;
                    else
                    {
                        int middlePointX = (int)(0.5 * (line.startingPosition.X + line.endingPosition.X));
                        double r = s * GaussianRandomNumber() * (int)(Math.Abs(line.endingPosition.X - line.startingPosition.X));
                        int y = (int)(0.5 * (line.startingPosition.Y + line.endingPosition.Y) + r);
                        y = Math.Min(y, m_graphics.PreferredBackBufferHeight - 1);
                        y = Math.Max(y, (int)(m_graphics.PreferredBackBufferHeight * 0.30));

                        Line line1 = new Line(
                            new Vector3(line.startingPosition.X, line.startingPosition.Y, 0),
                            new Vector3(middlePointX, y, 0),
                            false);
                        Line line2 = new Line(
                            new Vector3(middlePointX, y, 0),
                            new Vector3(line.endingPosition.X, line.endingPosition.Y, 0),
                            false);
                        temp_lines.replaceLineWithLines(
                            temp_lines.m_lines.IndexOf(line),
                            line1,
                            line2);
                    }
                }
                lines = new Lines(temp_lines);
                s *= 0.65f;
            }
            m_vertsLineStrip = lines.getPointsLines();
        }

        public void Draw()
        {
            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                m_graphics.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    verticesToDraw,
                    0,
                    verticesToDraw.Length / 3); // The number of triangles is the number of vertices divided by 3

                // Draw the line segments
                m_graphics.GraphicsDevice.DrawUserPrimitives(
                PrimitiveType.LineStrip,
                m_vertsLineStrip,
                0,
                m_vertsLineStrip.Length - 1);

                
            }

        }

        public class Lines
        {
            public List<Line> m_lines;
            public Lines() 
            {
                m_lines = new List<Line>();
            }

            // Copy constructor
            public Lines(Lines original)
            {
                // Create a new List<Line>, adding all the Line references from the original list
                m_lines = new List<Line>(original.m_lines);
            }

            public void AddLine(Line line)
            {
                m_lines.Add(line);
            }

            

            public void replaceLineWithLines(int position, Line line1, Line line2)
            {
                m_lines.RemoveAt(position);

                m_lines.Insert(position, line2);
                m_lines.Insert(position, line1);
            }

            public VertexPositionColor[] getPointsLines() 
            {
                VertexPositionColor[] points = new VertexPositionColor[m_lines.Count * 2];
                int i = 0;

                Color landingColor = Color.Red;

                foreach (Line line in m_lines)
                {
                    points[i].Position = line.startingPosition;
                    if (line.is_safe_zone)
                    {
                        points[i].Color = landingColor;
                    }
                    else 
                    {
                        points[i].Color = Color.White;
                    }

                    points[i+1].Position = line.endingPosition;
                    if (line.is_safe_zone)
                    {
                        points[i+1].Color = landingColor;
                    }
                    else
                    {
                        points[i+1].Color = Color.White;
                    }

                    i += 2;
                }
                return points;
            }
        }

        public class Line
        {
            public Vector3 startingPosition;
            public Vector3 endingPosition;
            public bool is_safe_zone;

            public Line(Vector3 startingPosition, Vector3 endingPosition, bool is_safe_zone)
            {
                this.startingPosition = startingPosition;
                this.endingPosition = endingPosition;
                this.is_safe_zone = is_safe_zone;
            }
        }



    }
}
