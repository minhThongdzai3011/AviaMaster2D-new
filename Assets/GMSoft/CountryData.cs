using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GmSoft/Country-Data")]
public class CountryData : ScriptableObject
{
    public List<Country> countries = new List<Country>();

    public static CountryData _Instance = null;

    public static CountryData Instance 
    {
        get 
        {
            if (_Instance == null) 
            {
                _Instance = Resources.Load<CountryData>("Country-Data");
            }
            return _Instance;
        }
    }

    public Country GetCountryByName(string countryName) 
    {
        if (string.IsNullOrEmpty(countryName)) return null;
        if (countries == null || countries.Count <= 0) return null;
        return countries.FirstOrDefault(c => c.countryName == countryName);
    }

    public Country GetCountryByISOCode(string ISOCode) 
    {
        if (string.IsNullOrEmpty(ISOCode)) return null;
        if (countries == null || countries.Count <= 0) return null;
        return countries.FirstOrDefault(c => c.countryISOCode == ISOCode);
    }

    public Sprite GetFlagByName(string countryName) 
    {
        if (string.IsNullOrEmpty(countryName)) return null;
        if (countries == null || countries.Count <= 0) return null;
        return GetCountryByName(countryName).flag;
    }

    public Sprite GetFlagByISOCode(string ISOCode)
    {
        if (string.IsNullOrEmpty(ISOCode)) return null;
        if (countries == null || countries.Count <= 0) return null;
        return GetCountryByISOCode(ISOCode).flag;
    }
}

[Serializable]
public class Country 
{
    public string countryName;
    public string countryISOCode;
    public Sprite flag;
}
