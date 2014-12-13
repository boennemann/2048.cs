using System;
using _2048;

class MainClass {
  public static void Main(string[] args) {
    var game = new Game(4, 4);

    game.Move();

    while(true) {
      switch (Console.ReadKey().Key) {
      case ConsoleKey.W:
      case ConsoleKey.UpArrow:
        game.Move(Direction.Up);
        break;
      case ConsoleKey.S:
      case ConsoleKey.DownArrow:
        game.Move(Direction.Down);
        break;
      case ConsoleKey.A:
      case ConsoleKey.LeftArrow:
        game.Move(Direction.Left);
        break;
      case ConsoleKey.D:
      case ConsoleKey.RightArrow:
        game.Move(Direction.Right);
        break;
      case ConsoleKey.Escape:
        Environment.Exit(0);
        break;
      }
    }
  }
}
