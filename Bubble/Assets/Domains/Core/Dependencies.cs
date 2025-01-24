using Domains.Bubbles.Factories;

namespace Domains.Core
{
	public interface IConstructed
	{
		void Construct(IDependencies deps);
	}

	public interface IDependencies
	{
		// add get only dependencies here
		IBubbleFactory BubbleFactory { get; }
	}

	public class Dependencies : IDependencies
	{
		public IBubbleFactory BubbleFactory { get; set; }
	}
}
