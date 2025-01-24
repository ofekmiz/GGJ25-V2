using System;
using System.Collections.Generic;
using Domains.Bubbles.Factories;

namespace Domains.Core
{
	public interface IConstructed
	{
		void Construct(in Dependencies d);
	}

	public struct Dependencies
	{
		public IBubbleFactory BubbleFactory;
		public GameOverManager gameOverManager;
		public List<PlatformerItem> PlatformerItems;
	}
}
