// Generated code -- http://www.redblobgames.com/grids/hexagons/

using System;
using System.Linq;
using System.Collections.Generic;

public struct Point
{
    public Point(double x, double y)
    {
        this.x = x;
        this.y = y;
    }
    public readonly double x;
    public readonly double y;
}

public enum GameArea {Board, WhiteBase, BlackBase};

public struct Hex : IEquatable<Hex>
{
    public Hex(int q, int r, int s, int y, GameArea area)
    {
        this.q = q;
        this.r = r;
        this.s = s;
		this.y = y;
        this.area = area;
        if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
    }
    
    public Hex(int q, int r, int s, int y) {
		this.q = q;
        this.r = r;
        this.s = s;
		this.y = y;
        this.area = GameArea.Board;
        if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
	}

	public Hex(int q, int r, int s) {
		this.q = q;
        this.r = r;
        this.s = s;
		this.y = 0;
        this.area = GameArea.Board;
        if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
	}
	public Hex(int q, int r) {
		this.q = q;
        this.r = r;
        this.s =  -1 * (q+r);
		this.y = 0;
        this.area = GameArea.Board;
        if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
	}

    public readonly int q;
    public readonly int r;
    public readonly int s;
	public readonly int y;
    public readonly GameArea area;

    public Hex Up()
    {
        return new Hex(q, r, s, y + 1);
    }

    public Hex Add(Hex b)
    {
        return new Hex(q + b.q, r + b.r, s + b.s);
    }


    public Hex Subtract(Hex b)
    {
        return new Hex(q - b.q, r - b.r, s - b.s);
    }

    public bool Equals(Hex b)
    {
        return q == b.q && r == b.r && s == b.s && y == b.y && area == b.area;
    }

    public bool InSameHexIgnoringY(Hex b)
    {
        return q == b.q && r == b.r && s == b.s;
    }

    public Hex Scale(int k)
    {
        return new Hex(q * k, r * k, s * k);
    }


    public Hex RotateLeft()
    {
        return new Hex(-s, -q, -r);
    }


    public Hex RotateRight()
    {
        return new Hex(-r, -s, -q);
    }

    static public List<Hex> directions = new List<Hex>{new Hex(1, 0, -1), new Hex(1, -1, 0), new Hex(0, -1, 1), new Hex(-1, 0, 1), new Hex(-1, 1, 0), new Hex(0, 1, -1)};

    static public Hex Direction(int direction)
    {
        return Hex.directions[direction];
    }


    public Hex Neighbor(int direction)
    {
        return Add(Hex.Direction(direction));
    }

    public List<Hex> Neighbors()
    {
        return new List<Hex>() {
            this.Neighbor(0),
            this.Neighbor(1),
            this.Neighbor(2),
            this.Neighbor(3),
            this.Neighbor(4),
            this.Neighbor(5)
        };
    }

    static public List<Hex> diagonals = new List<Hex>{new Hex(2, -1, -1), new Hex(1, -2, 1), new Hex(-1, -1, 2), new Hex(-2, 1, 1), new Hex(-1, 2, -1), new Hex(1, 1, -2)};

    public Hex DiagonalNeighbor(int direction)
    {
        return Add(Hex.diagonals[direction]);
    }


    public int Length()
    {
        return (int)((Math.Abs(q) + Math.Abs(r) + Math.Abs(s)) / 2);
    }


    public int Distance(Hex b)
    {
        return Subtract(b).Length();
    }

	public override String ToString()
	{
		return "{" + q + ", " + r + ", " + s + ", " + y + "}";
	}

    public Point ToPixel(double size)
    {
        double x = size * (Math.Sqrt(3) * q  + Math.Sqrt(3)/2 * r);
    	double y = size * (3.0/2 * r);
        return new Point(x, y);
    }

    public static double HexLongWidthForSize(double size)
    {
        return 2*size;
    }

    public static double HexShortWidthForSize(double size)
    {
        return Math.Sqrt(3) * size;
    }

    public static Hex LongestHex(List<Hex> hexes)
    {
        Hex hex = new Hex(0,0);
        foreach (Hex h in hexes) {
            hex = h.Length() > hex.Length() ? h : hex;
        }
        return hex;
    }
}
