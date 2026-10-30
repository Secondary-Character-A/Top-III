using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PuzzleMenu : MonoBehaviour
{
    public PuzzleBuilder builder;
    public Material materialBase;
    public Texture2D[] imagens;
    public int[] tamanhos = { 2, 3 };
    public int maxPecas = 10;

    [Header("Aparência")]
    public float tamanhoMiniatura = 0.3f;
    public float espaco = 0.1f;
    public float tamanhoTexto = 0.6f;

    GameObject grupoTamanho;
    GameObject grupoImagens;

    void Awake()
    {
        if (builder != null) builder.imagem = null;
    }

    void Start()
    {
        if (builder == null || materialBase == null || imagens == null || imagens.Length == 0
            || tamanhos == null || tamanhos.Length == 0)
        {
            Debug.LogError("PuzzleMenu: preencha Builder, Material Base, Tamanhos e a lista de Imagens no Inspector.");
            return;
        }

        grupoTamanho = new GameObject("Etapa1_Tamanho");
        grupoTamanho.transform.SetParent(transform, false);
        grupoImagens = new GameObject("Etapa2_Imagem");
        grupoImagens.transform.SetParent(transform, false);

        ConstruirEtapaTamanho();
        ConstruirEtapaImagens();
        MostrarTamanho();
    }

    void MostrarTamanho()
    {
        grupoTamanho.SetActive(true);
        grupoImagens.SetActive(false);
    }

    void MostrarImagens()
    {
        grupoTamanho.SetActive(false);
        grupoImagens.SetActive(true);
    }

    void ConstruirEtapaTamanho()
    {
        var pai = grupoTamanho.transform;
        CriarTexto(pai, "Escolha o tamanho", new Vector3(0, 0.3f, 0), tamanhoTexto * 1.6f);

        var validos = new System.Collections.Generic.List<int>();
        foreach (int g in tamanhos)
            if (g >= 2 && g * g <= maxPecas) validos.Add(g);
        if (validos.Count == 0)
        {
            Debug.LogWarning("PuzzleMenu: nenhum tamanho dentro do limite de " + maxPecas + " peças. Usando 2x2 e 3x3.");
            validos.Add(2);
            validos.Add(3);
        }

        float largura = 0.5f, altura = 0.3f, folga = 0.1f;
        float x0 = -(validos.Count - 1) * (largura + folga) / 2f;
        for (int i = 0; i < validos.Count; i++)
        {
            int g = validos[i];
            CriarBotao(pai,
                g + " x " + g + "\n" + (g * g) + " peças",
                new Vector3(x0 + i * (largura + folga), 0, 0),
                new Vector2(largura, altura),
                new Color(0.15f, 0.4f, 0.9f),
                tamanhoTexto * 1.3f,
                () => { builder.grade = g; MostrarImagens(); });
        }
    }

    void ConstruirEtapaImagens()
    {
        var pai = grupoImagens.transform;
        float passo = tamanhoMiniatura + espaco;
        float x0 = -(imagens.Length - 1) * passo / 2f;

        CriarTexto(pai, "Escolha a imagem", new Vector3(0, tamanhoMiniatura / 2f + 0.15f, 0), tamanhoTexto * 1.6f);

        for (int i = 0; i < imagens.Length; i++)
            if (imagens[i] != null)
                CriarMiniatura(pai, imagens[i], new Vector3(x0 + i * passo, 0, 0));

        float y = -(tamanhoMiniatura / 2f + 0.2f);
        CriarBotao(pai, "Reiniciar", new Vector3(-0.3f, y, 0), new Vector2(0.5f, 0.15f),
            new Color(0.15f, 0.4f, 0.9f), tamanhoTexto, () => builder.Reiniciar());
        CriarBotao(pai, "Trocar tamanho", new Vector3(0.3f, y, 0), new Vector2(0.5f, 0.15f),
            new Color(0.5f, 0.5f, 0.5f), tamanhoTexto, () => MostrarTamanho());
    }

    void CriarMiniatura(Transform pai, Texture2D img, Vector3 pos)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Miniatura_" + img.name;
        go.transform.SetParent(pai, false);
        go.transform.localPosition = pos;
        var escala = new Vector3(tamanhoMiniatura, tamanhoMiniatura, 0.03f);
        go.transform.localScale = escala;

        var moldura = new Material(materialBase);
        moldura.color = new Color(0.15f, 0.15f, 0.15f);
        go.GetComponent<Renderer>().material = moldura;

        var face = GameObject.CreatePrimitive(PrimitiveType.Quad);
        face.name = "Imagem";
        DestroyImmediate(face.GetComponent<Collider>());
        face.transform.SetParent(go.transform, false);
        face.transform.localPosition = new Vector3(0, 0, -0.51f);
        face.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        var m = new Material(materialBase);
        m.mainTexture = img;
        face.GetComponent<Renderer>().material = m;

        var inter = go.AddComponent<XRSimpleInteractable>();
        inter.selectEntered.AddListener(_ => builder.Reconstruir(img));
        inter.hoverEntered.AddListener(_ => go.transform.localScale = escala * 1.12f);
        inter.hoverExited.AddListener(_ => go.transform.localScale = escala);
    }

    void CriarBotao(Transform pai, string texto, Vector3 pos, Vector2 tam, Color cor,
                    float fonte, System.Action acao)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Botao_" + texto.Replace("\n", " ");
        go.transform.SetParent(pai, false);
        go.transform.localPosition = pos;
        var escala = new Vector3(tam.x, tam.y, 0.03f);
        go.transform.localScale = escala;

        var m = new Material(materialBase);
        m.color = cor;
        go.GetComponent<Renderer>().material = m;

        var inter = go.AddComponent<XRSimpleInteractable>();
        inter.selectEntered.AddListener(_ => acao());
        inter.hoverEntered.AddListener(_ => go.transform.localScale = escala * 1.08f);
        inter.hoverExited.AddListener(_ => go.transform.localScale = escala);

        CriarTexto(pai, texto, pos + new Vector3(0, 0, -0.03f), fonte);
    }

    void CriarTexto(Transform pai, string texto, Vector3 pos, float tamanho)
    {
        var go = new GameObject("Texto_" + texto.Replace("\n", " "));
        go.transform.SetParent(pai, false);
        go.transform.localPosition = pos;
        var tmp = go.AddComponent<TextMeshPro>();
        tmp.text = texto;
        tmp.fontSize = tamanho;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white;
    }
}
