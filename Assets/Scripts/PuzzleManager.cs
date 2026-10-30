using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    public PuzzleSlot[] slots;
    public GameObject mensagemVitoria;
    public AudioClip somVitoria;
    public UnityEvent onCompleto;

    bool completo;

    public void Check()
    {
        if (slots == null || slots.Length == 0) return;

        bool todosCertos = true;
        foreach (var s in slots)
            if (!s.IsCorrect) { todosCertos = false; break; }

        if (todosCertos && !completo)
        {
            completo = true;
            Debug.Log("Puzzle completo!");
            if (mensagemVitoria != null) mensagemVitoria.SetActive(true);
            if (somVitoria != null) AudioSource.PlayClipAtPoint(somVitoria, transform.position, 1f);
            onCompleto.Invoke();
        }
        else if (!todosCertos && completo)
        {
            completo = false;
            if (mensagemVitoria != null) mensagemVitoria.SetActive(false);
        }
    }
}
