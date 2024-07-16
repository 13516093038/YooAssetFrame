using HotUpdate.Window;

namespace HotUpdate
{
    public class MainProcedure : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Test();
        }

        async void Test()
        {
            ModelWindow modelWindow = await UIModule.Ins.PopUpWindow<ModelWindow>();
        }
    }
}