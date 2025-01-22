using UnityEngine;

public class GameManager : MonoBehaviour
{
    public OpenAnimation openAnimation;

    private void Start(){
        if (openAnimation.gameObject.activeSelf)
        {
            openAnimation.StartOpenAnimation();
        }
    }
}
