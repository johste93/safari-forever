using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryUtil
{
    public static bool GDPRLawApplies(Country country)
    {
        return IsEEA(country);
    }

    public static bool IsEU(Country country)
    {
        switch(country)
        {
            case Country.Austria:
            case Country.Belgium:
            case Country.Bulgaria:
            case Country.Croatia:
            case Country.Cyprus:
            case Country.Czechia:
            case Country.Denmark:
            case Country.Estonia:
            case Country.Finland:
            case Country.France:
            case Country.Germany:
            case Country.Greece:
            case Country.Hungary:
            case Country.Ireland:
            case Country.Italy:
            case Country.Latvia:
            case Country.Lithuania:
            case Country.Luxembourg:
            case Country.Malta:
            case Country.Netherlands:
            case Country.Poland:
            case Country.Portugal:
            case Country.Romania:
            case Country.Slovakia:
            case Country.Slovenia:
            case Country.Spain:
            case Country.Sweden:
                return true;
        }

        return false;
    }

    public static bool IsEEA(Country country)
    {
        switch(country)
        {
            case Country.Iceland:
            case Country.Liechtenstein:
            case Country.Norway:
                return true;
        }

        return IsEU(country);
    }
}
