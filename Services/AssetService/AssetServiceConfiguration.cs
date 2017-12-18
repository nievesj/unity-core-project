using System.Collections;
using System.Collections.Generic;
using Core.Service;
using UnityEngine;

namespace Core.Assets
{
	public class AssetServiceConfiguration : ServiceConfiguration
	{
		[SerializeField]
		protected string assetBundlesURL;
		public string AssetBundlesURL { get { return assetBundlesURL; } set { assetBundlesURL = value; } }

		[SerializeField]
		protected bool useStreamingAssets = false;
		public bool UseStreamingAssets { get { return useStreamingAssets; } set { useStreamingAssets = value; } }

		[SerializeField]
		protected bool useCache = true;
		public bool UseCache { get { return useCache; } set { useCache = value; } }

		protected override IService GetServiceClass()
		{
			return new AssetService();
		}

		public override void ShowEditorUI()
		{

		}
	}
}