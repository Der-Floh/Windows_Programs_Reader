﻿using Bogus;
using Windows_Programs_Reader.Data;

namespace Windows_Programs_Reader.Test.Data.Faker;

public sealed class WindowsLanguageDataFaker : Faker<WindowsLanguageData>
{
    public WindowsLanguageDataFaker()
    {
        RuleFor(x => x.Name, f => f.Address.Country());
        RuleFor(x => x.Code, f => f.Random.Word().Substring(0, 2).ToLower());
        RuleFor(x => x.LCIDCode, f => f.Random.Int(0, 65535).ToString("X"));
        RuleFor(x => x.WindowsCodeDecimal, f => (uint)f.Random.Int(0, 65535));
        RuleFor(x => x.WindowsCodeHex, f => f.Random.Int(0, 65535).ToString("X"));
        RuleFor(x => x.CodePage, f => (uint)f.Random.Int(0, 65535));
    }
}
