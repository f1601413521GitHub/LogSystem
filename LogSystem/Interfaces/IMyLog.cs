namespace LogSystem.Interfaces
{
    interface IMyLog
    {
        string ContextId { get; }
        string Controller { get; }
        string Action { get; }
    }
}