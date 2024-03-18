using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarLander.Terrain
{
    class Terrain
    {
        private BasicEffect m_effect;
        public VertexPositionColor[] m_vertsLineStrip;
        public List<Line> m_lines;
        VertexPositionColor[] verticesToDraw;
        private GraphicsDeviceManager m_graphics;
        Random m_random;

        public Terrain(GraphicsDeviceManager m_graphics, Random m_random) 
        {
            this.m_graphics = m_graphics;
            this.m_random = m_random;
            
            m_effect = new BasicEffect(m_graphics.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up),

                Projection = Matrix.CreateOrthographicOffCenter(
                    0, m_graphics.GraphicsDevice.Viewport.Width,
                    m_graphics.GraphicsDevice.Viewport.Height, 0,   // doing this to get it to match the default of upper left of (0, 0)
                    0.1f, 2)
            };
        }

        public List<Line> getLines()
        {
            if (m_lines == null)
            {
                return new List<Line>();
            }
            return m_lines;
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
                    new Vector2(0, m_random.Next(yStartRange, yEndRange)),
                    new Vector2(secondSafeZoneX, secondSafeZoneY),
                    false);
                firstSafeLine = new Line(
                    new Vector2(secondSafeZoneX, secondSafeZoneY),
                    new Vector2(secondSafeZoneX + SAFE_ZONE_LENGTH, secondSafeZoneY),
                    true);
                connectorSafeLine = new Line(
                    new Vector2(secondSafeZoneX + SAFE_ZONE_LENGTH, secondSafeZoneY),
                    new Vector2(firstSafeZoneX, firstSafeZoneY),
                    false);
                secondSafeLine = new Line(
                    new Vector2(firstSafeZoneX, firstSafeZoneY),
                    new Vector2(firstSafeZoneX + SAFE_ZONE_LENGTH, firstSafeZoneY),
                    true);
                lastLine = new Line(
                    new Vector2(firstSafeZoneX + SAFE_ZONE_LENGTH, firstSafeZoneY),
                    new Vector2(m_graphics.PreferredBackBufferWidth, m_random.Next(yStartRange, yEndRange)),
                    false);
            }
            else
            {
                startLine = new Line(
                    new Vector2(0, m_graphics.PreferredBackBufferHeight / 2),
                    new Vector2(firstSafeZoneX, firstSafeZoneY),
                    false);
                firstSafeLine = new Line(
                    new Vector2(firstSafeZoneX, firstSafeZoneY),
                    new Vector2(firstSafeZoneX + SAFE_ZONE_LENGTH, firstSafeZoneY),
                    true);
                connectorSafeLine = new Line(
                    new Vector2(firstSafeZoneX + SAFE_ZONE_LENGTH, firstSafeZoneY),
                    new Vector2(secondSafeZoneX, secondSafeZoneY),
                    false);
                secondSafeLine = new Line(
                    new Vector2(secondSafeZoneX, secondSafeZoneY),
                    new Vector2(secondSafeZoneX + SAFE_ZONE_LENGTH, secondSafeZoneY),
                    true);
                lastLine = new Line(
                    new Vector2(secondSafeZoneX + SAFE_ZONE_LENGTH, secondSafeZoneY),
                    new Vector2(m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight / 2),
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
                            new Vector2(line.startingPosition.X, line.startingPosition.Y),
                            new Vector2(middlePointX, y),
                            false);
                        Line line2 = new Line(
                            new Vector2(middlePointX, y),
                            new Vector2(line.endingPosition.X, line.endingPosition.Y),
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
            m_lines = lines.m_lines;
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
    }

   
}
