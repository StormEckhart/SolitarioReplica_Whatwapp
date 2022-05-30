using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    //Detect current clicks on the GameObject (the one with the script attached)
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        //Output the name of the GameObject that is being clicked
        Debug.Log(name + "Game Object has been clicked");
    }

    //Detect click being moved on the GameObject (the one with the script attached)
    public void OnPointerMove(PointerEventData eventData)
    {
        Debug.Log(name + "Game Object is being dragged");
    }

    //Detect if click on the GameObject has been released (the one with the script attached)
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        Debug.Log(name + "Game Object click has been released");
    }
}
