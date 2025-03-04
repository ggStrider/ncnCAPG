using UnityEngine;

namespace ncn.Model.Defs
{
    [CreateAssetMenu(menuName = "Defs/InventoryItems", fileName = "InventoryItems")]
    public class InventoryItemsDef : ScriptableObject
    {
        [SerializeField] private ItemDef[] _items;

        public ItemDef Get(string id)
        {
            foreach(var itemDef in _items)
            {
                if(itemDef.Id == id)
                    return itemDef;
            }
            return default;
        }
    }

    [System.Serializable]
    public struct ItemDef
    {
        [SerializeField] private string _id;
        public string Id => _id;

        public bool IsVoid => string.IsNullOrEmpty(_id);
    }
}
