using System;
using saxion_provided;
using UnityEngine;

	public class ChosenImagesCommunicator : Singleton<ChosenImagesCommunicator>
	{
		[SerializeField] private Client networkingClient;

		public override void Awake()
		{
			base.Awake();

			if (networkingClient == null) { throw new NullReferenceException("networking client is not set in the editor!"); }
			
			gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			if (!Player.Instance.dead)
			{
				return;
			}

			Packet packet = new ();
			packet.Write(new RequestFileNames());
			networkingClient.SendData(packet);
		}

		public void RewriteDataBaseCache(GetFileNames serverObject)
		{
			Debug.Log(serverObject.ToString());
		}
	}
