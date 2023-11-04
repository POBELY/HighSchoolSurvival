using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolData : Data
{

    public enum SYMBOL { ROUND, TRIANGLE, SQUARE };

    public static Dictionary<SYMBOL, string> symbolsName = new Dictionary<SYMBOL, string>() { { SYMBOL.ROUND, "\x25CF" }, { SYMBOL.TRIANGLE, "\x25B2" }, { SYMBOL.SQUARE, "\x25A0" } };

    public SYMBOL symbol;

    public SymbolData(SYMBOL symbol)
    {
        this.symbol = symbol;
    }

    public override string ToString()
    {
        return symbolsName[symbol];
    }
}
