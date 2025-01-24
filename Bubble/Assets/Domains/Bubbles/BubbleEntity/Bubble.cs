using Domains.Core;
using UnityEngine;

namespace Domains.Bubbles.BubbleEntity
{

    public interface IBubble
    {
    }

    public class Bubble : MonoBehaviour, IBubble, IConstructed
    {
        public void Construct(in Dependencies deps) { }
    }
}
