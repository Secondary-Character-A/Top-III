using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PuzzleBuilder : MonoBehaviour
{
    [Header("Configuração")]
    public Texture2D imagem;
    public Material materialBase;
    [Range(2, 16)] public int grade = 3;
    public float tamanhoQuadro = 0.6f;
    public float mesaAbaixo = 0.5f;

    [Header("Ajuste da imagem nas peças")]
    public int rotacaoImagem = 0;
    public bool espelharImagem = false;
    public Color corFundoImagem = Color.white;

    [Header("Mensagem de vitória")]
    public float tamanhoTexto = 1.2f;

    Texture2D texturaPronta;
    GameObject conteudo;
    readonly List<GameObject> pecas = new List<GameObject>();
    bool montado;
    float tamanhoAtual;

    void Start()
    {
        if (materialBase == null)
        {
            Debug.LogError("PuzzleBuilder: arraste o Material Base no Inspector.");
            return;
        }
        if (!montado && imagem != null) IniciarMontagem();
    }

    public void Reconstruir(Texture2D novaImagem)
    {
        if (novaImagem != null) imagem = novaImagem;
        Debug.Log("PuzzleBuilder: montando " + grade + "x" + grade);
        IniciarMontagem();
    }

    public void Reiniciar()
    {
        Debug.Log("PuzzleBuilder: reiniciando");
        IniciarMontagem();
    }

    void IniciarMontagem()
    {
        StopAllCoroutines();
        StartCoroutine(MontarDepois());
    }

    IEnumerator MontarDepois()
    {
        LimparConteudo();
        yield return null;
        Montar();
    }

    void LimparConteudo()
    {
        foreach (var p in pecas)
        {
            if (p == null) continue;
            p.SetActive(false);
            Destroy(p);
        }
        pecas.Clear();

        if (conteudo != null)
        {
            conteudo.SetActive(false);
            Destroy(conteudo);
            conteudo = null;
        }
        if (texturaPronta != null)
        {
            Destroy(texturaPronta);
            texturaPronta = null;
        }
    }

    void Montar()
    {
        if (imagem == null || materialBase == null) return;

        tamanhoAtual = tamanhoQuadro / grade;

        conteudo = new GameObject("Conteudo");
        conteudo.transform.SetParent(transform, false);

        texturaPronta = PrepararImagem();
        Construir();
        montado = true;
    }

    Texture2D PrepararImagem()
    {
        var rt = RenderTexture.GetTemporary(imagem.width, imagem.height, 0,
            RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        Graphics.Blit(imagem, rt);

        var anterior = RenderTexture.active;
        RenderTexture.active = rt;
        var tex = new Texture2D(imagem.width, imagem.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = anterior;
        RenderTexture.ReleaseTemporary(rt);

        Color32 f = corFundoImagem;
        var px = tex.GetPixels32();
        for (int i = 0; i < px.Length; i++)
        {
            float a = px[i].a / 255f;
            px[i] = new Color32(
                (byte)(px[i].r * a + f.r * (1f - a)),
                (byte)(px[i].g * a + f.g * (1f - a)),
                (byte)(px[i].b * a + f.b * (1f - a)),
                255);
        }
        tex.SetPixels32(px);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();
        return tex;
    }

    GameObject CriarMensagem()
    {
        var go = new GameObject("MensagemVitoria");
        go.transform.SetParent(conteudo.transform, false);
        go.transform.localPosition = new Vector3(0, tamanhoQuadro / 2f + 0.25f, -0.05f);

        var tmp = go.AddComponent<TextMeshPro>();
        tmp.text = "Muito bem!\nPuzzle completo!";
        tmp.fontSize = tamanhoTexto;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = new Color(0.2f, 1f, 0.3f);

        go.SetActive(false);
        return go;
    }

    AudioClip CriarSomVitoria()
    {
        int taxa = 44100;
        float[] notas = { 523.25f, 659.25f, 783.99f, 1046.5f };
        int n = (int)(taxa * 0.18f);
        var dados = new float[n * notas.Length];
        for (int k = 0; k < notas.Length; k++)
            for (int i = 0; i < n; i++)
            {
                float tempo = i / (float)taxa;
                dados[k * n + i] = Mathf.Sin(2f * Mathf.PI * notas[k] * tempo) * (1f - i / (float)n) * 0.4f;
            }
        var clip = AudioClip.Create("Vitoria", dados.Length, 1, taxa, false);
        clip.SetData(dados, 0);
        return clip;
    }

    void Construir()
    {
        var manager = conteudo.AddComponent<PuzzleManager>();
        manager.mensagemVitoria = CriarMensagem();
        manager.somVitoria = CriarSomVitoria();
        var slots = new List<PuzzleSlot>();

        float t = tamanhoAtual;
        float meio = (grade - 1) / 2f;
        int total = grade * grade;
        float passo = t * 1.25f;
        float topoMesa = -mesaAbaixo;
        float zInicio = -0.35f;

        var fundo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fundo.name = "Fundo";
        fundo.transform.SetParent(conteudo.transform, false);
        fundo.transform.localPosition = new Vector3(0, 0, 0.115f);
        fundo.transform.localScale = new Vector3(grade * t + 0.04f, grade * t + 0.04f, 0.1f);
        var mFundo = new Material(materialBase);
        mFundo.color = new Color(0.2f, 0.2f, 0.2f);
        fundo.GetComponent<Renderer>().material = mFundo;

        float larguraMesa = grade * passo + 0.2f;
        float zCentro = zInicio - (grade - 1) / 2f * passo;
        var mesa = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mesa.name = "Mesa";
        mesa.transform.SetParent(conteudo.transform, false);
        mesa.transform.localPosition = new Vector3(0, topoMesa - 0.025f, zCentro);
        mesa.transform.localScale = new Vector3(larguraMesa, 0.05f, larguraMesa);
        mesa.GetComponent<Renderer>().material = new Material(materialBase);

        var ids = new List<int>();
        for (int i = 0; i < total; i++) ids.Add(i);
        for (int i = ids.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (ids[i], ids[j]) = (ids[j], ids[i]);
        }

        for (int id = 0; id < total; id++)
        {
            int col = id % grade;
            int row = id / grade;
            slots.Add(CriarSlot(id, (col - meio) * t, (row - meio) * t, manager));
        }
        manager.slots = slots.ToArray();

        for (int k = 0; k < total; k++)
        {
            int id = ids[k];
            int c = k % grade;
            int r = k / grade;
            var pos = new Vector3((c - meio) * passo, topoMesa + 0.015f, zInicio - r * passo);
            pecas.Add(CriarPeca(id, pos));
        }
    }

    PuzzleSlot CriarSlot(int id, float x, float y, PuzzleManager manager)
    {
        var go = new GameObject("Slot_" + id);
        go.transform.SetParent(conteudo.transform, false);
        go.transform.localPosition = new Vector3(x, y, 0);

        var col = go.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(tamanhoAtual * 0.8f, tamanhoAtual * 0.8f, 0.12f);

        var socket = go.AddComponent<XRSocketInteractor>();
        socket.showInteractableHoverMeshes = false;

        var slot = go.AddComponent<PuzzleSlot>();
        slot.expectedId = id;
        slot.manager = manager;
        return slot;
    }

    GameObject CriarPeca(int id, Vector3 posLocal)
    {
        int pc = id % grade;
        int pr = id / grade;

        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Peca_" + id;
        go.transform.SetParent(conteudo.transform, false);
        go.transform.localPosition = posLocal;
        go.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        go.transform.localScale = new Vector3(tamanhoAtual, tamanhoAtual, 0.02f);

        var mCorpo = new Material(materialBase);
        mCorpo.color = new Color(0.15f, 0.15f, 0.15f);
        go.GetComponent<Renderer>().material = mCorpo;

        var face = GameObject.CreatePrimitive(PrimitiveType.Quad);
        face.name = "Imagem";
        DestroyImmediate(face.GetComponent<Collider>());
        face.transform.SetParent(go.transform, false);
        face.transform.localPosition = new Vector3(0, 0, -0.51f);
        face.transform.localRotation = Quaternion.Euler(0, 0, rotacaoImagem);
        face.transform.localScale = new Vector3(espelharImagem ? -1f : 1f, 1f, 1f);

        var m = new Material(materialBase);
        m.mainTexture = texturaPronta;
        m.mainTextureScale = new Vector2(1f / grade, 1f / grade);
        m.mainTextureOffset = new Vector2(pc / (float)grade, pr / (float)grade);
        face.GetComponent<Renderer>().material = m;

        var caixa = go.GetComponent<BoxCollider>();
        caixa.size = new Vector3(0.96f, 0.96f, 1f);

        go.AddComponent<PuzzlePiece>().id = id;
        var grab = go.AddComponent<XRGrabInteractable>();
        grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;

        var rb = go.GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        return go;
    }
}
