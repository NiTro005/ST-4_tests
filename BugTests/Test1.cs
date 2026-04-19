using BugPro;

namespace BugTests
{
    [TestClass]
    public sealed class Test1
    {
        private Bug bug;

        [TestInitialize]
        public void Setup()
        {
            bug = new Bug();
        }

        [TestMethod]
        public void Test01_FromNewState_TransitionToTriage_ShouldSucceed()
        {
            Assert.AreEqual(BugState.New, bug.State);
            bug.Fire(BugTrigger.AssignToTriage);
            Assert.AreEqual(BugState.Triage, bug.State);
        }

        [TestMethod]
        public void Test02_FromTriageState_StartFix_ShouldMoveToFixing()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.StartFix);
            Assert.AreEqual(BugState.Fixing, bug.State);
        }

        [TestMethod]
        public void Test03_FromFixingState_ProblemNo_ShouldMoveToClosing()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.StartFix);
            bug.Fire(BugTrigger.ProblemNo);
            Assert.AreEqual(BugState.Closing, bug.State);
        }

        [TestMethod]
        public void Test04_FromFixingState_ProblemYes_ShouldMoveToProblemSolving()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.StartFix);
            bug.Fire(BugTrigger.ProblemYes);
            Assert.AreEqual(BugState.ProblemSolving, bug.State);
        }

        [TestMethod]
        public void Test05_FromTriageState_MarkNotADefect_ShouldMoveToOkCheck()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.NotADefect);
            Assert.AreEqual(BugState.OkCheck, bug.State);
        }

        [TestMethod]
        public void Test06_FromTriageState_MarkWontFix_ShouldMoveToOkCheck()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.WontFix);
            Assert.AreEqual(BugState.OkCheck, bug.State);
        }

        [TestMethod]
        public void Test07_FromTriageState_MarkDuplicate_ShouldMoveToOkCheck()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.Duplicate);
            Assert.AreEqual(BugState.OkCheck, bug.State);
        }

        [TestMethod]
        public void Test08_FromTriageState_MarkCannotReproduce_ShouldMoveToOkCheck()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.CannotReproduce);
            Assert.AreEqual(BugState.OkCheck, bug.State);
        }

        [TestMethod]
        public void Test09_InTriageState_TriggerNoTime_StateRemainsUnchanged()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            BugState stateBefore = bug.State;
            bug.Fire(BugTrigger.NoTime);
            Assert.AreEqual(stateBefore, bug.State);
        }

        [TestMethod]
        public void Test10_InTriageState_TriggerNeedMoreInfo_StateRemainsUnchanged()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            BugState stateBefore = bug.State;
            bug.Fire(BugTrigger.NeedMoreInfo);
            Assert.AreEqual(stateBefore, bug.State);
        }

        [TestMethod]
        public void Test11_InTriageState_TriggerSeparateSolution_StateRemainsUnchanged()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            BugState stateBefore = bug.State;
            bug.Fire(BugTrigger.SeparateSolution);
            Assert.AreEqual(stateBefore, bug.State);
        }

        [TestMethod]
        public void Test12_InTriageState_TriggerOtherProduct_StateRemainsUnchanged()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            BugState stateBefore = bug.State;
            bug.Fire(BugTrigger.OtherProduct);
            Assert.AreEqual(stateBefore, bug.State);
        }

        [TestMethod]
        public void Test13_FromProblemSolvingState_OkNo_ShouldMoveToOkCheck()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.StartFix);
            bug.Fire(BugTrigger.ProblemYes);
            bug.Fire(BugTrigger.OkNo);
            Assert.AreEqual(BugState.OkCheck, bug.State);
        }

        [TestMethod]
        public void Test14_InOkCheckState_OkNo_ShouldMoveToTriage()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.StartFix);
            bug.Fire(BugTrigger.ProblemYes);
            bug.Fire(BugTrigger.OkNo);
            bug.Fire(BugTrigger.OkNo);
            Assert.AreEqual(BugState.Triage, bug.State);
        }

        [TestMethod]
        public void Test15_InOkCheckState_OkYes_ShouldMoveToClosing()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.StartFix);
            bug.Fire(BugTrigger.ProblemYes);
            bug.Fire(BugTrigger.OkYes);
            Assert.AreEqual(BugState.Closing, bug.State);
        }

        [TestMethod]
        public void Test16_FromProblemSolvingState_OkYes_ShouldMoveToClosing()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.StartFix);
            bug.Fire(BugTrigger.ProblemYes);
            bug.Fire(BugTrigger.OkYes);
            Assert.AreEqual(BugState.Closing, bug.State);
        }

        [TestMethod]
        public void Test17_FromNotADefectState_Reopen_ShouldMoveToReopened()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.NotADefect);
            bug.Fire(BugTrigger.OkYes);
            bug.Fire(BugTrigger.Reopen);
            Assert.AreEqual(BugState.Reopened, bug.State);
        }

        [TestMethod]
        public void Test18_FromReopenedState_AssignToTriage_ShouldMoveToTriage()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.NotADefect);
            bug.Fire(BugTrigger.OkYes);
            bug.Fire(BugTrigger.Reopen);
            bug.Fire(BugTrigger.AssignToTriage);
            Assert.AreEqual(BugState.Triage, bug.State);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test19_NewState_StartFixWithoutTriage_ShouldThrowException()
        {
            bug.Fire(BugTrigger.StartFix);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test20_ClosedBug_ReopenTwice_ShouldThrowException()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.NotADefect);
            bug.Fire(BugTrigger.OkYes);
            bug.Fire(BugTrigger.Reopen);
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.Reopen);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test21_StartFix_InFixing_ShouldThrowException()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.StartFix);
            bug.Fire(BugTrigger.StartFix);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test22_FromTriageState_DirectReopen_ShouldThrowException()
        {
            bug.Fire(BugTrigger.AssignToTriage);
            bug.Fire(BugTrigger.Reopen);
        }
    }
}