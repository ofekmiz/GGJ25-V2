using Domains.Bubbles.Factories;
using UnityEngine;

namespace Domains.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] BubbleFactory _bubbleFactory;

        [SerializeField] private PlayerController _playerPrefab;
        [SerializeField] private Transform _playerParent;

        private PlayerController _player;

        public void CreatePlayer()
        {
            _player = Instantiate(_playerPrefab, _playerParent);
            _player.PlayerDead += OnPlayerDead;
        }

        private void OnPlayerDead()
        {
            _player.PlayerDead -= OnPlayerDead;
            Debug.Log($"Player Dead");
        }
    }
}
