using System.Text.RegularExpressions;
using UnityEngine;
using NaughtyAttributes;

public class TextFieldValidator : MonoBehaviour
{
    public enum RuleType
    {
        Required,
        Email,
        Length,
        NoSpaces,
        Alphanumeric,
        Numeric,
        HasSymbol,
        Regex,
        EqualsLiteral,
        NotEqualsLiteral
    }

    public enum MatchMode { Exact, IgnoreCase }

    [SerializeField] private RuleType rule;

    [Space(10)]
    [ShowIf("rule", RuleType.Length)]
    [SerializeField] private Vector2 intParam;

    [ShowIf("rule", RuleType.Regex)]
    [SerializeField] private string stringParam;

    [ShowIf("rule", RuleType.EqualsLiteral)]
    [SerializeField] private string literalValue;

    [ShowIf("rule", RuleType.EqualsLiteral)]
    [SerializeField] private MatchMode matchMode;

    [ShowIf("rule", RuleType.NotEqualsLiteral)]
    [SerializeField] private string forbiddenValue;

    public bool Validate(string text){
        switch(rule){
            case RuleType.Required:         return !string.IsNullOrEmpty(text);
            case RuleType.Email:            return text.Contains("@") && text.Contains(".");
            case RuleType.Length:           return text.Length >= intParam.x && text.Length <= intParam.y;
            case RuleType.NoSpaces:         return !text.Contains(" ");
            case RuleType.Alphanumeric:     return IsAlphanumeric(text);
            case RuleType.Numeric:          return int.TryParse(text, out _);
            case RuleType.HasSymbol:        return HasSymbol(text);
            case RuleType.Regex:            return Regex.IsMatch(text, stringParam);
            case RuleType.EqualsLiteral:    return MatchesLiteral(text, literalValue, matchMode);
            case RuleType.NotEqualsLiteral: return !MatchesLiteral(text, forbiddenValue, MatchMode.IgnoreCase);
        }
        return true;
    }

    static bool MatchesLiteral(string text, string expected, MatchMode mode){
        if(expected == null) return true;
        return mode == MatchMode.IgnoreCase
            ? string.Equals(text, expected, System.StringComparison.OrdinalIgnoreCase)
            : text == expected;
    }

    static bool IsAlphanumeric(string s){
        foreach(var c in s) if(!char.IsLetterOrDigit(c)) return false;
        return true;
    }

    static bool HasSymbol(string s){
        foreach(var c in s)
            if(!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)) return true;
        return false;
    }

    public string GetRuleName(){
        switch(rule){
            case RuleType.Required:         return "Required";
            case RuleType.Email:            return "Email";
            case RuleType.Length:           return $"Length({intParam.x}-{intParam.y})";
            case RuleType.NoSpaces:         return "NoSpaces";
            case RuleType.Alphanumeric:     return "Alphanumeric";
            case RuleType.Numeric:          return "Numeric";
            case RuleType.HasSymbol:        return "HasSymbol";
            case RuleType.Regex:            return $"Regex({stringParam})";
            case RuleType.EqualsLiteral:    return $"EqualsLiteral({literalValue})";
            case RuleType.NotEqualsLiteral: return $"NotEqualsLiteral({forbiddenValue})";
        }
        return rule.ToString();
    }
}