using Stateless;

namespace BugPro;

public enum BugState
{
    New,
    Triage,
    Fixing,
    ProblemSolving,
    Closing,
    NotADefect,
    WontFix,
    Duplicate,
    CannotReproduce,
    OkCheck,
    Return,
    Reopened
}

public enum BugTrigger
{
    AssignToTriage,
    NoTime,
    SeparateSolution,
    OtherProduct,
    NeedMoreInfo,
    StartFix,
    ProblemNo,
    ProblemYes,
    NotADefect,
    WontFix,
    Duplicate,
    CannotReproduce,
    OkNo,
    OkYes,
    ReturnNo,
    ReturnYes,
    Reopen
}

public class Bug
{
    private readonly StateMachine<BugState, BugTrigger> _workflow;

    private static readonly BugTrigger[] TriageReentryTriggers =
    [
        BugTrigger.NoTime,
        BugTrigger.SeparateSolution,
        BugTrigger.OtherProduct,
        BugTrigger.NeedMoreInfo
    ];

    private static readonly BugTrigger[] ResolutionCheckTriggers =
    [
        BugTrigger.NotADefect,
        BugTrigger.WontFix,
        BugTrigger.Duplicate,
        BugTrigger.CannotReproduce
    ];

    private static readonly BugState[] ReopenableStates =
    [
        BugState.NotADefect,
        BugState.WontFix,
        BugState.Duplicate,
        BugState.CannotReproduce,
        BugState.Closing
    ];

    public Bug()
    {
        _workflow = new StateMachine<BugState, BugTrigger>(BugState.New);
        BuildWorkflow();
    }

    public BugState State => _workflow.State;

    public bool IsTriggerAllowed(BugTrigger trigger) => _workflow.CanFire(trigger);

    private void BuildWorkflow()
    {
        _workflow.Configure(BugState.New)
            .Permit(BugTrigger.AssignToTriage, BugState.Triage);

        ConfigureTriageState();

        _workflow.Configure(BugState.Fixing)
            .Permit(BugTrigger.ProblemNo, BugState.Closing)
            .Permit(BugTrigger.ProblemYes, BugState.ProblemSolving);

        _workflow.Configure(BugState.ProblemSolving)
            .Permit(BugTrigger.OkNo, BugState.OkCheck)
            .Permit(BugTrigger.OkYes, BugState.Closing);

        _workflow.Configure(BugState.OkCheck)
            .Permit(BugTrigger.OkNo, BugState.Triage)
            .Permit(BugTrigger.OkYes, BugState.Closing);

        _workflow.Configure(BugState.Return)
            .Permit(BugTrigger.ReturnNo, BugState.Triage)
            .Permit(BugTrigger.ReturnYes, BugState.Closing);

        foreach (var state in ReopenableStates)
        {
            _workflow.Configure(state)
                .Permit(BugTrigger.Reopen, BugState.Reopened);
        }

        _workflow.Configure(BugState.Reopened)
            .Permit(BugTrigger.AssignToTriage, BugState.Triage);
    }

    private void ConfigureTriageState()
    {
        var triage = _workflow.Configure(BugState.Triage)
            .Permit(BugTrigger.StartFix, BugState.Fixing);

        foreach (var trigger in TriageReentryTriggers)
        {
            triage.PermitReentry(trigger);
        }

        foreach (var trigger in ResolutionCheckTriggers)
        {
            triage.Permit(trigger, BugState.OkCheck);
        }
    }

    public void Fire(BugTrigger trigger)
    {
        if (!IsTriggerAllowed(trigger))
        {
            throw new InvalidOperationException(
                $"Невозможно выполнить {trigger} в состоянии {State}");
        }

        _workflow.Fire(trigger);
    }
}

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("WorkFlow бага");
        Console.WriteLine();

        var bug = new Bug();
        Console.WriteLine("Начальное состояние: " + bug.State);
        Console.WriteLine();

        try
        {
            bug.Fire(BugTrigger.AssignToTriage);
            Console.WriteLine("AssignToTriage -> " + bug.State);

            bug.Fire(BugTrigger.StartFix);
            Console.WriteLine("StartFix -> " + bug.State);

            bug.Fire(BugTrigger.ProblemNo);
            Console.WriteLine("ProblemNo -> " + bug.State);

            Console.WriteLine();
            Console.WriteLine("Конечное состояние: " + bug.State);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
        }
    }
}
