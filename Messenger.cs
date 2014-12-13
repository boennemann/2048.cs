using System;

namespace _2048 {
  public static class Messenger {
    public static event PointHandler Points;

    public delegate void PointHandler(int points);

    public static void AddPoints(int points) {
      var evt = Points;
      if (evt != null) evt(points);
    }
  }
}

