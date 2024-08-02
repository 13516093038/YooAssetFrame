/*---------------------------
 *Title:UI自动化组件查找代码工具
 *Author:Jet
 *Date:8/2/2024 4:15:19 PM
 *Description:变量需以[Text]括号加组件类型的格式进行声明，然后右键窗口物体，一键生成UI组件查找脚本即可
 *注意：以下文件是自动生成的，任何手动修改都会被下次生成覆盖，若手动修改后，尽量避免自动生成
---------------------------*/

using UnityEngine.UI;
using UnityEngine;

namespace HotUpdate
{
	public class ModelWindowUIComponent
	{
		public Button CloseButton;

		public Button SureButton;

		public Button Test123Button;

		public Button Test111Button;

		public Button Test1111Button;

		public GameObject i666GameObject;

		public Button T666Button;

		public Button TestButton;

		public void InitComponent(WindowBase target)
		{
			//组件查找
			CloseButton = target.transform.Find("UIContent/[Button]Close").GetComponent<Button>();
			SureButton = target.transform.Find("UIContent/[Button]Sure").GetComponent<Button>();
			Test123Button = target.transform.Find("UIContent/[Button]Test123").GetComponent<Button>();
			Test111Button = target.transform.Find("UIContent/[Button]Test111").GetComponent<Button>();
			Test1111Button = target.transform.Find("UIContent/[Button]Test1111").GetComponent<Button>();
			i666GameObject = target.transform.Find("UIContent/GameObject/[GameObject]i666").gameObject;
			T666Button = target.transform.Find("UIContent/GameObject/[Button]T666").GetComponent<Button>();
			TestButton = target.transform.Find("UIContent/GameObject/[Button]Test").GetComponent<Button>();
	
			//组件事件绑定
			ModelWindow mWindow = (ModelWindow)target;
			target.AddButtonClickListener(CloseButton,mWindow.OnCloseButtonClick);
			target.AddButtonClickListener(SureButton,mWindow.OnSureButtonClick);
			target.AddButtonClickListener(Test123Button,mWindow.OnTest123ButtonClick);
			target.AddButtonClickListener(Test111Button,mWindow.OnTest111ButtonClick);
			target.AddButtonClickListener(Test1111Button,mWindow.OnTest1111ButtonClick);
			target.AddButtonClickListener(T666Button,mWindow.OnT666ButtonClick);
			target.AddButtonClickListener(TestButton,mWindow.OnTestButtonClick);
		}
	}
}
