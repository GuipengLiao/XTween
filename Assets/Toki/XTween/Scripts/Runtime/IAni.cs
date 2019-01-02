using UnityEngine;
using System;
using System.Collections.Generic;
using Toki.Tween;

public interface IAni
{
	float Duration
	{
		get;
	}
		
	float Position
	{
		get;
	}
		
	bool IsPlaying
	{
		get;
	}
		
	uint FrameSkip
	{
		get;
		set;
	}
		
	IExecutable OnPlay
	{
		get;
		set;
	}
		
	IExecutable OnStop
	{
		get;
		set;
	}
		
	IExecutable OnUpdate
	{
		get;
		set;
	}
		
	IExecutable OnComplete
	{
		get;
		set;
	}

    void StopOnDestroy();
	void Play();
	WaitForTweenPlay WaitForPlay();
	WaitForTweenPlay WaitForGotoAndPlay( float position );
	void Stop();	
	void TogglePause();
	void GotoAndPlay( float position );
	void GotoAndStop( float position );
	IAni Clone();
}
