using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LunarLander.Terrain
{
    internal class Lines
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
                points[i].Position = new Vector3(line.startingPosition.X, line.startingPosition.Y, 0);
                if (line.is_safe_zone)
                {
                    points[i].Color = landingColor;
                }
                else
                {
                    points[i].Color = Color.White;
                }

                points[i + 1].Position = new Vector3(line.endingPosition.X, line.endingPosition.Y, 0);
                if (line.is_safe_zone)
                {
                    points[i + 1].Color = landingColor;
                }
                else
                {
                    points[i + 1].Color = Color.White;
                }

                i += 2;
            }
            return points;
        }
    }
}
