using System;

namespace _2048 {
  public static class Messenger {
    public static event PointHandler Points;

    public delegate void PointHandler(int points);

    public static void AddPoints(int points) {
      var evt = Points;
      if (evt != null) evt(points);
    }

    public static event EndHandler End;

    public delegate void EndHandler(Boolean won);

    public static void EndGame(Boolean won) {
      var evt = End;
      if (evt != null) evt(won);
    }
  }
}

