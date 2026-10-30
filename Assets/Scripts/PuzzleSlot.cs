using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class PuzzleSlot : MonoBehaviour
{
    public int expectedId;
    public PuzzleManager manager;
    public bool IsCorrect { get; private set; }

    XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnEnter);
        socket.selectExited.AddListener(OnExit);
    }

    void OnDestroy()
    {
        if (socket == null) return;
        socket.selectEntered.RemoveListener(OnEnter);
        socket.selectExited.RemoveListener(OnExit);
    }

    void OnEnter(SelectEnterEventArgs args)
    {
        var piece = args.interactableObject.transform.GetComponent<PuzzlePiece>();
        IsCorrect = piece != null && piece.id == expectedId;
        if (manager != null) manager.Check();
    }

    void OnExit(SelectExitEventArgs args)
    {
        IsCorrect = false;
        if (manager != null) manager.Check();
    }
}
