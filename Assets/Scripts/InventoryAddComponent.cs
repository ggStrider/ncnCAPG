using ncn.PlayerSettings;
using UnityEngine;

namespace ncn.Mechanics
{
    public class InventoryAddComponent : MonoBehaviour
    {
        [SerializeField] private string _id;
        [SerializeField] private int _count;
        [SerializeField] private PlayerSystem _player;

        public void Add()
        {
            _player.AddInInventory(_id, _count);
        }
    }
}