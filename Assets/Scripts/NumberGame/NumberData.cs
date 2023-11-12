using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberData : Data, IComparable<NumberData>
{

    public byte number;

    public NumberData(byte number)
    {
        this.number = number;
    }

    public override string ToString()
    {
        return this.number.ToString();
    }

    // TODO : Not used
    public int CompareTo(NumberData number)
    {
        return this.number.CompareTo(number);
    }

}
