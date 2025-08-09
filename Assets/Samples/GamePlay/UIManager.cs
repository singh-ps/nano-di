using UnityEngine;

namespace GamePlay
{
	public class UIManager
	{
		public UIManager() {}

		public void ShowUI(string uiName)
		{
			Debug.Log($"Showing UI: {uiName}");
		}

		public void HideUI(string uiName)
		{
			Debug.Log($"Hiding UI: {uiName}");
		} 
	}
}
