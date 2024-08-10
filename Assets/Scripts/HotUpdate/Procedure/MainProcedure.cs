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

            if (Input.GetKeyDown(KeyCode.B))
            {
                Application.Quit();
            }
        }

        async void Test()
        {
            // ModelWindow modelWindow = await UIModule.Ins.PopUpWindow<ModelWindow>();
            // TestWindow testWindow = await UIModule.Ins.PopUpWindow<TestWindow>();
            UIModule.Ins.PreLoadWindow<ModelWindow333>();
            UIModule.Ins.PushAndPopStackWindow<ModelWindow>();
            UIModule.Ins.PushAndPopStackWindow<ModelWindow222>();
        }
        
        void Test1()
        {
            UIModule.Ins.PushAndPopStackWindow<ModelWindow333>();
        }
        
        async void Test2()
        {
            //ModelWindow modelWindow = await UIModule.Ins.PopUpWindow<ModelWindow>();
        }
    }
}