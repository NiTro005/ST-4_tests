using Stateless;

namespace BugPro
{
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
        private StateMachine<BugState, BugTrigger> state_machine;

        public Bug()
        {
            state_machine = new StateMachine<BugState, BugTrigger>(BugState.New);
            ConfigureStates();
        }

        public BugState State => state_machine.State;

        private void ConfigureStates()
        {
            state_machine.Configure(BugState.New)
                .Permit(BugTrigger.AssignToTriage, BugState.Triage);

            state_machine.Configure(BugState.Triage)
                .PermitReentry(BugTrigger.NoTime)
                .PermitReentry(BugTrigger.SeparateSolution)
                .PermitReentry(BugTrigger.OtherProduct)
                .PermitReentry(BugTrigger.NeedMoreInfo)
                .Permit(BugTrigger.StartFix, BugState.Fixing)
                .Permit(BugTrigger.NotADefect, BugState.OkCheck)
                .Permit(BugTrigger.WontFix, BugState.OkCheck)
                .Permit(BugTrigger.Duplicate, BugState.OkCheck)
                .Permit(BugTrigger.CannotReproduce, BugState.OkCheck);

            state_machine.Configure(BugState.Fixing)
                .Permit(BugTrigger.ProblemNo, BugState.Closing)
                .Permit(BugTrigger.ProblemYes, BugState.ProblemSolving);

            state_machine.Configure(BugState.ProblemSolving)
                .Permit(BugTrigger.OkNo, BugState.OkCheck)
                .Permit(BugTrigger.OkYes, BugState.Closing);

            state_machine.Configure(BugState.OkCheck)
                .Permit(BugTrigger.OkNo, BugState.Triage)
                .Permit(BugTrigger.OkYes, BugState.Closing);

            state_machine.Configure(BugState.Return)
                .Permit(BugTrigger.ReturnNo, BugState.Triage)
                .Permit(BugTrigger.ReturnYes, BugState.Closing);

            state_machine.Configure(BugState.NotADefect)
                .Permit(BugTrigger.Reopen, BugState.Reopened);

            state_machine.Configure(BugState.WontFix)
                .Permit(BugTrigger.Reopen, BugState.Reopened);

            state_machine.Configure(BugState.Duplicate)
                .Permit(BugTrigger.Reopen, BugState.Reopened);

            state_machine.Configure(BugState.CannotReproduce)
                .Permit(BugTrigger.Reopen, BugState.Reopened);

            state_machine.Configure(BugState.Closing)
                .Permit(BugTrigger.Reopen, BugState.Reopened);

            state_machine.Configure(BugState.Reopened)
                .Permit(BugTrigger.AssignToTriage, BugState.Triage);
        }

        public void Fire(BugTrigger trigger)
        {
            if (state_machine.CanFire(trigger))
            {
                state_machine.Fire(trigger);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Невозможно выполнить {trigger} в состоянии {state_machine.State}");
            }
        }
    }

    class Program
    {
        static void Main()
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
}
