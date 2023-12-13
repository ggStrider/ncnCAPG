using UnityEngine;
using ncn.Model.Defs;
using ncn.Model;
using UnityEngine.Events;

namespace ncn.Mechanics
{
    public class CheckRequirementItem : MonoBehaviour
    {
        [SerializeField] private InventoryItemData[] _required;
        [SerializeField] private bool _removeAfterUse;

        [Space]
        [SerializeField] private UnityEvent _onSuccess;
        [SerializeField] private UnityEvent _onFail;

        public void CheckItems()
        {
            var session = FindObjectOfType<GameSession>();
            var areAllRequirementsMet = true;

            foreach(var item in _required)
            {
                var numItems = session.Data.Inventory.Count(item.Id);   
                if(numItems < item.Value)
                    areAllRequirementsMet = false;
            }

            if(areAllRequirementsMet)
            {
                if(_removeAfterUse)
                {
                    foreach(var item in _required)
                        session.Data.Inventory.Remove(item.Id, item.Value);
                }
                _onSuccess?.Invoke();
            }
            else
            {
                _onFail.Invoke();
            }
        }
    }
}
