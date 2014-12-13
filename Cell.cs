using System;

namespace _2048 {
  public enum CellAction {
    Block,
    Merge,
    Swap
  }

  public class Cell {
    public int value;
    private Boolean spawning = true;

    public Cell() {
      this.value = 2;
    }

    public Cell(int value) {
      this.value = value;
    }

    public virtual CellAction AddressedBy(Cell other) {
      if (this.value == other.value) {
        return CellAction.Merge;
      }
      return CellAction.Block;
    }

    public override string ToString() {
      var output = this.value.ToString();
      if (this.spawning) {
        output += " ✨";
        this.spawning = false;
      }
      return output;
    }

    public virtual Cell GetCopy() {
      Cell copy = (Cell)Activator.CreateInstance(this.GetType());
      copy.value = this.value;
      if (!this.spawning) copy.ToString();
      return copy;
    }
  }

  public class EmptyCell : Cell {
    public EmptyCell() : base(0) {}

    public override CellAction AddressedBy(Cell other) {
      if (other is EmptyCell) {
        return CellAction.Block;
      }
      return CellAction.Swap;
    }

    public override string ToString() {
      return "";
    }
  }

  public class BonusCell : EmptyCell {
    public override CellAction AddressedBy(Cell other) {
      var action = base.AddressedBy(other);
      if (action != CellAction.Block) {
        Messenger.AddPoints(other.value * 10);
      }
      return action;  
    }

    public override string ToString() {
      return "\ud83c\udf89";
    }
  }

  public class BlockCell : EmptyCell {
    public override CellAction AddressedBy(Cell other) {
      return CellAction.Block;
    }

    public override string ToString() {
      return "\ud83d\udca9";
    }
  }
}

