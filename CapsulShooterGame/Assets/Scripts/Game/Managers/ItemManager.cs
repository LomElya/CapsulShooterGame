using System.Collections.Generic;
using UnityEngine;
using Items;
using UnityEngine.Events;

public class ItemManager : MonoBehaviour
{
    public enum ItemSwitchState
    {
        Up,
        Down,
        PutDownPrevious,
        PutUpNew,
    }

    [SerializeField] private List<Item> _startingItem = new List<Item>();
    [SerializeField] private Transform _itemParentSocket;
    //Задержка перед переключением предмета
    [SerializeField] private float _itemSwitchDelay = 1f;
    [SerializeField] private LayerMask _itemLayer;
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private Camera _itemCamera;

    private Item[] _itemSlots = new Item[6];
    private ItemSwitchState _itemSwitchState;
    private float _timeStartedItemSwitch;
    private int _itemSwitchNewItemIndex;

    public Camera ItemCamera => _itemCamera;

    public UnityAction<Item> OnSwitchedToItem;
    public UnityAction<Item, int> OnAddedItem;
    public UnityAction<Item, int> OnRemoveItem;

    public int ActiveItemIndex { get; private set; }

    private void Start()
    {
        ActiveItemIndex = -1;
        _itemSwitchState = ItemSwitchState.Down;

        OnSwitchedToItem += OnItemSwitched;

        foreach (var item in _startingItem)
        {
            if (item == null || HasItem(item))
                continue;

            AddItem(item);
        }

        SwitchItem(true);
    }

    private void Update()
    {
        /// Использование/атака
        Item activeItem = GetActiveItem();
        if (activeItem != null && _itemSwitchState == ItemSwitchState.Up)
        {
            bool hasUse = activeItem.HandleUseInputs(
                _inputHandler.GetUseInputDown(),
                _inputHandler.GetUseInputHeld(),
                _inputHandler.GetUseInputReleased());

            // Сопротивление отдачи
            if (hasUse)
            {
                //Debug.Log("Отдача");
            }
        }
        //Переключение оружия
        if (_itemSwitchState == ItemSwitchState.Up || _itemSwitchState == ItemSwitchState.Down)
        {
            int switchItemInput = _inputHandler.GetSwitchItemInput();

            if (switchItemInput != 0)
            {
                bool switchUp = switchItemInput > 0;
                SwitchItem(switchUp);
            }
            else
            {
                switchItemInput = _inputHandler.GetSelectItemInput();

                if (switchItemInput != 0)
                {
                    if (GetItemAtSlotIndex(switchItemInput - 1) != null)
                        SwitchToItemIndex(switchItemInput - 1);
                }
            }

        }
    }

    private void LateUpdate()
    {
        UpdateItemSwitching();
    }

    public Item HasItem(Item itemPrefab)
    {
        // Проверяет есль ли предмет из Prefab
        for (var index = 0; index < _itemSlots.Length; index++)
        {
            var w = _itemSlots[index];
            if (w != null && w.SourcePrefab == itemPrefab.gameObject)
            {
                return w;
            }
        }

        return null;
    }

    public bool AddItem(Item itemPrefab)
    {
        //Если такой предмет есть - не добавлять его
        if (HasItem(itemPrefab))
            return false;

        //Найти первое свободный слот в инвенторе
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            //Добавить предмет в слот, если оно свободно
            if (_itemSlots[i] == null)
            {

                _itemSlots[i] = InstantiateItem(itemPrefab);
                _itemSlots[i].ShowItem(false);

                OnAddedItem?.Invoke(_itemSlots[i], i);

                return true;
            }
        }

        if (GetActiveItem() == null)
        {
            SwitchItem(true);
        }
        return false;
    }

    private Item InstantiateItem(Item item)
    {
        Item itemInstance = Instantiate(item, _itemParentSocket);
        itemInstance.Add(gameObject, item, _itemParentSocket);

        int layerIndex =
            Mathf.RoundToInt(Mathf.Log(_itemLayer.value, 2));

        foreach (Transform t in gameObject.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = layerIndex;
        }

        return itemInstance;
    }

    private bool RemoveItem(Item item)
    {
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            if (_itemSlots[i] == item)
            {
                _itemSlots[i] = null;

                OnRemoveItem?.Invoke(item, i);

                Destroy(item.gameObject);

                if (i == ActiveItemIndex)
                {
                    SwitchItem(true);
                }

                return true;
            }
        }

        return false;
    }

    public void SwitchItem(bool ascendingOrder)
    {
        int newItemIndex = -1;
        int closestSlotDistance = _itemSlots.Length;
        for (int i = 0; i < 6; i++)
        {
            if (i != ActiveItemIndex && GetItemAtSlotIndex(i) != null)
            {
                int distanceToActiveIndex = GetDistanceBetweenItemSlots(ActiveItemIndex, i, ascendingOrder);

                if (distanceToActiveIndex < closestSlotDistance)
                {
                    closestSlotDistance = distanceToActiveIndex;
                    newItemIndex = i;
                }
            }
        }
        // Переключение на новый индекс предмета
        SwitchToItemIndex(newItemIndex);
    }

    public void SwitchToItemIndex(int newItemIndex, bool force = false)
    {
        if (force || (newItemIndex != ActiveItemIndex && newItemIndex >= 0))
        {
            // "Анимация" переключения предмета
            _itemSwitchNewItemIndex = newItemIndex;
            _timeStartedItemSwitch = Time.time;
            if (GetActiveItem() == null)
            {
                _itemSwitchState = ItemSwitchState.PutUpNew;
                ActiveItemIndex = _itemSwitchNewItemIndex;

                Item newItem = GetItemAtSlotIndex(_itemSwitchNewItemIndex);
                OnSwitchedToItem?.Invoke(newItem);
            }
            else
            {
                _itemSwitchState = ItemSwitchState.PutDownPrevious;
            }
        }
    }

    public Item GetItemAtSlotIndex(int index)
    {
        // Найдите активный предмет в слотах для предмета на основе нашего индекса активного предмета
        if (index >= 0 &&
            index < _itemSlots.Length)
        {
            return _itemSlots[index];
        }

        // Если не нашелся активный предмет в слотах => null
        return null;
    }

    public Item GetActiveItem()
    {
        return GetItemAtSlotIndex(ActiveItemIndex);
    }

    private void UpdateItemSwitching()
    {
        float switchingTimeFactor = 0f;
        if (_itemSwitchDelay == 0f)
        {
            switchingTimeFactor = 1f;
        }
        else
        {
            switchingTimeFactor = Mathf.Clamp01((Time.time - _timeStartedItemSwitch) / _itemSwitchDelay);
        }

        if (switchingTimeFactor >= 1f)
        {
            if (_itemSwitchState == ItemSwitchState.PutDownPrevious)
            {
                // Деактивация старого предмета
                Item oldItem = GetItemAtSlotIndex(ActiveItemIndex);
                if (oldItem != null)
                {
                    oldItem.ShowItem(false);
                }

                ActiveItemIndex = _itemSwitchNewItemIndex;
                switchingTimeFactor = 0f;

                //Активация нового предмета
                Item newItem = GetItemAtSlotIndex(ActiveItemIndex);

                OnSwitchedToItem?.Invoke(newItem);

                if (newItem)
                {
                    _timeStartedItemSwitch = Time.time;
                    _itemSwitchState = ItemSwitchState.PutUpNew;
                }
                else
                {
                    // Если новый предмет пустой, не устанавливать его обратно
                    _itemSwitchState = ItemSwitchState.Down;
                }
            }
            else if (_itemSwitchState == ItemSwitchState.PutUpNew)
            {
                _itemSwitchState = ItemSwitchState.Up;
            }
        }
    }

    private int GetDistanceBetweenItemSlots(int fromSlotIndex, int toSlotIndex, bool ascendingOrder)
    {
        int distanceBetweenSlots = 0;

        if (ascendingOrder)
        {
            distanceBetweenSlots = toSlotIndex - fromSlotIndex;
        }
        else
        {
            distanceBetweenSlots = -1 * (toSlotIndex - fromSlotIndex);
        }

        if (distanceBetweenSlots < 0)
        {
            distanceBetweenSlots = _itemSlots.Length + distanceBetweenSlots;
        }

        return distanceBetweenSlots;
    }

    private void OnItemSwitched(Item newItem)
    {
        newItem?.ShowItem(true);
    }
}
