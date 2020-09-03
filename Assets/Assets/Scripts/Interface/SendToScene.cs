using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(InteractSender))]
class SendToScene : MonoBehaviour
{
    [SerializeField] private string scene;//
    private void Awake() => GetComponent<InteractSender>().OnInteract += Send;
    private void Send() => SceneManager.LoadScene(scene);
}