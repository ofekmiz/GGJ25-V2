using Domains.Bubbles.BubbleEntity;
using UnityEngine;

namespace Domains.Bubbles.Factories
{
    public interface IBubbleFactory
    {
        
    }
    
    public class BubbleFactory : ScriptableObject
    {
        [SerializeField] private Bubble _bubblePrefab;

        
    }
}
