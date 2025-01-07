using UnityEngine;
using UnityEngine.Events;

public class BasicInputHandler : MonoBehaviour
{  
    public UnityEvent OnInteractionEvent;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            //Interaction occurs, handle it 
            OnInteractionEvent?.Invoke();
        }
    }
}
