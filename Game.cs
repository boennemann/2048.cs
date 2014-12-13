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

    private int points = 0;

    private Boolean gameover = false;

    private string header = "\n" +
      "\t _,  _, . ,  _,\n" +
      "\t'_) |.| |_| (_)\n" +
      "\t/_. |_|   | (_)\n\n\n";

    private string footer = "ESC to Exit\t R to Restart\t Arrows/WASD to Move\n";

    public Game() : this(4,4) {}

    public Game(int width, int height) {
      field = new Cell[width, height];

      for (var i = 0; i < field.GetLength(0); i++) {
        for (var j = 0; j < field.GetLength(1); j++) {
          field [i, j] = new EmptyCell();
        }
      }

      Messenger.Points += HandlePoints;
      Messenger.End += HandleEnd;
    }

    private void HandlePoints(int points) {
      this.points += points;
    }

    private void Won() {
      Console.Clear();
      Console.WriteLine("{0}Congrats! You won with {1} points!\n{2}", this.header, this.points, this.footer);
      this.gameover = true;
    }

    private void Lost() {
      Console.Clear();
      Console.WriteLine("{0}Oh Noes :( You lost with {1} points!\n{2}", this.header, this.points, this.footer);
      this.gameover = true;
    }

    private void HandleEnd(Boolean won) {
      if (won) {
        Won();
        return;
      }
      Lost();
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

      if (list.Count == 0 && IsGameOver()) {
        Messenger.EndGame(false);
        return;
      }

      Random random = new Random();
      Position pos = list[random.Next(0, list.Count)];

      field[pos.x, pos.y] = new Cell();
    }

    protected virtual Boolean IsGameOver() {
      Direction[] all = {
        Direction.Up,
        Direction.Down,
        Direction.Left,
        Direction.Right
      };  


      var movement = false;
      foreach (Direction dir in all) {
        var columns = GetColumns(dir);
        for (var i = 0; i < columns.Count; i++) {
          movement = Shift(columns [i], true) || movement;
        }
      }
    
      return !movement;
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

    protected Cell[,] Copy() {
      var width = field.GetLength(0);
      var height = field.GetLength(1);
      var copyField = new Cell[width, height];

      for (var i = 0; i < width; i++) {
        for (var j = 0; j < height; j++) {
          copyField[i, j] = field[i, j].GetCopy();
        }
      }

      return copyField;
    }

    protected Boolean Shift(List<Position> column, Boolean dryRun) {
      var alreadyMerged = new List<Cell>();
      var movement = false;

      var usedField = dryRun ? Copy() : field;

      for (var i = 1; i < column.Count; i++) {
        for (var j = i; j > 0; j--) {
          var cur = column [j];
          var next = column [j - 1];
          var result = usedField[next.x, next.y].AddressedBy(usedField[cur.x, cur.y]);

          if (result == CellAction.Swap) {
            var nextCell = usedField[next.x, next.y];
            usedField[next.x, next.y] = usedField[cur.x, cur.y];
            usedField[cur.x, cur.y] = nextCell;
            movement = true;
            continue;
          }

          if (result == CellAction.Merge) {
            var target = usedField[next.x, next.y];
            if (alreadyMerged.Contains(target) || alreadyMerged.Contains(usedField[cur.x, cur.y])) continue;
            target.value = target.value * 2;
            Messenger.AddPoints(target.value);

            if (target.value >= 2048) {
              Messenger.EndGame(true);
            }

            alreadyMerged.Add(target);
            usedField[cur.x, cur.y] = new EmptyCell();
            movement = true;
          }
        }
      }

      return movement;
    }

    protected Boolean Shift(List<Position> column) {
      return Shift(column, false);
    }

    public virtual void Move(Direction dir) {
      var movement = false;
      if (dir != Direction.Still) {
        var columns = GetColumns(dir);
        for (var i = 0; i < columns.Count; i++) {
          movement = Shift(columns [i]) || movement;
        }
       }
      if (movement || dir == Direction.Still) {
        Spwan();
      }
      Print(dir);
    }

    public void Move() {
      Move(Direction.Still);
    }

    public void Print(Direction dir) {
      if (gameover) return;

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

      output += this.points + " Points \t Went " + dir;
      output += this.footer;

      Console.Clear();
      Console.WriteLine(output);
    }
  }
}

