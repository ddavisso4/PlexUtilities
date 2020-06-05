namespace Ddavisso4.PlexUtilities.Actions
{
    public interface IActionHandler<TAction>
            where TAction : ActionOptionsBase
    {
        void Handle(TAction options);
    }
}