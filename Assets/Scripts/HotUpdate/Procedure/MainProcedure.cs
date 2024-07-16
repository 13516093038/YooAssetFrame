using HotUpdate.Window;
using UnityEngine;

namespace HotUpdate
{
    public class MainProcedure : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Test();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Input.GetKeyDown(KeyCode.A))
            {
                Test1();
            }
        }

        async void Test()
        {
            ModelWindow modelWindow = await UIModule.Ins.PopUpWindow<ModelWindow>();
            TestWindow testWindow = await UIModule.Ins.PopUpWindow<TestWindow>();
        }
        
        void Test1()
        {
            UIModule.Ins.HideWindow<TestWindow>();
        }
        
        async void Test2()
        {
            ModelWindow modelWindow = await UIModule.Ins.PopUpWindow<ModelWindow>();
        }
    }
}