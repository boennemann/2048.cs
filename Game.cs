using System;
using System.Linq;
using System.Collections.Generic;

namespace _2048 {

  public struct Position {
    public int x;
    public int y;
    public Position(int x, int y) {
      this.x = x;
      this.y = y;
    }
    public override string ToString() {
      return "{x: " + x + ", y: " + y + "}";
    }
  }

  public enum Direction {
    Still,
    Up,
    Down,
    Left,
    Right
  }

  public class Game {
    protected Cell[,] field;

    public Game() : this(4,4) {}

    public Game(int width, int height) {
      field = new Cell[width, height];

      for (var i = 0; i < field.GetLength(0); i++) {
        for (var j = 0; j < field.GetLength(1); j++) {
          field [i, j] = new EmptyCell();
        }
      }
    }

    protected virtual List<Position> GetAvailableCells(Type type) {
      var list = new List<Position>();

      for (var i = 0; i < field.GetLength(0); i++) {
        for (var j = 0; j < field.GetLength(1); j++) {
          if (field [i, j].GetType() == type) list.Add(new Position(i, j));
        }
      }

      return list;
    }

    protected virtual List<Position> GetAvailableCells() {
      return GetAvailableCells(typeof(EmptyCell));
    }

    protected virtual void Spwan() {
      var list = GetAvailableCells();
      Random random = new Random();
      Position pos = list[random.Next(0, list.Count)];

      field[pos.x, pos.y] = new Cell();
    }

    protected List<Position> GetColumn(int i, int row, Direction dir) {
      var column = new List<Position>();

      for (var j = 0; j < field.GetLength(row); j++) {
        if (row == 0) {
          column.Add(new Position(i, j));
        } else {
          column.Add(new Position(j, i));
        }
      }

      if (dir == Direction.Down || dir == Direction.Right) {
        column.Reverse();
      }

      return column;
    }

    protected virtual List<List<Position>> GetColumns(Direction dir) {
      var list = new List<List<Position>>();

      var row = 1;

      if (dir == Direction.Down || dir == Direction.Up) {
        row = 0;
      }

      for (var i = 0; i < field.GetLength(row); i++) {
        // ~row: converts 1 to 0 and 0 to 1
        list.Add(GetColumn(i, (~(row - 1) + 1), dir));
      }

      return list;
    }

    protected void Shift(List<Position> column) {
      var alreadyMerged = new List<Cell>();

      for (var i = 1; i < column.Count; i++) {
        for (var j = i; j > 0; j--) {
          var cur = column [j];
          var next = column [j - 1];
          var result = field [next.x, next.y].AddressedBy(field [cur.x, cur.y]);

          if (result == CellAction.Swap) {
            var nextCell = field [next.x, next.y];
            field [next.x, next.y] = field [cur.x, cur.y];
            field [cur.x, cur.y] = nextCell;
            continue;
          }

          if (result == CellAction.Merge) {
            var target = field[next.x, next.y];
            if (alreadyMerged.Contains(target) || alreadyMerged.Contains(field[cur.x, cur.y])) continue;
            target.value = target.value * 2;
            alreadyMerged.Add(target);
            field [cur.x, cur.y] = new EmptyCell();
          }
        }
      }
    }

    public virtual void Move(Direction dir) {
      if (dir != Direction.Still) {
        var columns = GetColumns(dir);
        for (var i = 0; i < columns.Count; i++) {
          Shift(columns [i]);
        }
       }
      Spwan();
      Print();
      Console.WriteLine("Went " + dir);
    }

    public void Move() {
      Move(Direction.Still);
    }

    public void Print() {
      string output = "\n" +
        "\t _,  _, . ,  _,\n" +
        "\t'_) |.| |_| (_)\n" +
        "\t/_. |_|   | (_)\n\n\n";

      var line = string.Concat(Enumerable.Repeat("\t_\t", field.GetLength(0))) + "\n";
      output += line;
      for (var i = 0; i < field.GetLength(0); i++) {
        output += "\n|";
       
        for (var j = 0; j < field.GetLength(1); j++) {
          output += "\t" + field [i, j] + "\t";
        }
        output += "|\n\n";
      }
      output += line;

      Console.Clear();
      Console.WriteLine(output);
    }
  }
}

