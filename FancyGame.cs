using System;

namespace _2048 {
  public class FancyGame : Game {
    public FancyGame() : base() {}
    public FancyGame(int x, int y) : base(x, y) {}

    protected override void Spwan() {
      base.Spwan();

      var list = GetAvailableCells();
      Random random = new Random();

      Cell cell;
      var luck = random.Next(0, 101);

      if (luck < 5) {
        cell = new BonusCell();
      } else if (luck > 99) {
        cell = new BlockCell();
      } else {
        return;
      }

      Position pos = list[random.Next(0, list.Count)];
      field[pos.x, pos.y] = cell;
    }
  }
}

