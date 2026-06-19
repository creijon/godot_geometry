using System.Collections.Generic;
using Godot;

public partial class BitmaskTest : Node
{
    List<uint> _bitList;
    Bitmask _bitmask;

    // Start is called before the first frame update
    public override void _Ready()
    {
        _bitList = new List<uint>();
        _bitmask = new Bitmask(_bitList, 0, 512);

        _bitmask.Set(63);
        bool b63 = _bitmask.Get(63);
        GD.Print("Set bit 63: " + (b63 ? "OK" : "FAIL"));
        _bitmask.Set(64);
        bool b64 = _bitmask.Get(64);
        GD.Print("Set bit 64: " + (b64 ? "OK" : "FAIL"));
        _bitmask.Clear(63);
        b63 = _bitmask.Get(63);
        GD.Print("Clear bit 63: " + (!b63 ? "OK" : "FAIL"));
        _bitmask.Clear(64);

        uint countAll = _bitmask.PopCount();
        GD.Print("Clear all: " + (countAll == 0 ? "OK" : "FAIL"));
        for (int i = 0; i < 100; i += 2)
        {
            _bitmask.Set(i);
        }
        countAll = _bitmask.PopCount();
        GD.Print("Set 50 even bits: " + (countAll == 50 ? "OK" : "FAIL"));

        uint count10 = _bitmask.PopCountBefore(10);
        GD.Print("Before 10: " + (count10 == 5 ? "OK" : "FAIL"));
        uint count20 = _bitmask.PopCountBefore(20);
        GD.Print("Before 20: " + (count20 == 10 ? "OK" : "FAIL"));
        uint count30 = _bitmask.PopCountBefore(30);
        GD.Print("Before 30: " + (count30 == 15 ? "OK" : "FAIL"));
        uint count100 = _bitmask.PopCountBefore(100);
        GD.Print("Before 100: " + (count100 == 50 ? "OK" : "FAIL"));

        count10 = _bitmask.PopCountAfter(10);
        GD.Print("After 10: " + (count10 == 44 ? "OK" : "FAIL"));
        count20 = _bitmask.PopCountAfter(20);
        GD.Print("After 20: " + (count20 == 39 ? "OK" : "FAIL"));
        count30 = _bitmask.PopCountAfter(30);
        GD.Print("After 30: " + (count30 == 34 ? "OK" : "FAIL"));
        count100 = _bitmask.PopCountAfter(100);
        GD.Print("After 100:" + (count100 == 0 ? "OK" : "FAIL"));

        _bitmask.Reset(false);
        for (int i = 1; i < 100; i += 2)
        {
            _bitmask.Set(i);
        }
        countAll = _bitmask.PopCount();
        GD.Print("Set 50 odd bits: " + (countAll == 50 ? "OK" : "FAIL"));

        count10 = _bitmask.PopCountBefore(10);
        GD.Print("Before 10: " + (count10 == 5 ? "OK" : "FAIL"));
        count20 = _bitmask.PopCountBefore(20);
        GD.Print("Before 20: " + (count20 == 10 ? "OK" : "FAIL"));
        count30 = _bitmask.PopCountBefore(30);
        GD.Print("Before 30: " + (count30 == 15 ? "OK" : "FAIL"));
        count100 = _bitmask.PopCountBefore(100);
        GD.Print("Before 100: " + (count100 == 50 ? "OK" : "FAIL"));

        count10 = _bitmask.PopCountAfter(10);
        GD.Print("After 10: " + (count10 == 45 ? "OK" : "FAIL"));
        count20 = _bitmask.PopCountAfter(20);
        GD.Print("After 20: " + (count20 == 40 ? "OK" : "FAIL"));
        count30 = _bitmask.PopCountAfter(30);
        GD.Print("After 30: " + (count30 == 35 ? "OK" : "FAIL"));
        count100 = _bitmask.PopCountAfter(100);
        GD.Print("After 100: " + (count100 == 0 ? "OK" : "FAIL"));

        _bitmask.Reset(true);
        countAll = _bitmask.PopCountBefore(512);
        GD.Print("Before 512:" + (countAll == 512 ? "OK" : "FAIL"));
        countAll = _bitmask.PopCount();
        GD.Print("Popcount:" + (countAll == 512 ? "OK" : "FAIL"));

        _bitmask.Clear(64);
        _bitmask.Clear(200);
        _bitmask.Clear(300);
        
        countAll = _bitmask.PopCountBefore(512);
        GD.Print("Before 512:" + (countAll == 509 ? "OK" : "FAIL"));
        countAll = _bitmask.PopCount();
        GD.Print("Popcount:" + (countAll == 509 ? "OK" : "FAIL"));
    }
}
