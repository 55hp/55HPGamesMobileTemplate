namespace hp55games.Mobile.Core.Progression
{
    /// <summary>
    /// The handful of currency "lanes" most mobile games end up needing: Soft (earned via
    /// play), Hard (earned slowly or bought), Premium (bought only). Kept as a fixed enum
    /// rather than a string/ScriptableObject — unlike ItemData.category, the number of
    /// currency lanes is a design decision made once per project, not an open-ended list.
    /// </summary>
    public enum CurrencyType
    {
        Soft,
        Hard,
        Premium
    }
}
