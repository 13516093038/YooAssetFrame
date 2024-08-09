/*---------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:Jet
 *Date:8/9/2024 4:01:57 PM
 *Description:表现层，该层只负责界面的交互，表现相关的更新，不允许编写任何业务逻辑代码
 *注意：以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原油代码上进行新增，可放心使用
---------------------------*/

using UnityEngine.UI;
using UnityEngine;

namespace HotUpdate
{

	public class ModelWindow222 : WindowBase
	{
		private ModelWindow222DataComponent dataComp;

		#region 生命周期函数
		public override void OnAwake()
		{
			base.OnAwake();
			dataComp = gameObject.GetComponent<ModelWindow222DataComponent>();
			dataComp.InitComponent(this);
		}

		public override void OnShow()
		{
			base.OnShow();
		}

		public override void OnHide()
		{
			base.OnHide();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
		}
		#endregion
		#region APIFunction

		#endregion
		#region UI组件事件
		public void OnCloseButtonClick()
		{
			HideWindow();
		}
		#endregion
	}
}
