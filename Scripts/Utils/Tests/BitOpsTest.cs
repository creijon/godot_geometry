using Godot;

public partial class BitOpsTest : Node
{
    // Start is called before the first frame update
    public override void _Ready()
    {
        uint countAll;
        countAll = BitOps.PopCountBefore(0xFFFFFFFF, 0xFFFFFFFF, 0);
        GD.Print("0xFFFFFFFF, 0xFFFFFFFF before  0:" + (countAll == 0));

        countAll = BitOps.PopCountBefore(0xFFFFFFFF, 0xFFFFFFFF, 1);
        GD.Print("0xFFFFFFFF, 0xFFFFFFFF before  1:" + (countAll == 1));

        countAll = BitOps.PopCountBefore(0xFFFFFFFF, 0xFFFFFFFF, 2);
        GD.Print("0xFFFFFFFF, 0xFFFFFFFF before  2:" + (countAll == 2));

        countAll = BitOps.PopCountBefore(0xFFFFFFFF, 0xFFFFFFFF, 31);
        GD.Print("0xFFFFFFFF, 0xFFFFFFFF before 31:" + (countAll == 31));

        countAll = BitOps.PopCountBefore(0xFFFFFFFF, 0xFFFFFFFF, 32);
        GD.Print("0xFFFFFFFF, 0xFFFFFFFF before 32:" + (countAll == 32));

        countAll = BitOps.PopCountBefore(0xFFFFFFFF, 0xFFFFFFFF, 33);
        GD.Print("0xFFFFFFFF, 0xFFFFFFFF before 33:" + (countAll == 33));

        countAll = BitOps.PopCountBefore(0xFFFFFFFF, 0xFFFFFFFF, 63);
        GD.Print("0xFFFFFFFF, 0xFFFFFFFF before 63:" + (countAll == 63));


        countAll = BitOps.PopCountBefore(0x33333333, 0xCCCCCCCC, 0);
        GD.Print("0x33333333, 0xCCCCCCCC before  0:" + (countAll == 0));

        countAll = BitOps.PopCountBefore(0x33333333, 0xCCCCCCCC, 1);
        GD.Print("0x33333333, 0xCCCCCCCC before  1:" + (countAll == 1));

        countAll = BitOps.PopCountBefore(0x33333333, 0xCCCCCCCC, 2);
        GD.Print("0x33333333, 0xCCCCCCCC before  2:" + (countAll == 2));

        countAll = BitOps.PopCountBefore(0x33333333, 0xCCCCCCCC, 31);
        GD.Print("0x33333333, 0xCCCCCCCC before 15:" + (countAll == 16));

        countAll = BitOps.PopCountBefore(0x33333333, 0xCCCCCCCC, 32);
        GD.Print("0x33333333, 0xCCCCCCCC before 32:" + (countAll == 16));

        countAll = BitOps.PopCountBefore(0x33333333, 0xCCCCCCCC, 33);
        GD.Print("0x33333333, 0xCCCCCCCC before 33:" + (countAll == 16));

        countAll = BitOps.PopCountBefore(0x33333333, 0xCCCCCCCC, 63);
        GD.Print("0x33333333, 0xCCCCCCCC before 63:" + (countAll == 31));


        countAll = BitOps.PopCountBefore(0xCCCCCCCC33333333, 0);
        GD.Print("0xCCCCCCCC33333333 before  0:" + (countAll == 0));

        countAll = BitOps.PopCountBefore(0xCCCCCCCC33333333, 1);
        GD.Print("0xCCCCCCCC33333333 before  1:" + (countAll == 1));

        countAll = BitOps.PopCountBefore(0xCCCCCCCC33333333, 2);
        GD.Print("0xCCCCCCCC33333333 before  2:" + (countAll == 2));

        countAll = BitOps.PopCountBefore(0xCCCCCCCC33333333, 31);
        GD.Print("0xCCCCCCCC33333333 before 15:" + (countAll == 16));

        countAll = BitOps.PopCountBefore(0xCCCCCCCC33333333, 32);
        GD.Print("0xCCCCCCCC33333333 before 32:" + (countAll == 16));

        countAll = BitOps.PopCountBefore(0xCCCCCCCC33333333, 33);
        GD.Print("0xCCCCCCCC33333333 before 33:" + (countAll == 16));

        countAll = BitOps.PopCountBefore(0xCCCCCCCC33333333, 63);
        GD.Print("0xCCCCCCCC33333333 before 63:" + (countAll == 31));


    }
}
