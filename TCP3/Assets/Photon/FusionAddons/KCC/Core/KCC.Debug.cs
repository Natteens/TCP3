namespace Fusion.Addons.KCC
{
	using UnityEngine;
	using System.Runtime.CompilerServices;

	// This file contains utilities for debugging.
	public partial class KCC
	{
		// PUBLIC METHODS

		/// <summary>
		/// Logs current KCC and KCCData state.
		/// </summary>
		[HideInCallstack]
		public void Dump()
		{
			_debug.Dump(this);
		}

		/// <summary>
		/// Enable logs for a given duration. This outputs same logs as <c>Dump()</c>.
		/// <param name="duration">Duration of logs. Use negative value for infinite logging.</param>
		/// </summary>
		public void EnableLogs(float duration = -1.0f)
		{
			_debug.EnableLogs(this, duration);
		}

		/// <summary>
		/// Logs info message into console with frame/tick metadata.
		/// </summary>
		/// <param name="messages">Custom message objects.</param>
		[HideInCallstack]
		public void Log(params object[] messages)
		{
			KCCUtility.Log(this, default, EKCCLogType.Info, messages);
		}

		/// <summary>
		/// Logs warning message into console with frame/tick metadata.
		/// </summary>
		/// <param name="messages">Custom message objects.</param>
		[HideInCallstack]
		public void LogWarning(params object[] messages)
		{
			KCCUtility.Log(this, default, EKCCLogType.Warning, messages);
		}

		/// <summary>
		/// Logs error message into console with frame/tick metadata.
		/// </summary>
		/// <param name="messages">Custom message objects.</param>
		[HideInCallstack]
		public void LogError(params object[] messages)
		{
			KCCUtility.Log(this, default, EKCCLogType.Error, messages);
		}

		/// <summary>
		/// Draws debug line in editor scene view.
		/// The color is yellow (forward tick), red (resimulation tick) or green (render).
		/// </summary>
		/// <param name="duration">Duration of the drawing.</param>
		public void DrawLine(float duration = 0.0f)
		{
			Color   color;
			Vector3 position;

			NetworkRunner runner = Runner;
			if (runner != null)
			{
				bool isInFixedUpdate = runner.Stage != default;
				bool isInForwardTick = runner.IsForward == true;

				if (isInFixedUpdate == true)
				{
					position = _fixedData.TargetPosition;

					if (isInForwardTick == true)
					{
						color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
					}
					else
					{
						color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
					}
				}
				else
				{
					position = _renderData.TargetPosition;
					color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
				}
			}
			else
			{
				position = _transform.position;
				color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
			}

			DrawLine(position, position + Vector3.up, color, duration);
		}

		/// <summary>
		/// Draws debug line in editor scene view.
		/// </summary>
		/// <param name="color">Color of the line.</param>
		/// <param name="duration">Duration of the drawing.</param>
		public void DrawLine(Color color, float duration = 0.0f)
		{
			Vector3 position = Data.TargetPosition;
			DrawLine(position, position + Vector3.up, color, duration);
		}

		// PRIVATE METHODS

		[HideInCallstack]
		[System.Diagnostics.Conditional(TRACING_SCRIPT_DEFINE)]
		private void Trace(params object[] messages)
		{
			KCCUtility.Trace<KCC>(this, messages);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool CheckSpawned()
		{
			if (_isSpawned == false)
			{
				LogError($"{nameof(KCC)}.{nameof(Spawned)}() has not been called yet! Use {nameof(KCC)}.{nameof(InvokeOnSpawn)}() to register a callback.", this);
				return false;
			}

			return true;
		}

		private void DrawLine(Vector3 startPosition, Vector3 endPosition, Color color, float duration)
		{
			UnityEngine.Debug.DrawLine(startPosition, endPosition, color, duration);
		}
	}
}
