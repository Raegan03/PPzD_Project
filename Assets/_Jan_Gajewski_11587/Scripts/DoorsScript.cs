using UnityEngine;

public class DoorsScript : MonoBehaviour
{
    public void OpenDoors()
    {
        AudioManager.Instance.PlayOpenDoorsClip(transform.position);
        Destroy(gameObject);
    }
}
