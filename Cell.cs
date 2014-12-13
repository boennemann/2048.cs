﻿using System;

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
      var output = "";
      if (this.spawning) {
        output += "N";
        this.spawning = false;
      }
      return output + this.value.ToString();
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
      // emit event for bonus points
      return base.AddressedBy(other);
    }

    public override string ToString() {
      return "B";
    }
  }

  public class BlockCell : EmptyCell {
    public override CellAction AddressedBy(Cell other) {
      return CellAction.Block;
    }

    public override string ToString() {
      return "X";
    }
  }
}

