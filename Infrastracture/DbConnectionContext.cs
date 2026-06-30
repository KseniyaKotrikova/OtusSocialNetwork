public static class DbConnectionContext
{
    private static readonly AsyncLocal<DbNodeRole> _currentRole = new();

    public static DbNodeRole CurrentRole
    {
        get => _currentRole.Value;
        set => _currentRole.Value = value;
    }
}