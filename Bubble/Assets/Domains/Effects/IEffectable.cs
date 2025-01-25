using System;

[Serializable]
public class EffectArgs
{
	public string Type;
}

public interface IEffectable
{
	void ApplyEffect(GameModifierType args);
	void DisableEffect();
}


