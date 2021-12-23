namespace AdventOfCode.Year2021.Day17
{
    public class TrajectorySolver : ISolver
    {
        public void SolvePartOne(List<string> data)
        {
            BoundingBox targetArea = Parse(data);

            var validArcs = from vel in Velocities(new Vector2(-1000, -1000), new Vector2(1001, 1001))
                            let arc = Arc.GenerateArc(vel, targetArea)
                            where arc.HitTarget
                            select arc;

            Arc highestArc = validArcs.MaxBy(arc => arc.HighestPoint);

            Console.WriteLine(targetArea);
            Console.WriteLine(highestArc);
        }

        public void SolvePartTwo(List<string> data)
        {
            BoundingBox targetArea = Parse(data);

            var validArcs = from vel in Velocities(new Vector2(-1000, -1000), new Vector2(1001, 1001))
                            let arc = Arc.GenerateArc(vel, targetArea)
                            where arc.HitTarget
                            select arc;
            Console.WriteLine(validArcs.Count());
        }

        private static BoundingBox Parse(List<string> data)
        {
            string[] points = data[0].TrimStart("target area: ".ToCharArray()).Split(',');
            string xRange = points[0].Trim().TrimStart("x=".ToCharArray());
            string yRange = points[1].Trim().TrimStart("y=".ToCharArray());

            var xPoints = from str in xRange.Split("..")
                          select int.Parse(str);

            var yPoints = from str in yRange.Split("..")
                          select int.Parse(str);

            Vector2 topLeft = new Vector2(xPoints.Min(), yPoints.Max());
            Vector2 bottomRight = new Vector2(xPoints.Max(), yPoints.Min());

            return new BoundingBox(topLeft, bottomRight);
        }

        private static IEnumerable<Vector2> Velocities(Vector2 min, Vector2 max)
        {
            for (int x = min.X; x < max.X; x++)
            {
                for (int y = min.Y; y < max.Y; y++)
                {
                    yield return new Vector2(x, y);
                }
            }
        }

        internal struct Vector2
        {
            public int X { get; }
            public int Y { get; }
            public Vector2(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static Vector2 operator +(Vector2 a, Vector2 b)
            {
                return new Vector2(a.X + b.X, a.Y + b.Y);
            }

            public override string ToString()
            {
                return $"({X},{Y})";
            }
        }

        internal struct BoundingBox
        {
            public Vector2 TopLeft { get; }
            public Vector2 BottomRight { get; }

            public BoundingBox(Vector2 topLeft, Vector2 bottomRight)
            {
                TopLeft = topLeft;
                BottomRight = bottomRight;
            }

            public bool Contains(Vector2 point)
            {
                return (TopLeft.X <= point.X && point.X <= BottomRight.X) && (BottomRight.Y <= point.Y && point.Y <= TopLeft.Y);
            }

            public override string ToString()
            {
                return $"{TopLeft} to {BottomRight}";
            }
        }

        internal class Arc
        {
            public Vector2 StartingVelocity { get; }
            public List<Vector2> Points { get; }
            public bool HitTarget { get; }

            public int HighestPoint => (from point in Points select point.Y).Max();

            internal Arc(Vector2 startingVelocity, List<Vector2> points, bool hitTarget)
            {
                StartingVelocity = startingVelocity;
                Points = points;
                HitTarget = hitTarget;
            }

            public static Arc GenerateArc(Vector2 initialVelocity, BoundingBox target)
            {
                List<Vector2> points = new();
                Vector2 position = new Vector2(0, 0);
                points.Add(position);
                Vector2 velocity = initialVelocity;
                var done = false;
                var found = false;
                while (!done)
                {
                    int xDelta = Math.Sign(velocity.X) switch
                    {
                        -1 => 1,
                        0 => 0,
                        1 => -1,
                        _ => throw new InvalidDataException()
                    };

                    position += velocity;
                    velocity = new Vector2(velocity.X + xDelta, velocity.Y - 1);

                    points.Add(position);

                    if (target.Contains(position))
                    {
                        done = true;
                        found = true;
                    }

                    if (position.X < target.TopLeft.X && Math.Sign(velocity.X) != 1)
                    {
                        // if left of and not moving right
                        done = true;
                    }
                    else if (target.BottomRight.X < position.X && Math.Sign(velocity.X) != -1)
                    {
                        // if right of and not moving left
                        done = true;
                    }
                    else if (position.Y < target.BottomRight.Y && Math.Sign(velocity.Y) == -1)
                    {
                        done = true;
                    }
                }

                return new Arc(initialVelocity, points, found);
            }

            public override string ToString()
            {
                return $"<Arc: Num Points: {Points.Count}, Hit Target?: {HitTarget}, Highest Point: {HighestPoint}, Points: {string.Join('|', Points)})>";
            }
        }
    }
}