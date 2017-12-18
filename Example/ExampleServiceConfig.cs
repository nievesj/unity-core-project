using System;
using System.Collections;
using Core.Service;
using UnityEngine;

namespace Core
{
	public class ExampleServiceConfig : ServiceConfiguration
	{
		public bool IsCool;
		public int HowCool;
		public string SomeString;

		protected override IService GetServiceClass()
		{
			return new ExampleService();
		}

		public override void ShowEditorUI()
		{
			IsCool = GUILayout.Toggle(IsCool, "Is Cool?");
			GUILayout.Label("How Cool?");
			HowCool = Convert.ToInt32(GUILayout.TextField(HowCool.ToString()));
			GUILayout.Label("Something");
			if (SomeString == null) SomeString = "";
			SomeString = GUILayout.TextField(SomeString);
		}
	}
}