using System.Collections.Generic;
using Domains.Bubbles.BubbleSpawner;
using Domains.Bubbles.Factories;

namespace Domains.Core
{
	public interface IConstructed
	{
		void Construct(in Dependencies d);
	}

	public struct Dependencies
	{
		public GameManager GameManager;
		public IBubbleFactory BubbleFactory;
		public BubblesManager BubblesManager;
		public GameOverManager GameOverManager;
		public List<EffectItem> PlatformerItems;
	}
}
