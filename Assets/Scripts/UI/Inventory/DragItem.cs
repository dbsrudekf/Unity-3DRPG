using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler where T : class
{
    Vector3 startPosition;
    Transform originalParent;
    IDragSource<T> source;

    Canvas parentCanvas;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        source = GetComponentInParent<IDragSource<T>>();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) //드래그가 시작 될 때 호출
    {
        startPosition = transform.position;
        originalParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        transform.SetParent(parentCanvas.transform, true);
    }

    void IDragHandler.OnDrag(PointerEventData eventData) //드래그가 진행되는 동안 호출
    {
        transform.position = eventData.position;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) //드래그가 종료 될 때 호출
    {
        transform.position = startPosition;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        transform.SetParent(originalParent, true);

        IDragDestination<T> container;
        if (!EventSystem.current.IsPointerOverGameObject()) //마우스 포인터가 다른 UI요소 위에 있지 않을 경우
        {
            container = parentCanvas.GetComponent<IDragDestination<T>>();
        }
        else
        {
            container = GetContainer(eventData);
        }

        if (container != null)
        {
            DropItemIntoContainer(container);
        }


    }

    private IDragDestination<T> GetContainer(PointerEventData eventData)
    {
        if (eventData.pointerEnter)
        {
            var container = eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();

            return container;
        }
        return null;
    }

    private void DropItemIntoContainer(IDragDestination<T> destination)
    {
        if (object.ReferenceEquals(destination, source))
        {
            return;
        }

        var destinationContainer = destination as IDragContainer<T>;
        var sourceContainer = source as IDragContainer<T>;
        if (destinationContainer == null || sourceContainer == null ||
            destinationContainer.GetItem() == null ||
            object.ReferenceEquals(destinationContainer.GetItem(), sourceContainer.GetItem()))
        {
            AttemptSimpleTransfer(destination);
            return;
        }

        AttemptSwap(destinationContainer, sourceContainer);
    }

    private void AttemptSwap(IDragContainer<T> destination, IDragContainer<T> source)
    {
        var removedSourceNumber = source.GetNumber();
        var removedSourceItem = source.GetItem();
        var removedDestinationNumber = destination.GetNumber();
        var removedDestinationItem = destination.GetItem();

        source.RemoveItems(removedSourceNumber);
        destination.RemoveItems(removedDestinationNumber);

        var sourceTakeBackNumber = CalculateTakeBack(removedSourceItem, removedSourceNumber, source, destination);
        var destinationTakeBackNumber = CalculateTakeBack(removedDestinationItem, removedDestinationNumber, destination, source);

        if (sourceTakeBackNumber > 0)
        {
            source.AddItems(removedSourceItem, sourceTakeBackNumber);
            removedSourceNumber -= sourceTakeBackNumber;
        }
        if (destinationTakeBackNumber > 0)
        {
            destination.AddItems(removedDestinationItem, destinationTakeBackNumber);
            removedDestinationNumber -= destinationTakeBackNumber;
        }

        if (source.MaxAcceptable(removedDestinationItem) < removedDestinationNumber ||
            destination.MaxAcceptable(removedSourceItem) < removedSourceNumber ||
            removedSourceNumber == 0)
        {
            if (removedDestinationNumber > 0)
            {
                destination.AddItems(removedDestinationItem, removedDestinationNumber);
            }
            if (removedSourceNumber > 0)
            {
                source.AddItems(removedSourceItem, removedSourceNumber);
            }
            return;
        }

        if (removedDestinationNumber > 0)
        {
            source.AddItems(removedDestinationItem, removedDestinationNumber);
        }
        if (removedSourceNumber > 0)
        {
            destination.AddItems(removedSourceItem, removedSourceNumber);
        }
    }

    private bool AttemptSimpleTransfer(IDragDestination<T> destination)
    {
        var draggingItem = source.GetItem();
        var draggingNumber = source.GetNumber();

        var acceptable = destination.MaxAcceptable(draggingItem);
        var toTransfer = Mathf.Min(acceptable, draggingNumber);

        if (toTransfer > 0)
        {
            source.RemoveItems(toTransfer);
            destination.AddItems(draggingItem, toTransfer);
            return false;
        }

        return true;
    }

    private int CalculateTakeBack(T removedItem, int removedNumber, IDragContainer<T> removeSource, IDragContainer<T> destination)
    {
        var takeBackNumber = 0;
        var destinationMaxAcceptable = destination.MaxAcceptable(removedItem);
        if (destinationMaxAcceptable < removedNumber)
        {
            takeBackNumber = removedNumber - destinationMaxAcceptable;

            var sourceTakeBackAcceptable = removeSource.MaxAcceptable(removedItem);

            if (sourceTakeBackAcceptable < takeBackNumber)
            {
                return 0;
            }
        }
        return takeBackNumber;
    }
}
