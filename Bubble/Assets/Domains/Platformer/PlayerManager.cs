using Domains.Core;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameManager _gm;

    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private Transform _playerParent;

    // public PlayerController CreatePlayer()
    // {
    //     var player = _gm.Spawn(_playerPrefab, _playerParent);
    //     return player;
    // }
}
