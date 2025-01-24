using System;

[Serializable]
public class EffectArgs
{
	public string Type;
}

public interface IEffectable
{
	void ApplyEffect(EffectArgs args);
	void DisableEffect();
}


